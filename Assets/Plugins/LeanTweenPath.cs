using UnityEngine;
using System.Collections;

[AddComponentMenu("LeanTween/LeanTweenPath")]

public class LeanTweenPath:MonoBehaviour {
	public int count;
	
	public Transform[] pts;
	public Vector3[] path;
	public enum LeanTweenPathType{
		bezier,
		spline
	}
	public LeanTweenPathType pathType;

	private int i;
	private int lastCnt = 1;


	public static Color curveColor;
	public static Color lineColor;

	private void init(){
		
		if(path!=null && path.Length!=0 && (pts == null || pts.Length == 0)){ // transfer over paths made with the legacy path variable
			for (i=transform.childCount-1; i>=0; i--) {
				DestroyImmediate( transform.GetChild(i).gameObject );
			}
			pts = new Transform[ path.Length ];
			for(i=0;i<path.Length;i++){
				if(i>3 && i%4==0){
					
				}else{
					pts[i] = createChild(i, path[i]);
				}
			}
			reset();
			path = new Vector3[0];
			lastCnt = count = 1;
		}

		if(pts == null || pts.Length == 0){ // initial creation
			for (i=transform.childCount-1; i>=0; i--) { // Destroy anything currently a child
				DestroyImmediate( transform.GetChild(i).gameObject );
			}

			if(pathType==LeanTweenPathType.bezier){
				pts = new Transform[]{createChild(0, new Vector3(0f,0f,0f)), createChild(1, new Vector3(5f,0f,0f)), createChild(2, new Vector3(4f,0f,0f)), createChild(3, new Vector3(5f,0f,5f))};
			}else{
				pts = new Transform[]{createChild(0, new Vector3(0f,0f,0f)), createChild(1, new Vector3(2f,0f,0f)), createChild(2, new Vector3(3f,2f,0f))};
			}
			reset();
			lastCnt = count = 1;
		}

		if(lastCnt!=count){ // A curve must have been added or subtracted
			
			if(pathType==LeanTweenPathType.bezier){ // BEZIER
				if(lastCnt>count){ // remove unused points
					int diff = lastCnt - count;
					int k = diff*4;
					for (i=pts.Length-1; k>0; i--) {
						if(pts[i]){
							DestroyImmediate( pts[i].gameObject );
						}
						k--;
					}
				}
				Transform[] newPts = new Transform[ count * 4 ];
				for(i=0;i<newPts.Length;i++){
					if(i<pts.Length){ // transfer old points
						newPts[i] = pts[i];
					}else{ // add in a new point
						if(i%4==1){
							newPts[i] = createChild(i, newPts[i-2].position+new Vector3(5f,0f,0f));
						}else if(i%4==2){
							newPts[i] = createChild(i, newPts[i-3].position+new Vector3(4f,0f,0f));
						}else if(i%4==3){
							newPts[i] = createChild(i, newPts[i-4].position+new Vector3(5f,0f,5f));
						}
					}
				}		
				pts = newPts;
			}else{ // SPLINE
				if(lastCnt>count){ // remove unused points
					int diff = lastCnt - count;
					int k = diff;
					for (i=pts.Length-1; k>0; i--) {
						if(pts[i]){
							DestroyImmediate( pts[i].gameObject );
						}
						k--;
					}
				}

				Transform[] newPts = new Transform[ count ];
				for(i=0;i<newPts.Length;i++){
					if(i<pts.Length){ // transfer old points
						newPts[i] = pts[i];
					}else{ // add in a new point
						newPts[i] = createChild(i, newPts[i-1].localPosition + new Vector3(1f,1f,0f));
					}
				}		
				pts = newPts;

			}
			lastCnt = count;
		}

		reset();
	}

	private void reset(){
		if(pathType==LeanTweenPathType.bezier){
			for(i=0;i<pts.Length;i++){
				LeanTweenPathControl[] ct = new LeanTweenPathControl[]{};
				if(i%4==0){
					if( i+2 < pts.Length)
						ct = new LeanTweenPathControl[]{(LeanTweenPathControl)pts[i+2].gameObject.GetComponent("LeanTweenPathControl"),null};
				}else if(i%4==3){
					ct = new LeanTweenPathControl[]{(LeanTweenPathControl)pts[i-2].gameObject.GetComponent("LeanTweenPathControl"),null};
					if(i+3<pts.Length)
						ct[1] = (LeanTweenPathControl)pts[i+3].gameObject.GetComponent("LeanTweenPathControl");
				}

				if(pts[i]){
					LeanTweenPathControl pathControl = (LeanTweenPathControl)pts[i].gameObject.GetComponent("LeanTweenPathControl");
					pathControl.init( i%4==0||i%4==3, ct);
				}
			}
		}else{
			for(i=0;i<pts.Length;i++){
				LeanTweenPathControl[] ct = new LeanTweenPathControl[]{};
				if(pts[i]){
					LeanTweenPathControl pathControl = (LeanTweenPathControl)pts[i].gameObject.GetComponent("LeanTweenPathControl");
					pathControl.init( ct );
				}
			}
		}
	}

	private Transform createChild(int i, Vector3 pos ){
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.AddComponent( "LeanTweenPathControl" );
		Transform trans = go.transform;
		DestroyImmediate( go.GetComponent( "BoxCollider" ) );
		trans.parent = transform;
		if(pathType==LeanTweenPathType.bezier){
			int iMod = i%4;
			bool isPoint = iMod==0||iMod==3;
			string type = isPoint ? "point" : "control";
			int ptArea = (iMod/2);
			if(isPoint==false){
				ptArea = i==2 ? 0 : 1;
				trans.localScale = new Vector3(0.5f,0.5f,0.25f);
			}else{
				trans.localScale = Vector3.one * 0.5f;
			}
			trans.name = "path"+Mathf.Floor(i/4)+"-"+type+ptArea;
		}else{
			trans.localScale = Vector3.one * 0.5f;
			trans.name = "path"+i;
		}
		// trans.name = "pt"+i;
		trans.localPosition = pos;
	
		return trans;
	}

	void Start () {
		init();
		for(i=0; i < pts.Length; i++){
			if(pts[i])
				pts[i].gameObject.SetActive(false);
		}
	}

	void OnDrawGizmos(){
		init();
		
		if(pathType==LeanTweenPathType.bezier){
			for(int i = 0; i < pts.Length-3; i += 4){
				Vector3 first = i>3 ? pts[i-1].position : pts[i].position;
				
				Gizmos.color = curveColor;
				LeanTween.drawBezierPath(first, pts[i+2].position, pts[i+1].position, pts[i+3].position);
				
				Gizmos.color = lineColor;
				Gizmos.DrawLine(first,pts[i+2].position);
				Gizmos.DrawLine(pts[i+1].position,pts[i+3].position);
			}
		}else{
			
			LTSpline s = new LTSpline( splineVector() );
			Gizmos.color = curveColor;
			s.gizmoDraw();
		}
	}

	public Vector3[] splineVector(){
		Vector3[] p = new Vector3[ pts.Length + 2 ];
		int k = 0;
		for(int i = 0; i < p.Length; i++){
			p[i] = pts[k].position;
			// Debug.Log("k:"+k+" i:"+i);
			if(i>=1 && i < p.Length-2){
				k++;
			}
		}
		return p;
	}

	public Vector3[] vec3{
		get{
			if(pathType==LeanTweenPathType.bezier){
				Vector3[] p = new Vector3[ pts.Length ];
				for(i=0; i < p.Length; i++){
					p[i] = i>3 && i%4==0 ? pts[i-1].position : pts[i].position;
				}
				return p;
			}else{
				return splineVector();
			}
		}
		set{

		}
	}
}