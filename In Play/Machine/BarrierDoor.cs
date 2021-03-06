﻿using UnityEngine;
using System.Collections;

public class BarrierDoor : Defenses {

	public GameObject brokenVersion;
	protected Animator animator;

	protected override void Awake () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		storedColor = spriteRenderer.color;
		currentHP = 4;
		maxHP = 4;
		currentAP = 1;
		maxAP = 1;
		atk = 0;
		def = 2;
		objectName = "Barrier Door";
		special = "Activate to project barrier";
	}

	protected override void Start() {
		base.Start ();
		animator = GetComponent<Animator> ();
		abilityList.Add(new DoorBarrierAbility());
		numCombatActions = 1;
		attackable = false;
	}

	public override void Kill () {
		base.Kill();
		//Instantiate (brokenVersion, this.transform.position, this.transform.rotation);
	}

	protected override void ProcessCombatPanelClick (int buttonNum) {
		switch (buttonNum) {
		case 1:
			if (abilityList [0].apCost <= currentAP) {
				gameManager.playerInput.TogglePlayerInputLock (true);
				gameManager.combatManager.ToggleActionLock (true);
				StartCoroutine (abilityList [0].Execute (this));
			}
			break;
		case 2:
			break;
		default:
			break;
		}
	}

//	public void Rotate () {
//		if (rotated) {
//			this.transform.Rotate (0, 0, -90);
//			rotated = false;
//		} else {
//			this.transform.Rotate (0, 0, 90);
//			rotated = true;
//		}
//	}
}
