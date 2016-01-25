using UnityEngine;
using System.Collections;

public class Turret : Defenses {

	protected override void Awake () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		storedColor = spriteRenderer.color;
		currentHP = 7;
		maxHP = 7;
		currentAP = 4;
		maxAP = 4;
		atk = 4;
		def = 2;
		objectName = "Turret";
		special = "Long range fire; static";
	}

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		currentFacing = direction.None;
		currentFacing8 = direction8.Up;
		abilityList.Add(new TurretAttack());
		numCombatActions = 1;
	}


	protected override void ProcessCombatPanelClick (int buttonNum) {
		switch (buttonNum) {
		case 1:
			//Target Attack
			if (abilityList [0].apCost <= currentAP) {
				gameManager.combatManager.ActivateTargeting (abilityList [0]);
			}
			break;
		default:
			break;
		}
	}
}
