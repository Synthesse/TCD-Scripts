using UnityEngine;
using System.Collections;

public class Leader : Unit {

	protected override void Start ()
	{
		base.Start ();
		objectName = "Valerie";
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
