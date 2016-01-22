using UnityEngine;
using System.Collections;

public class MarkTarget : Ability {

	public MarkTarget(int inputRange) {
		apCost = 3;
		cooldown = 0;
		range = inputRange;
		friendlyTarget = false;
		abilityName = "Mark Target";
		abilityDescription = "Fires at a single enemy. Costs 2 AP. Deals damage = atk.";
		abilityButtonText = abilityName+"\n"+apCost+" AP, "+range+" range\nDebuff enemy";
		keyPress = "a";
		currentCooldown = 0;
		targetType = abilityTargetingTypes.One;
	}

	public override IEnumerator Execute(Unit self, GameObject target) {
		self.ChangeFacing(gameManager.boardManager.FindDirection(self.transform.position, target.transform.position));
		GameObject mark = GameObject.Instantiate(gameManager.combatManager.targetMarkObj, target.transform.position, target.transform.rotation) as GameObject;
		SpriteRenderer markSpriteRenderer = mark.GetComponent<SpriteRenderer> ();
		mark.transform.SetParent (target.transform);
		gameManager.soundManager.PlayTargetVO ();
		float storedAlpha = 1f;//markSpriteRenderer.color.a;
		//markSpriteRenderer.color = new Color (markSpriteRenderer.color.r, markSpriteRenderer.color.g, markSpriteRenderer.color.b, 0f);
		while (markSpriteRenderer.color.a < 1f) {
			markSpriteRenderer.color = new Color(markSpriteRenderer.color.r,markSpriteRenderer.color.g,markSpriteRenderer.color.b,markSpriteRenderer.color.a + storedAlpha / 50f);
			Debug.Log (markSpriteRenderer.color.a);
			yield return null;
		}
		self.DeductAP (apCost);
		gameManager.combatManager.DeactivateTargeting ();
		gameManager.playerInput.TogglePlayerInputLock (false);
		gameManager.combatManager.ToggleActionLock (false);
	}
}
