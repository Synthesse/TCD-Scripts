using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShadowStep : Ability {

	public ShadowStep(int inputRange) {
		apCost = 6;
		cooldown = 0;
		range = inputRange;
		friendlyTarget = false;
		abilityName = "Shadow Step";
		abilityDescription = "Fires at a single enemy. Costs 2 AP. Deals damage = atk.";
		abilityButtonText = abilityName+"\n"+apCost+" AP, "+range+" range\nMove to enemy";
		keyPress = "a";
		currentCooldown = 0;
		targetType = abilityTargetingTypes.One;
	}

	public override IEnumerator Execute(Unit self, GameObject target) {
		List<Vector2> openSpaces = new List<Vector2>();
		openSpaces.AddRange(target.GetComponent<PhysicalObject> ().NearestOpenSpaces (self.transform.position));
		if (openSpaces.Count > 0) {
			self.ChangeFacing (gameManager.boardManager.FindDirection (self.transform.position, target.transform.position));
			SpriteRenderer selfSpriteRenderer = self.GetComponent<SpriteRenderer> ();
			float storedAlpha = selfSpriteRenderer.color.a;
			while (selfSpriteRenderer.color.a > Mathf.Epsilon) {
				selfSpriteRenderer.color = new Color(selfSpriteRenderer.color.r,selfSpriteRenderer.color.g,selfSpriteRenderer.color.b,selfSpriteRenderer.color.a - storedAlpha / 70f);
				yield return null;
			}
			gameManager.soundManager.PlayWhooshSFX ();
			self.transform.position = openSpaces [0];
			while (selfSpriteRenderer.color.a < storedAlpha) {
				selfSpriteRenderer.color = new Color(selfSpriteRenderer.color.r,selfSpriteRenderer.color.g,selfSpriteRenderer.color.b,selfSpriteRenderer.color.a + storedAlpha / 70f);
				yield return null;
			}
			self.DeductAP (apCost);
		}
		gameManager.combatManager.DeactivateTargeting ();
		gameManager.playerInput.TogglePlayerInputLock (false);
		gameManager.combatManager.ToggleActionLock (false);
	}
}
