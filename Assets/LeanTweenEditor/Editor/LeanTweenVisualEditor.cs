namespace CriticalState.TweenSys
{
	#region Using
	using UnityEditor;
	using UnityEngine;
	#endregion

	[CustomEditor (typeof(LeanTweenVisual))]
	public class LeanTweenVisualEditor : Editor
	{
		public static LeanTweenVisual copyObj = null;

		public static Color colorGroupName = new Color(0f/255f,162f/255f,255f/255f);
		public static Color colorDelete = new Color(255f/255f,25f/255f,25f/255f);
		public static Color colorTweenName = new Color(0f/255f,255f/255f,30f/255f);
		public static Color colorAddTween = new Color(0f/255f,209f/255f,25f/255f);
		public static Color colorAddGroup = new Color(0f/255f,144f/255f,226f/255f);

		public override void OnInspectorGUI()
		{
			LeanTweenVisual tween = target as LeanTweenVisual;
			float overallDelay = 0;
			bool clicked, deleted;
			Vector3 vec;

			/*EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Copy", GUILayout.Width(100)))
			{
				copyObj = tween;
			}
			if(copyObj == null)
			{
				GUI.color = Color.gray;
			}
			if(GUILayout.Button("Paste", GUILayout.Width(100)))
			{
				if(copyObj != null)
				{
					copyObj.CopyTo(tween);
				}
				else
				{
					Debug.LogError("Nothing to paste.");
				}
			}
			
			GUI.color = Color.white;

			if(copyObj != null)
			{
				EditorGUILayout.LabelField("Clipboard: " + copyObj.name);
			}
			EditorGUILayout.EndHorizontal();*/

			tween.restartOnEnable = EditorGUILayout.Toggle("Restart on enable", tween.restartOnEnable);
			tween.repeat = EditorGUILayout.Toggle("Repeat", tween.repeat);
			if(tween.repeat)
				tween.repeatDelay = EditorGUILayout.FloatField("Repeat Delay", tween.repeatDelay);

			float addedGroupDelay = 0f;
			foreach(LeanTweenGroup group in tween.groupList)
			{
				EditorGUILayout.Space();

				GUI.color = colorGroupName;
				EditorGUILayout.BeginHorizontal();
				clicked = GUILayout.Button("Group: " + group.name + " " + (group.startTime + overallDelay) + "s - " + (group.endTime + overallDelay) + "s");
				GUI.color = colorDelete;
				deleted = GUILayout.Button("Delete", GUILayout.Width(52));
				EditorGUILayout.EndHorizontal();
				GUI.color = Color.white;

				if(clicked)
				{
					group.foldout = !group.foldout;
				}
				if (deleted)
				{
					tween.groupList.Remove(group);
					break;
				}

				float addedTweenDelay = 0f;
				if(group.foldout)
				{

					group.name = EditorGUILayout.TextField("Group Name", group.name);
					group.delay = EditorGUILayout.FloatField("Group Delay", group.delay);
					foreach(LeanTweenItem item in group.itemList)
					{

						GUI.color = colorTweenName;
						EditorGUILayout.BeginHorizontal();
						GUILayout.Space(15);
						clicked = GUILayout.Button("Tween: "+item.action + " " + (overallDelay + group.delay + item.delay) + "s - " + (overallDelay + group.delay + item.delay + item.duration) + "s");
						GUI.color = colorDelete;
						deleted = GUILayout.Button("Delete", GUILayout.Width(52));
						EditorGUILayout.EndHorizontal();
						GUI.color = Color.white;


						if(clicked)
						{
							item.foldout = !item.foldout;
						}
						if (deleted)
						{
							group.itemList.Remove(item);
							break;
						}

						if(item.foldout)
						{
							//item.name = EditorGUILayout.TextField("    Name", item.name);
							item.action = (TweenAction)EditorGUILayout.EnumPopup("    Action", item.action);

							if(item.action == TweenAction.MOVE_CURVED || item.action == TweenAction.MOVE_CURVED_LOCAL)
							{
								item.bezierPath = EditorGUILayout.ObjectField(item.bezierPath, typeof(LeanTweenPath), true) as LeanTweenPath;
								item.orientToPath = EditorGUILayout.Toggle("    Orient to Path", item.orientToPath);
							}

							if(item.action == TweenAction.MOVE || item.action == TweenAction.MOVE_LOCAL || item.action == TweenAction.ROTATE || item.action == TweenAction.ROTATE_LOCAL || item.action == TweenAction.SCALE  || item.action == TweenAction.DELAYED_SOUND)
							{
								// VECTORS
								if(item.between == LeanTweenBetween.FromTo)
								{
									item.from = EditorGUILayout.Vector3Field("    From", item.from);
								}
								item.to = EditorGUILayout.Vector3Field("    To", item.to);
							} else {
								// FLOATS
								if(item.between == LeanTweenBetween.FromTo)
								{
									vec = Vector3.zero;
									vec.x = EditorGUILayout.FloatField("    From", item.from.x);
									item.from = vec;
								}
								vec = Vector3.zero;
								vec.x = EditorGUILayout.FloatField("    To", item.to.x);
								item.to = vec;
							}

							if(item.action == TweenAction.ROTATE_AROUND)
							{
								item.axis = ShowAxis("    Axis", item.axis);
							}

							if(item.action == TweenAction.DELAYED_SOUND){
								item.audioClip = EditorGUILayout.ObjectField(item.audioClip, typeof(AudioClip), true) as AudioClip;
							}else{
								item.between = (LeanTweenBetween)EditorGUILayout.EnumPopup("    Between", item.between);
								item.ease = (LeanTweenType)EditorGUILayout.EnumPopup("    Ease", item.ease);
								if(item.ease==LeanTweenType.animationCurve){
									item.animationCurve = EditorGUILayout.CurveField("    Ease Curve", item.animationCurve);
								}
							}
							item.delay = EditorGUILayout.FloatField("    Delay", item.delay);
							
							if(item.action == TweenAction.DELAYED_SOUND){
								item.duration = EditorGUILayout.FloatField("    Volume", item.duration);
							}else{
								item.duration = EditorGUILayout.FloatField("    Duration", item.duration);
							}
							addedTweenDelay += item.duration;
							

							EditorGUILayout.Separator();
							EditorGUILayout.Separator();
						}
					}
					if (ShowLeftButton("+ Tween", colorAddTween, 15))
					{
						group.itemList.Add(new LeanTweenItem(addedTweenDelay));
					}
					addedGroupDelay += addedTweenDelay;

					EditorGUILayout.Separator();
				}
				overallDelay += group.endTime;
			}

			if (ShowLeftButton("+ Group", colorAddGroup))
			{
				tween.groupList.Add(new LeanTweenGroup(addedGroupDelay));
			}
		}
		
		private bool ShowRightButton(string label, Color color)
		{
			bool clicked;
			GUI.color = color;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			clicked = GUILayout.Button(label, GUILayout.Width(100));
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			return clicked;
		}

		private bool ShowLeftButton(string label, Color color)
		{
			return ShowLeftButton(label, color, 0);
		}

		private bool ShowLeftButton(string label, Color color, float space)
		{
			bool clicked;
			GUI.color = color;
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(space);
			clicked = GUILayout.Button(label, GUILayout.Width(100));
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			return clicked;
		}

		private Vector3 ShowAxis(string label, Vector3 value)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(15);
			if(GUILayout.Button("Up"))
			{
				value = Vector3.up;
			}
			if(GUILayout.Button("Down"))
			{
				value = Vector3.down;
			}
			if(GUILayout.Button("Left"))
			{
				value = Vector3.left;
			}
			if(GUILayout.Button("Right"))
			{
				value = Vector3.right;
			}
			if(GUILayout.Button("Back"))
			{
				value = Vector3.back;
			}
			if(GUILayout.Button("Forward"))
			{
				value = Vector3.forward;
			}
			EditorGUILayout.EndHorizontal();
			
			return EditorGUILayout.Vector3Field("    Axis", value);
		}
	}
}