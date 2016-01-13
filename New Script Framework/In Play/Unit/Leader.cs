using UnityEngine;
using System.Collections;

public class Leader : Unit {

	protected override void Awake ()
	{
		objectName = "Valerie";
		currentHP = 20;
		maxHP = 20;
		currentAP = 6;
		maxAP = 6;
		atk = 6;
		def = 2;
	}

	public override void Damage (int damageTaken)
	{
		base.Damage (damageTaken);
		if (currentHP <= 0)
			gameManager.GameOver ();
	}


	protected void TargetTestAttack() {
		gameManager.combatManager.ActivateTargeting ("ExecuteTestAttack");
	}

	protected void ExecuteTestAttack(GameObject hitTarget) {
		int attackAPCost = 2;
		hitTarget.SendMessage ("Damage", atk, SendMessageOptions.DontRequireReceiver);
		DeductAP (attackAPCost);
		ScanPaths ();
	}

	protected override void ProcessCombatPanelClick (int buttonNum) {
		switch (buttonNum) {
		case 1:
			TargetTestAttack ();
			break;
		case 2:
			break;
		default:
			break;
		}
	}
}
