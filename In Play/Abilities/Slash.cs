using UnityEngine;
using System.Collections;

public class Slash : Ability {

	public Slash() {
		apCost = 1;
		cooldown = 0;
		range = 1;
		friendlyTarget = false;
		abilityName = "Slash";
		abilityDescription = "Slashes a single enemy in melee range. Costs 1 AP. Deals damage = atk.";
		abilityButtonText = abilityName+"\n"+apCost+" AP, "+range+" range";
		keyPress = "a";
		currentCooldown = 0;
		targetType = abilityTargetingTypes.One;
	}

	public override IEnumerator Execute(Unit self, GameObject target) {
		self.ChangeFacing(gameManager.boardManager.FindDirection(self.transform.position, target.transform.position));
		target.SendMessage ("SetDamageDirection", (Vector2)self.transform.position, SendMessageOptions.DontRequireReceiver);
		gameManager.soundManager.PlaySlashSFX ();
		GameObject slash = GameObject.Instantiate(gameManager.combatManager.slashAttackObj, target.transform.position, Quaternion.Euler(0,0,Random.Range(0,359))) as GameObject;
		//		while (laser.activeInHierarchy)
		//			yield return null;
		yield return new WaitForSeconds(0.5f);
		target.SendMessage ("Damage", self.atk, SendMessageOptions.DontRequireReceiver);
		self.ScanPaths ();
		self.DeductAP (apCost);
		gameManager.combatManager.DeactivateTargeting ();
		gameManager.playerInput.TogglePlayerInputLock (false);
		gameManager.combatManager.ToggleActionLock (false);
	}
}
