using UnityEngine;
using System.Collections;

public class Turret : Defenses {

	protected override void Awake () {
		currentHP = 5;
		maxHP = 5;
		currentAP = 8;
		maxAP = 8;
		atk = 6;
		def = 4;
		objectName = "Turret";
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
