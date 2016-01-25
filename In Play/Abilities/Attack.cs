using UnityEngine;
using System.Collections;

public class Attack : Ability {

	public Attack(int inputRange) {
		apCost = 2;
		cooldown = 0;
		range = inputRange;
		friendlyTarget = false;
		abilityName = "Shoot";
		abilityDescription = "Fires at a single enemy. Costs 2 AP. Deals damage = atk.";
		abilityButtonText = abilityName+"\n"+apCost+" AP, "+range+" range";
		keyPress = "a";
		currentCooldown = 0;
		targetType = abilityTargetingTypes.One;
	}

	public override IEnumerator Execute(Unit self, GameObject target) {
		self.ChangeFacing(gameManager.boardManager.FindDirection(self.transform.position, target.transform.position));
		target.SendMessage ("SetDamageDirection", (Vector2)self.transform.position, SendMessageOptions.DontRequireReceiver);
		gameManager.soundManager.PlayLaserSFX ();
		GameObject laser = GameObject.Instantiate(gameManager.combatManager.laserAttackObj, self.transform.position, Quaternion.Euler(0, 0, Mathf.Rad2Deg*(gameManager.boardManager.FindAngle(self.transform.position, target.transform.position)+Mathf.PI)+90)) as GameObject;
		laser.GetComponent<Mover>().target = target.transform.position;
//		while (laser.activeInHierarchy)
//			yield return null;
		yield return new WaitWhile(() => laser.activeInHierarchy);
		GameObject.Destroy (laser);
		target.SendMessage ("Damage", self.atk, SendMessageOptions.DontRequireReceiver);
		self.ScanPaths ();
		self.DeductAP (apCost);
		gameManager.combatManager.DeactivateTargeting ();
		gameManager.playerInput.TogglePlayerInputLock (false);
		gameManager.combatManager.ToggleActionLock (false);
	}

}
