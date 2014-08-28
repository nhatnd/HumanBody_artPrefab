using UnityEngine;
using System.Collections;

public class LTPathExampleCircleCS : MonoBehaviour {

	public LeanTweenPath path;

	private GameObject lt;

	// Use this for initialization
	void Start () {
		lt = GameObject.Find("LeanTweenAvatar");
	
		loopAroundCircle();
	}
	
	void loopAroundCircle(){
		LeanTween.move(lt, path.vec3, 4.0f, new object[]{"orientToPath",true,"delay",1.0f,"ease",LeanTweenType.easeInOutQuad,"onComplete","loopAroundCircle","onCompleteTarget",gameObject});
	}
}
