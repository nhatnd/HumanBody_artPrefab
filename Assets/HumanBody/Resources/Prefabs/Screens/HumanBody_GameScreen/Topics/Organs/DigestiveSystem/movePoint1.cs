using UnityEngine;
using System.Collections;

public class movePoint1 : MonoBehaviour {
	public tk2dBaseSprite pointSprite;
	public tk2dBaseSprite pointBorderSprite;
	public LeanTweenPath path1;
	public LeanTweenPath path2;
	public GameObject point;

	private float animateSeconds = .5f;
	// Use this for initialization
	void Start () {
		LeanTween.move (point,new Vector3[]{new Vector3(4.246088f,37.42023f,-33.85472f), new Vector3(8.754129f,35.31752f,-33.85472f), new Vector3(8.273588f,38.8784f,-33.85472f), new Vector3(8.754129f,35.31752f,-33.85472f), new Vector3(8.754129f,35.31752f,-33.85472f), new Vector3(8.488901f,30.41437f,-38.85472f), new Vector3(9.024204f,31.71601f,-33.85472f), new Vector3(8.902348f,26.94928f,-33.85472f), new Vector3(8.902348f,26.94928f,-33.85472f), new Vector3(10.90971f,26.13512f,-33.85472f), new Vector3(10.06123f,24.04051f,-33.85472f), new Vector3(12.0741f,24.21f,-33.85472f), new Vector3(12.0741f,24.21f,-33.85472f), new Vector3(9.896397f,20.94592f,-33.85472f), new Vector3(12.50753f,22.57325f,-33.85472f), new Vector3(9.896397f,20.94592f,-33.85472f)},5f).setDelay(0f);
		LeanTween.value(gameObject, (float value) => {
			pointSprite.color = new Color(pointSprite.color.r, pointSprite.color.g, pointSprite.color.b, value);
		}, 0.0f, 1.0f, animateSeconds);
		LeanTween.value(gameObject, (float value) => {
			pointSprite.color = new Color(pointSprite.color.r, pointSprite.color.g, pointSprite.color.b, value);
		}, 1.0f, 0.0f, animateSeconds).setDelay(5f);

		LeanTween.value(gameObject, (float value) => {
			pointBorderSprite.color = new Color(pointBorderSprite.color.r, pointBorderSprite.color.g, pointBorderSprite.color.b, value);
		}, 0.0f, 1.0f, animateSeconds);
		LeanTween.value(gameObject, (float value) => {
			pointBorderSprite.color = new Color(pointBorderSprite.color.r, pointBorderSprite.color.g, pointBorderSprite.color.b, value);
		}, 1.0f, 0.0f, animateSeconds).setDelay(5f);




		LeanTween.move (point,path2.vec3,20f).setDelay(10f);
		LeanTween.value(gameObject, (float value) => {
			pointSprite.color = new Color(pointSprite.color.r, pointSprite.color.g, pointSprite.color.b, value);
		}, 0.0f, 1.0f, animateSeconds).setDelay(10f);
		LeanTween.value(gameObject, (float value) => {
			pointSprite.color = new Color(pointSprite.color.r, pointSprite.color.g, pointSprite.color.b, value);
		}, 1.0f, 0.0f, animateSeconds).setDelay(30f);
		
		LeanTween.value(gameObject, (float value) => {
			pointBorderSprite.color = new Color(pointBorderSprite.color.r, pointBorderSprite.color.g, pointBorderSprite.color.b, value);
		}, 0.0f, 1.0f, animateSeconds).setDelay(10f);
		LeanTween.value(gameObject, (float value) => {
			pointBorderSprite.color = new Color(pointBorderSprite.color.r, pointBorderSprite.color.g, pointBorderSprite.color.b, value);
		}, 1.0f, 0.0f, animateSeconds).setDelay(30f);
	
	}
	
	// Update is called once per frame
	void Update () {

	}
}
