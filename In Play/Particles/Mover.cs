using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	public float speed;
	private Vector3 origin;
	private SpriteRenderer spriteRenderer;

	void Start () {
		GetComponent<Rigidbody2D> ().velocity = transform.up * speed;
		spriteRenderer = GetComponent<SpriteRenderer> ();
		origin = transform.position;
		spriteRenderer.enabled = false;
		//GetComponent<Rigidbody2D> ().MovePosition(new Vector2(10,10));
	}

	void OnTriggerStay2D(Collider2D other) {
		if (Vector3.Distance (transform.position, origin) > 0.75f && other.gameObject.layer == 8)
			gameObject.SetActive (false);
	}

	void Update () {
		if (!spriteRenderer.enabled && Vector3.Distance (transform.position, origin) > 0.75f)
			spriteRenderer.enabled = true;
	}

}
