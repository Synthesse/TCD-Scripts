using UnityEngine;
using System.Collections;

public class Soldier : Unit {

	protected override void Awake() {
		currentHP = 10;
		maxHP = 10;
		currentAP = 4;
		maxAP = 4;
		atk = 5;
		def = 1;
		objectName = "Soldier";
		special = "Long Range";
		aiAttackRange = 8;
	}

	protected override void Start() {
		base.Start ();
		abilityList.Add (new Attack (8));
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
