using UnityEngine;
using System.Collections;

public class LTSplineTweenCS : MonoBehaviour {

	public LeanTweenPath ltPath;

	public GameObject ltLogo;

	// Use this for initialization
	void Start () {
		
		LeanTween.moveSpline( ltLogo, ltPath.vec3, 3f).setLoopClamp().setOrientToPath(true);
	}
}
