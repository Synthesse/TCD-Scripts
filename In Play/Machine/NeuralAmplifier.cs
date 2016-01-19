using UnityEngine;
using System.Collections;

public class NeuralAmplifier : Machine {

	public Animator animator;
	public Unit controlledUnit;
	private Leader leader;

	protected override void Awake () {
		currentHP = 5;
		maxHP = 5;
		def = 2;
		objectName = "Neural Amplifier";
	}

	protected override void Start() {
		base.Start ();
		animator = GetComponent<Animator> ();
		animator.enabled = false;
		leader = FindObjectOfType<Leader> ();
		leader.amplifiers.Add (this);
		gameManager.buildManager.UpdateNeuralBuildCost ();
	}

	public override void Kill () {
		//Break rather than remove from play
		if (controlledUnit != null) {
			controlledUnit.Kill ();
			leader.numThralls--;
		}
		leader.amplifiers.Remove (this);
		gameManager.buildManager.UpdateNeuralBuildCost ();
		base.Kill();
		//Instantiate (brokenVersion, this.transform.position, this.transform.rotation);
	}

	public void Reset() {
		controlledUnit = null;
		animator.enabled = false;
	}
}
