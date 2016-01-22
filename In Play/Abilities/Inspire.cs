using UnityEngine;
using System.Collections;

public class Inspire : Ability {

	public Inspire() {
		apCost = 4;
		cooldown = 0;
		range = 666;
		friendlyTarget = true;
		abilityName = "Haste";
		abilityDescription = "Grants an ally 2 AP";
		abilityButtonText = abilityName+"\n"+apCost+" AP\n+2AP to Ally";
		keyPress = "a";
		currentCooldown = 0;
		targetType = abilityTargetingTypes.One;
	}

	public override IEnumerator Execute(Unit self, GameObject target) {
		
		self.ChangeFacing(gameManager.boardManager.FindDirection(self.transform.position, target.transform.position));
		target.SendMessage ("DeductAP", -2, SendMessageOptions.DontRequireReceiver);
		gameManager.soundManager.PlayHasteSFX ();
		GameObject hasteObj = GameObject.Instantiate (gameManager.combatManager.magicEffectObj, target.transform.position, target.transform.rotation) as GameObject;
		yield return new WaitForSeconds (1.75f);
		GameObject.Destroy (hasteObj);
		self.DeductAP (apCost);
		gameManager.combatManager.DeactivateTargeting ();
		gameManager.playerInput.TogglePlayerInputLock (false);
		gameManager.combatManager.ToggleActionLock (false);
	}
}
