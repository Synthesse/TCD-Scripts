using UnityEngine;
using System.Collections;

public class MindControl : Ability {

	public MindControl() {
		apCost = 5;
		cooldown = 0;
		range = 8;
		friendlyTarget = false;
		abilityName = "Mind Control";
		abilityDescription = "Enthralls a weakened enemy. Costs 5 AP. Requires enemy to be below 50% HP.";
		abilityButtonText = abilityName+"\n"+apCost+" AP, "+range+" range\nEnemy <50% HP";
		alternateAbilityButtonText = "Ability disabled\nBuild more Neural Amps";
		keyPress = "m";
		currentCooldown = 0;
		targetType = abilityTargetingTypes.One;
	}

	public override IEnumerator Execute(Unit self, GameObject target) {
		self.DeductAP (apCost);
		self.ChangeFacing(gameManager.boardManager.FindDirection(self.transform.position, target.transform.position));
		gameManager.soundManager.PlayPsySFX ();
		GameObject mcObj1 = GameObject.Instantiate (gameManager.combatManager.mindControlObj, self.transform.position, self.transform.rotation) as GameObject;
		yield return new WaitForSeconds (1.5f);
		GameObject.Destroy (mcObj1);
		GameObject mcObj2 = GameObject.Instantiate (gameManager.combatManager.mindControlObj, target.transform.position, target.transform.rotation) as GameObject;
		yield return new WaitForSeconds (1.5f);
		GameObject.Destroy (mcObj2);
		gameManager.soundManager.StopSFXLoop ();
		target.GetComponent<Unit> ().BecomeThrall (self.GetComponent<Leader> ().numThralls, self.isAlly);
		self.GetComponent<Leader> ().AddThrall (target);
		gameManager.combatManager.DeactivateTargeting ();
		gameManager.playerInput.TogglePlayerInputLock (false);
		gameManager.combatManager.ToggleActionLock (false);
	}
}
