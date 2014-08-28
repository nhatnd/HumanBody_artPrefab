using UnityEngine;
using System.Collections;

public class SpriteAnimator : MonoBehaviour {

	tk2dBaseSprite sprite;
	string spriteName;
	
	public void SetFrame (int id) {
		if (sprite == null) {
			sprite = this.GetComponent<tk2dBaseSprite>();
		}
		spriteName = sprite.GetCurrentSpriteDef().name;
		spriteName = spriteName.Substring(0,spriteName.Length-1);
		sprite.SetSprite(sprite.GetSpriteIdByName(spriteName + id.ToString()));		
	}
}
