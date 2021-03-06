﻿using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	public float speed;
	public Vector3 target;
	private Vector3 origin;
	private SpriteRenderer spriteRenderer;
	private float startTime;

	void Start () {
		GetComponent<Rigidbody2D> ().velocity = transform.up * speed;
		spriteRenderer = GetComponent<SpriteRenderer> ();
		origin = transform.position;
		spriteRenderer.enabled = false;
		startTime = Time.time;
		//GetComponent<Rigidbody2D> ().MovePosition(new Vector2(10,10));
	}

//	void OnTriggerStay2D(Collider2D other) {
//		if (Vector3.Distance (transform.position, origin) > 0.75f && other.gameObject.layer == 8)
//			gameObject.SetActive (false);
//	}

	void Update () {
		if (!spriteRenderer.enabled && Vector3.Distance (transform.position, origin) > 0.75f)
			spriteRenderer.enabled = true;
		if (Vector3.Distance (transform.position, target) < 0.5f) {
			gameObject.SetActive (false);
		}
		if (Time.time - startTime > 5) {
			gameObject.SetActive (false);
		}
	}

}
