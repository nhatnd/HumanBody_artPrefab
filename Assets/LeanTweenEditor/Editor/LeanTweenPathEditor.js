#pragma strict

import System.Collections.Generic;

@CustomEditor(LeanTweenPath)
@CanEditMultipleObjects

class LeanTweenPathEditor extends Editor {
	private static var MAX_SPLINES = 64;

	var pts : SerializedProperty;
	var path : SerializedProperty;
	var count : SerializedProperty;
	var pathType : SerializedProperty;

	private var drawpath:Vector3[];
	private var _target:LeanTweenPath;

	private enum PathCreateType{
		RectangleRounded,
		Snake,
		Random,
		Circle,
		Straight
	}

	function Awake(){
		interfaceShowControls = EditorPrefs.GetInt("interfaceShowControls", 1) == 1;
		LeanTweenPath.lineColor = Color( EditorPrefs.GetFloat("interfaceLineColor1", 1), EditorPrefs.GetFloat("interfaceLineColor2", 0), EditorPrefs.GetFloat("interfaceLineColor3", 1) );
		LeanTweenPath.curveColor = Color( EditorPrefs.GetFloat("interfaceCurveColor1", 1), EditorPrefs.GetFloat("interfaceCurveColor2", 1), EditorPrefs.GetFloat("interfaceCurveColor3", 1) );
		interfaceCapsColor = Color( EditorPrefs.GetFloat("interfaceCapsColor1", 1), EditorPrefs.GetFloat("interfaceCapsColor2", 1), EditorPrefs.GetFloat("interfaceCapsColor3", 1) );
		interfaceCapsColor2 = Color( EditorPrefs.GetFloat("interfaceCapsColor21", 1), EditorPrefs.GetFloat("interfaceCapsColor22", 1), EditorPrefs.GetFloat("interfaceCapsColor23", 1) );
	}

 	function OnDestroy() {
 		EditorPrefs.SetInt("interfaceShowControls", interfaceShowControls ? 1 : 0);

        EditorPrefs.SetFloat("interfaceLineColor1", LeanTweenPath.lineColor.r);
        EditorPrefs.SetFloat("interfaceLineColor2", LeanTweenPath.lineColor.g);
        EditorPrefs.SetFloat("interfaceLineColor3", LeanTweenPath.lineColor.b);

        EditorPrefs.SetFloat("interfaceCurveColor1", LeanTweenPath.curveColor.r);
        EditorPrefs.SetFloat("interfaceCurveColor2", LeanTweenPath.curveColor.g);
        EditorPrefs.SetFloat("interfaceCurveColor3", LeanTweenPath.curveColor.b);

        EditorPrefs.SetFloat("interfaceCapsColor1", interfaceCapsColor.r);
        EditorPrefs.SetFloat("interfaceCapsColor2", interfaceCapsColor.g);
        EditorPrefs.SetFloat("interfaceCapsColor3", interfaceCapsColor.b);

        EditorPrefs.SetFloat("interfaceCapsColor21", interfaceCapsColor2.r);
        EditorPrefs.SetFloat("interfaceCapsColor22", interfaceCapsColor2.g);
        EditorPrefs.SetFloat("interfaceCapsColor23", interfaceCapsColor2.b);
    }

	function OnEnable () {
		pts = serializedObject.FindProperty ("pts");
		path = serializedObject.FindProperty ("path");
		// Debug.Log( "path size:"+path.arraySize);
        count = serializedObject.FindProperty ("count");
        pathType = serializedObject.FindProperty("pathType");
        _target = target as LeanTweenPath;
        pos = _target.transform.position;

        if(LeanTweenPath.curveColor==Color.black){
        	LeanTweenPath.curveColor = Color.magenta;
        	LeanTweenPath.lineColor = Color.white;
        }

        if(count.intValue==0){
        	serializedObject.Update();
        	count.intValue = 1;
        	serializedObject.ApplyModifiedProperties ();
        }
    }
	
	private var vec3Arr:Vector3[] = new Vector3[ 4 ];
	private var camDistance:float = 1.0;
	private var controlPointSelected:int = -1;

	private var point:SerializedProperty;
	private var i:int;
	private var k:int;
	private var vec3:Vector3;
	private var fieldName:String;
	private var pathShown:boolean[] = new boolean[ MAX_SPLINES ];
	private var pos:Vector3;
	private var lastPos:Vector3;
	private var selectedCreate:PathCreateType = 0;
	private var trans:Transform;

	private var optionsSize:float = 5.0;
	private var optionsBevelSize:float = 1.0;
	private var optionsRangeMin:float = 0.0;
	private var optionsRangeMax:float = 1.0;
	private var optionsLength:int = 2;
	private var optionsSegments:int = 8;
	private var optionsDirection:Vector3 = Vector3(1,0,0);
	private var optionsControlSize:float = 1.0f;

	private var interfaceLineColor:Color = Color.magenta;
	private var interfaceCapsColor:Color = Color.white;
	private var interfaceCapsColor2:Color = Color(16.0/255.0,201.0/255.0,209.0/255.0);
	private var interfaceShowControls:boolean = true;
	private var didCopyIter:int = 0;

	private var sectionsToggled:boolean[] = [false,true,true];

	private function addTo(add:Vector3[]){
		path.arraySize = add.Length;
		for(var i:int=0;i<add.Length;i++){
			point = path.GetArrayElementAtIndex(i);
			point.vector3Value = pos + add[i]*optionsSize;
		}
	}

	function resetPath(){
		pts.arraySize = 0; // set to zero to reset
		if(pathType.intValue==LeanTweenPath.LeanTweenPathType.bezier){
			count.intValue = 4;
		}else{
			count.intValue = 3;
		}
	}

	function OnInspectorGUI(){	
		serializedObject.Update();

		// sectionsToggled[0] = EditorGUILayout.Foldout(sectionsToggled[0], "Interface Options");
		// if(sectionsToggled[0]){
		// 	interfaceShowControls = EditorGUILayout.Toggle("Show Controls", interfaceShowControls);
		// 	LeanTweenPath.curveColor = EditorGUILayout.ColorField("Curve Color", LeanTweenPath.curveColor);
		// 	LeanTweenPath.lineColor = EditorGUILayout.ColorField("Line Color", LeanTweenPath.lineColor);
		// 	interfaceCapsColor = EditorGUILayout.ColorField("Control Color", interfaceCapsColor);
		// 	interfaceCapsColor2 = EditorGUILayout.ColorField("End Color", interfaceCapsColor2);
		// 	EditorGUILayout.LabelField("");
		// }

		var lastPathType:int = pathType.intValue;
		var p:LeanTweenPath.LeanTweenPathType = pathType.intValue;
		pathType.intValue = System.Convert.ToInt32( EditorGUILayout.EnumPopup("Path Type", p) );
		if(lastPathType!=pathType.intValue){
			resetPath();
		}

		if(pathType.intValue==LeanTweenPath.LeanTweenPathType.bezier){
			sectionsToggled[1] = EditorGUILayout.Foldout(sectionsToggled[1], "Easy Path Creator");

			if(sectionsToggled[1]){
				selectedCreate = System.Convert.ToInt32( EditorGUILayout.EnumPopup("Path type to create:", selectedCreate) );

				optionsSize = EditorGUILayout.FloatField("Scale:", optionsSize);
				switch (selectedCreate) {
		                case PathCreateType.RectangleRounded:
		                    optionsBevelSize = EditorGUILayout.Slider("Bevel Size:", optionsBevelSize, 0.0,1.399);
		                    break;
		                case PathCreateType.Snake:
		                    optionsLength = EditorGUILayout.FloatField("Length:", optionsLength);
		                    break;
		                case PathCreateType.Random:
		                	optionsLength = EditorGUILayout.FloatField("Length:", optionsLength);
		                    EditorGUILayout.LabelField("Min Range:", optionsRangeMin.ToString());
				            EditorGUILayout.LabelField("Max Range:", optionsRangeMax.ToString());
				            EditorGUILayout.MinMaxSlider(optionsRangeMin, optionsRangeMax, 0, 10.0);

		                    break;
		                case PathCreateType.Circle:
		                    optionsSegments = closestSegment( EditorGUILayout.IntSlider("Segments:", optionsSegments, 4, MAX_SPLINES) );
		                    break;
		                case PathCreateType.Straight:
		                    optionsLength = EditorGUILayout.FloatField("Length:", optionsLength);
		                    optionsDirection = EditorGUILayout.Vector3Field("Direction:", optionsDirection);
		                    break;
		                default:
		                    break;
		            }

		        var createPressed:boolean = false;

		        if(GUILayout.Button("Create New Path")){
		        	
		        	// path.arraySize = count.intValue*4;
		        	switch (selectedCreate) {
		                case PathCreateType.RectangleRounded:
		                    path.arraySize = 8;
		                    pts.arraySize = 0;
		                    var b:float = optionsBevelSize;
		                    var n:float = 0.1;
				           	addTo( 	[ 
				           	 Vector3(b,0,0),Vector3(0,0,0),Vector3(0,0,0),Vector3(0,0,b)
				           	,Vector3(0,0,b),Vector3(0,0,b+n),Vector3(0,0,3-b-n),Vector3(0,0,3-b)
				           	,Vector3(0,0,3-b),Vector3(0,0,3),Vector3(0,0,3),Vector3(b,0,3)
				           	,Vector3(b,0,3),Vector3(3-b-n,0,3),Vector3(b+n,0,3),Vector3(3-b,0,3)
				           	,Vector3(3-b,0,3),Vector3(3,0,3),Vector3(3,0,3),Vector3(3,0,3-b)
				           	,Vector3(3,0,3-b),Vector3(3,0,b+n),Vector3(3,0,b+n),Vector3(3,0,b)
				           	,Vector3(3,0,b),Vector3(3,0,0),Vector3(3,0,0),Vector3(3-b,0,0)
				           	,Vector3(3-b,0,0),Vector3(3-b-n,0,0),Vector3(b+n,0,0),Vector3(b,0,0)
				           	] );
		                    break;
		                case PathCreateType.Snake:
			                path.arraySize = optionsLength*4;
		                	pts.arraySize = 0;
		                    k = 0;
							for(i=0; i < path.arraySize; i++){
					        	vec3 = pos + Vector3(k*optionsSize,0,k*optionsSize);
					        	if(i%4==1){
					        		vec3.x += 1*optionsSize;
					        	}
					        	if(i%4==2){
					        		vec3.x += -1*optionsSize;
					        	}
					        	point = path.GetArrayElementAtIndex(i);
								point.vector3Value = vec3;
								if(i%2==0)
									k++;
					        }
		                    break;
		                case PathCreateType.Random:
		                	path.arraySize = optionsLength*4;
		                	pts.arraySize = 0;
		                    k = 0;
		                    var rand:float;
		                    for(i=0; i < path.arraySize; i++){
					        	vec3 = pos + Vector3(k*optionsSize,0,0);
					        	
					        	if(i%4==1){
					        		vec3.z += rand*optionsSize;
					        	}
					        	rand = Random.Range(optionsRangeMin,optionsRangeMax)*optionsSize;
					        	if(i%4==2){
					        		vec3.z += -rand*optionsSize;
					        	}
					        	point = path.GetArrayElementAtIndex(i);
								point.vector3Value = vec3;
								if(i%2==0)
									k++;
					        }
		                    break;
		                case PathCreateType.Circle:
		                	pts.arraySize = 0;
		                	count.intValue = optionsSegments;
		                    var v:Vector3[] = generateCircularQuadraticBezierSegments(optionsSize,count.intValue);
		                    addTo(v);
		                    break;
		                case PathCreateType.Straight:
		                	k = 0;
		                	path.arraySize = optionsLength*4;
		                	pts.arraySize = 0;
		                	for(i=0; i < path.arraySize; i++){
					        	point = path.GetArrayElementAtIndex(i);
							 	point.vector3Value = pos + k*optionsDirection*optionsSize;
					        	if(i%2==0)
									k++;
					        }
		                    break;
		                default:
		                    Debug.LogError("Unrecognized Option");
		                    break;
		            }
		        }
		    }

			GUILayout.Label("\nEdit Path");
			// EditorGUILayout.IntSlider (count, 1, MAX_SPLINES, new GUIContent ("Curve Count"));
	       
	       	if(pathType.intValue==LeanTweenPath.LeanTweenPathType.bezier){
		        if(GUILayout.Button("Add Curve")){
					count.intValue = (pts.arraySize/4)+1;
		        }
		        if(GUILayout.Button("Remove Curve")){
					count.intValue = (pts.arraySize/4)-1;
		        }
		    }

	        if(createPressed){
	        	count.intValue = path.arraySize/4;
	        }

	        // GUILayout.Label("");
	        if(GUILayout.Button("Reverse Path Direction")){
	        	var newPath:Vector3[] = new Vector3[ pts.arraySize ];
	        	for(i=0; i < newPath.Length; i++){
	        		// Debug.Log("pt at:"+(newPath.Length-i-1)+ " len:"+newPath.Length);
	        		point = pts.GetArrayElementAtIndex(newPath.Length-i-1);
	        		trans = point.objectReferenceValue as Transform;
	        		if(trans==null){
	        			point = pts.GetArrayElementAtIndex(newPath.Length-i-2);
		        		trans = point.objectReferenceValue as Transform;
	        		}
	        		newPath[i] = trans.position;
	        	}

	        	path.arraySize = pts.arraySize;
		        pts.arraySize = 0;
	        	for(i=0; i < path.arraySize; i++){
		        	point = path.GetArrayElementAtIndex(i);
					point.vector3Value = newPath[i];
		        }
	        }

	        var arraySize:float = pts.arraySize;
	        var outputStr:String;
	        
			GUILayout.Label("\nOutput");
			
			if(GUILayout.Button("Copy C# Vector3 Array")){
				outputStr = "new Vector3[]{";
		        for(i=0; i < arraySize; i++){
					point = i>3 && i%4==0 ? pts.GetArrayElementAtIndex(i-1) : pts.GetArrayElementAtIndex(i);
					trans = point.objectReferenceValue as Transform;
					outputStr += "new Vector3("+trans.position.x+"f,"+trans.position.y+"f,"+trans.position.z+"f)";
					if(i<arraySize-1)
						outputStr += ", ";
				}
				outputStr += "}";
				EditorGUIUtility.systemCopyBuffer = outputStr;
				didCopyIter = 8;
	        }
	        if(GUILayout.Button("Copy Unityscript Vector3 Array")){
	        	outputStr = "[";
		        for(i=0; i < arraySize; i++){
					point = i>3 && i%4==0 ? pts.GetArrayElementAtIndex(i-1) : pts.GetArrayElementAtIndex(i);
					trans = point.objectReferenceValue as Transform;
					outputStr += "Vector3("+trans.position.x+","+trans.position.y+","+trans.position.z+")";
					if(i<arraySize-1)
						outputStr += ", ";
				}
				outputStr += "]";
				EditorGUIUtility.systemCopyBuffer = outputStr;
				didCopyIter = 8;
	        }
	        if(didCopyIter>0){
	        	GUILayout.Label("Copied to buffer");
	        	didCopyIter--;
	        }

	    }else{
	    	GUILayout.Label("\nEdit Path");
			
	    	if(GUILayout.Button("Add Point")){
				count.intValue = pts.arraySize+1;
	        }
	        if(GUILayout.Button("Remove Point")){
				count.intValue = pts.arraySize-1;
	        }
	    }

	    GUILayout.Label("Destroy");
	    if(GUILayout.Button("Clear Path")){
			resetPath();
        }

        GUILayout.Label("\nSettings");
        var lastoptionsControlSize = optionsControlSize;
	    optionsControlSize = EditorGUILayout.Slider("Control Sizes:", optionsControlSize, 0.01,10f);
	    if(lastoptionsControlSize!=optionsControlSize){
		    for(i=0; i < arraySize; i++){
				point = i>3 && i%4==0 ? pts.GetArrayElementAtIndex(i-1) : pts.GetArrayElementAtIndex(i);
				trans = point.objectReferenceValue as Transform;
				trans.localScale = Vector3.one * optionsControlSize;
			}
		}
        	
       

		// Preview section
		// preview own models, or LeanTween model

		serializedObject.ApplyModifiedProperties();
	}

	private function closestSegment( val:int ):int{
		if(val<6)
			return 4;
		if(val < 12)
			return 8;
		if(val < 24)
			return 16;
		if(val < 48)
			return 32;

		return 64;
	}

	private static var fudgeMulti:float[] = [0,0,0,0.16, 0,0,0,0.04, 0,0,0,0.02, 0,0,0,0.01, 0,0,0,0.2, 0,0,0,0.06, 0,0,0,0.02, 0,0,0,0.002,
	0,0,0,0.2, 0.2,0.2,0.2,0.06 ,0.06,0.06,0.06,0.02 ,0.06,0.06,0.06,0.02, 0,0,0,0.2, 0.2,0.2,0.2,0.06 ,0.06,0.06,0.06,0.02 ,0.06,0.06,0.06,0.0003];

	public static function generateCircularQuadraticBezierSegments(radius:float, numControlPoints:int):Vector3[]
    {
        var segments:Vector3[] = new Vector3[ numControlPoints * 4];
        var arcLength:float = 2 * Mathf.PI / numControlPoints;
        var controlRadius:float;
 		
 		var fudge:float = 0.14;
 		// Debug.Log("arcLength:"+arcLength + " fudge:"+fudge);
 		var controlMult:float = 1 - fudgeMulti[ numControlPoints - 1];// - 1.0 / numControlPoints * 0.5;

        for (var i:int = 0; i < numControlPoints; i++) {

            var startX:float = radius * Mathf.Cos(arcLength * i);
            var startY:float = radius * Mathf.Sin(arcLength * i);
            segments[i*4+0] = Vector3(startX, 0, startY);

            //control radius formula
            //where does it come from, why does it work?
            controlRadius = radius / Mathf.Cos(arcLength * .5);

            //the control point is plotted halfway between the arcLength and uses the control radius
            var controlX:float = controlRadius * Mathf.Cos(arcLength * (i + 1 + fudge) - arcLength * .5) * controlMult;
            var controlY:float = controlRadius * Mathf.Sin(arcLength * (i + 1 + fudge) - arcLength * .5) * controlMult;
            segments[i*4+1] = Vector3(controlX, 0, controlY);

            controlX = controlRadius * Mathf.Cos(arcLength * (i + 1 - fudge) - arcLength * .5) * controlMult;
            controlY = controlRadius * Mathf.Sin(arcLength * (i + 1 - fudge) - arcLength * .5) * controlMult;
            segments[i*4+2] = Vector3(controlX, 0, controlY);

            var endX:float = radius * Mathf.Cos(arcLength * (i + 1));
            var endY:float = radius * Mathf.Sin(arcLength * (i + 1));
            segments[i*4+3] = Vector3(endX, 0, endY);
        }

        return segments;
    }

	@MenuItem("GameObject/Create Other/LeanTweenPath")
	static function CreateLeanTweenPath()
	{
		var go:GameObject = new GameObject("LeanTweenPath");
		go.AddComponent(LeanTweenPath);
	}
}



