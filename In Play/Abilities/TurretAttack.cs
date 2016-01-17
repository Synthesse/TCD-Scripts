﻿using UnityEngine;
using System.Collections;

public class TurretAttack : Ability {

	public TurretAttack() {
		apCost = 2;
		cooldown = 0;
		range = 666;
		friendlyTarget = false;
		abilityName = "Attack";
		abilityDescription = "Fires at a single enemy. Costs 2 AP. Deals damage = atk.";
		keyPress = "a";
		currentCooldown = 0;
		targetType = abilityTargetingTypes.One;
	}

	public override IEnumerator Execute(Defenses self, GameObject target) {
		self.DeductAP (apCost);
		self.ChangeFacing(gameManager.boardManager.FindDirection8(self.transform.position, target.transform.position));
		GameObject laser = GameObject.Instantiate(self.laserAttackObj, self.transform.position, Quaternion.Euler(0, 0, Mathf.Rad2Deg*(gameManager.boardManager.FindAngle(self.transform.position, target.transform.position)+Mathf.PI)+90)) as GameObject;
		//		while (laser.activeInHierarchy)
		//			yield return null;
		yield return new WaitWhile(() => laser.activeInHierarchy);
		GameObject.Destroy (laser);
		target.SendMessage ("Damage", self.atk, SendMessageOptions.DontRequireReceiver);
		gameManager.combatManager.DeactivateTargeting ();
		gameManager.playerInput.TogglePlayerInputLock (false);
		gameManager.combatManager.ToggleActionLock (false);
	}
}