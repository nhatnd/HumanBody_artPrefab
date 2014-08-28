using UnityEngine;
using System.Collections;

public class test_particle : MonoBehaviour {
	public ParticleSystem ps;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			ps.Play();
		}
	}
}
