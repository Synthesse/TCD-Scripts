using UnityEngine;
using System.Collections;

public class LabMachine : Machine {

	protected Animator animator;

	protected override void Awake () {
		currentHP = 5;
		maxHP = 5;
		def = 2;
		objectName = "The Machine";
	}

	protected override void Start() {
		base.Start ();
		animator = GetComponent<Animator> ();
	}

	public override void Kill () {
		//Break rather than remove from play
		base.Kill();
		//Instantiate (brokenVersion, this.transform.position, this.transform.rotation);
	}

}
