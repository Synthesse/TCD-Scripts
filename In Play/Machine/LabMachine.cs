using UnityEngine;
using System.Collections;

public class LabMachine : Machine {

	protected Animator animator;

	protected override void Awake () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		storedColor = spriteRenderer.color;
		currentHP = 5;
		maxHP = 5;
		def = 2;
		objectName = "The Machine";
		special = "Increases income";
	}

	protected override void Start() {
		base.Start ();
		animator = GetComponent<Animator> ();
		gameManager.income++;
	}

	public override void Kill () {
		//Break rather than remove from play
		gameManager.income--;
		base.Kill();
		//Instantiate (brokenVersion, this.transform.position, this.transform.rotation);
	}

}
