using UnityEngine;
using System.Collections;

public class OldWall : MonoBehaviour {

	public int hp = 10;

	private SpriteRenderer spriteRenderer;

	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	public void DamageWall (int loss) {
		hp -= loss;
		if (hp <= 0)
			gameObject.SetActive (false);
	}
}
