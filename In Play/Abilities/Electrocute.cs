using UnityEngine;
using System.Collections;

public class Electrocute : Ability {

	public Electrocute() {
		apCost = 1;
		cooldown = 0;
		range = 1;
		friendlyTarget = false;
		abilityName = "Electrocute";
		abilityDescription = "Electrocutes a single enemy in melee range. Costs all remaining AP. Deals damage = remaining AP. Deals double to machines";
		abilityButtonText = abilityName+"\n"+apCost+" AP, "+range+" range";
		keyPress = "a";
		currentCooldown = 0;
		targetType = abilityTargetingTypes.One;
	}

	public override IEnumerator Execute(Unit self, GameObject target) {
		self.ChangeFacing(gameManager.boardManager.FindDirection(self.transform.position, target.transform.position));
		gameManager.soundManager.playElectricSFX ();
		GameObject electric = GameObject.Instantiate(gameManager.combatManager.electrocuteAttackObj, target.transform.position, Quaternion.identity) as GameObject;
		//		while (laser.activeInHierarchy)
		//			yield return null;
		yield return new WaitForSeconds(1.5f);
		int dmg = self.atk * self.currentAP;
		if (target.GetComponent<Machine> () != null)
			dmg *= 2;
		target.SendMessage ("Damage", dmg, SendMessageOptions.DontRequireReceiver);
		self.ScanPaths ();
		self.DeductAP (self.currentAP);
		gameManager.combatManager.DeactivateTargeting ();
		gameManager.playerInput.TogglePlayerInputLock (false);
		gameManager.combatManager.ToggleActionLock (false);
	}
}
