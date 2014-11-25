using UnityEngine;
//using UnityEditor;
using System.Collections;

//[ExecuteInEditMode]
public class ClippingAnimator : MonoBehaviour {
	
	public Vector2 clipTopRight = new Vector2(1,0);
	tk2dClippedSprite sprite = null;
	
	// Use this for initialization
	void Start() {
		sprite = GetComponent<tk2dClippedSprite>();
		
		if (sprite != null)
		{
			sprite._clipTopRight = clipTopRight;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (sprite != null && sprite._clipTopRight != clipTopRight)
		{
			sprite._clipTopRight = clipTopRight;
			sprite.Build();
		}
	}
}