using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	private SpriteRenderer spriteRenderer;

	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}
}
