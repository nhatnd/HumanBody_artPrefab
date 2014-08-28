using UnityEngine;
using System.Collections;

public class testLeanTween : MonoBehaviour {

	public LeanTweenPath[] path;
	public GameObject point;
	private GameObject[] sandParticle;

	// Use this for initialization
	void Start () {
		sandParticle = new GameObject[8];
		for (int i=0;i<8;i++)
		{
			sandParticle[i] = Instantiate(point) as GameObject;
			LTplay(sandParticle[i],path[i*2],2f,0f);
			LTplay(sandParticle[i],path[i*2+1],10f,2f);

		}
	}

	void LTplay(GameObject gameobject,LeanTweenPath path,float speed,float delay){
		LeanTween.move (gameobject,path.vec3,speed).setDelay(delay);
	}
	
}
