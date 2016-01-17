using UnityEngine;
using System.Collections;

public class MindControl : Ability {

	public MindControl() {
		apCost = 5;
		cooldown = 0;
		range = 666;
		friendlyTarget = false;
		abilityName = "Mind Control";
		abilityDescription = "Enthralls a weakened enemy. Costs 5 AP. Requires enemy to be below 50% HP.";
		keyPress = "m";
		currentCooldown = 0;
		targetType = abilityTargetingTypes.One;
	}

	public override IEnumerator Execute(Unit self, GameObject target) {
		self.DeductAP (apCost);
		self.ChangeFacing(gameManager.boardManager.FindDirection(self.transform.position, target.transform.position));
		GameObject mcObj1 = GameObject.Instantiate (gameManager.combatManager.mindControlObj, self.transform.position, self.transform.rotation) as GameObject;
		yield return new WaitForSeconds (1.5f);
		GameObject.Destroy (mcObj1);
		GameObject mcObj2 = GameObject.Instantiate (gameManager.combatManager.mindControlObj, target.transform.position, target.transform.rotation) as GameObject;
		yield return new WaitForSeconds (1.5f);
		GameObject.Destroy (mcObj2);
		target.GetComponent<Unit> ().Flip (self.isAlly);
		gameManager.combatManager.DeactivateTargeting ();
		gameManager.playerInput.TogglePlayerInputLock (false);
		gameManager.combatManager.ToggleActionLock (false);
	}
}
