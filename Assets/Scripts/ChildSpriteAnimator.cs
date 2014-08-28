using UnityEngine;
using System.Collections;

public class ChildSpriteAnimator : MonoBehaviour {
	
	[SerializeField]SpriteAnimator[] childrenWithAnimator;

	
	void Awake () {
		if (childrenWithAnimator == null) {
			childrenWithAnimator = this.GetComponentsInChildren<SpriteAnimator>();
		}
	}
	
	public void SetFrameOfChildren(int frame) {
		foreach (SpriteAnimator childWithAnimator in childrenWithAnimator) {
			childWithAnimator.SetFrame(frame);
		}
	}
}
