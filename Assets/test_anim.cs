using UnityEngine;
using System.Collections;

public class test_anim : MonoBehaviour {
	public tk2dSpriteAnimator[] onde_animator;
	public Animation anim;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			Play ();
		}
	
	}

	void Play()
	{
		foreach(tk2dSpriteAnimator sa in onde_animator)
		{
			sa.Stop ();
		}
		foreach(tk2dSpriteAnimator sa in onde_animator)
		{
			sa.Play ();
		}
		anim.Play ();

	}
}
