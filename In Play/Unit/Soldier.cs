using UnityEngine;
using System.Collections;

public class Soldier : Unit {

	protected override void Awake() {
		currentHP = 10;
		maxHP = 10;
		currentAP = 4;
		maxAP = 4;
		atk = 4;
		def = 1;
		objectName = "Soldier";
		special = "Long Range";
	}

	protected override void Start() {
		base.Start ();
		abilityList.Add (new Attack ());
	}

	protected override void ProcessCombatPanelClick (int buttonNum) {
		switch (buttonNum) {
		case 1:
			//Target Attack
			gameManager.combatManager.ActivateTargeting (abilityList [0]);
			break;
		default:
			break;
		}
	}
}
