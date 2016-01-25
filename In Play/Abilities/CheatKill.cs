using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheatKill : Ability {

	public CheatKill() {
		apCost = 0;
		cooldown = 0;
		range = 666;
		friendlyTarget = false;
		abilityName = "CheatKill";
		abilityDescription = "Fires at a single enemy. Costs 2 AP. Deals damage = atk.";
		abilityButtonText = "Debug Only\nKills all enemies";
		keyPress = "a";
		currentCooldown = 0;
		targetType = abilityTargetingTypes.One;
	}

	public override IEnumerator Execute(Unit self) {
		List<GameObject> activeEnemies = new List<GameObject> ();
		activeEnemies.AddRange (gameManager.combatManager.activeEnemies);
		foreach (GameObject enemy in activeEnemies) {
			enemy.GetComponent<Unit> ().Kill ();
		}
		yield return null;
		self.ScanPaths ();
		self.DeductAP (apCost);
		gameManager.combatManager.DeactivateTargeting ();
		gameManager.playerInput.TogglePlayerInputLock (false);
		gameManager.combatManager.ToggleActionLock (false);
	}
}
