using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Assassin : Unit {

	protected override void Awake() {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		storedColor = spriteRenderer.color;
		currentHP = 9;
		maxHP = 9;
		currentAP = 7;
		maxAP = 7;
		atk = 3;
		def = 0;
		objectName = "Assassin";
		special = "High mobility; rapid melee";
		aiAttackRange = 1;
	}

	protected override void Start() {
		base.Start ();
		abilityList.Add (new Slash ());
		abilityList.Add (new ShadowStep (10));
		numCombatActions = 2;
	}

	protected override void ProcessCombatPanelClick (int buttonNum) {
		switch (buttonNum) {
		case 1:
			//Target Attack
			if (abilityList [0].apCost <= currentAP) {
				gameManager.combatManager.ActivateTargeting (abilityList [0]);
			}
			break;
		case 2:
			// Shadowstep to target
			if (abilityList [1].apCost <= currentAP) {
				gameManager.combatManager.ActivateTargeting (abilityList [1]);
			}
			break;
		default:
			break;
		}
	}

	protected override IEnumerator ShittyTestAttack (GameObject target) {
		Slash attack = new Slash ();
		yield return StartCoroutine(attack.Execute (this, target));
	}

	protected  IEnumerator AIShadowStep (GameObject target) {
		ShadowStep attack = new ShadowStep (10);
		yield return StartCoroutine(attack.Execute (this, target));
	}

	public override IEnumerator EnableCombatAI() {
		if (!gameManager.combatManager.actionLock) { //Might have to remove this. Lots of weird coroutine/lock interactions
			gameManager.combatManager.ToggleActionLock(true);
			// Store path to neaest enemy
			StorePathToNearest ();
			List<Vector3> movePath = new List<Vector3> ();
			Vector3 currentLocation = this.transform.position;
			// While AP > 0
			while (currentAP > 0) {
				// Find targets in range
				boxCollider.enabled = false;
				gameManager.combatManager.FindTargets (currentLocation, false, aiAttackRange, false);
				boxCollider.enabled = true;
				Debug.Log ("Finding Targets: " + gameManager.combatManager.targetedObjects.Count);
				// If there are targets in range and current AP >= attack AP cost
				if (gameManager.combatManager.targetedObjects.Count > 0 && currentAP >= 1) {
					if (movePath.Count > 0) {
						gameManager.combatManager.ToggleActionLock (true);
						gameManager.playerInput.TogglePlayerInputLock (true);
						yield return StartCoroutine (SmoothMovement (movePath));
						movePath.Clear ();
					}
					Debug.Log ("Attacking");
					gameManager.combatManager.ToggleActionLock (true);
					gameManager.playerInput.TogglePlayerInputLock (true);
					yield return StartCoroutine (ShittyTestAttack (gameManager.combatManager.targetedObjects [0]));
					gameManager.combatManager.ResetTargets ();
					StorePathToNearest ();
				} else {
					boxCollider.enabled = false;
					gameManager.combatManager.FindTargets (currentLocation, false, 10, false); //Shadowstep Cast Range
					boxCollider.enabled = true;
					List<GameObject> validShadowStepTargets = new List<GameObject> ();
					foreach (GameObject potentialTarget in gameManager.combatManager.targetedObjects) {
						if (Vector2.Distance (transform.position, potentialTarget.transform.position) >= 5) {
							validShadowStepTargets.Add (potentialTarget);
						}
					}

					if (currentAP >= 6 && validShadowStepTargets.Count > 0) {
						if (movePath.Count > 0) {
							gameManager.combatManager.ToggleActionLock (true);
							gameManager.playerInput.TogglePlayerInputLock (true);
							yield return StartCoroutine (SmoothMovement (movePath));
							movePath.Clear ();
						}
						gameManager.combatManager.ToggleActionLock (true);
						gameManager.playerInput.TogglePlayerInputLock (true);
						yield return StartCoroutine (AIShadowStep (validShadowStepTargets [0]));
						StorePathToNearest ();
					} else {
						if (storedPath.Count == 0) {
							Debug.Log ("Cant do anything, lose all AP");
							DeductAP (currentAP);
						} else {
							currentLocation = ExecuteNextAIMove (currentLocation);
							movePath.Add (currentLocation);
							Debug.Log ("Moving to ");
						}
					}
				}
				gameManager.combatManager.ResetTargets ();
			}
			if (movePath.Count > 0) {
				gameManager.combatManager.ToggleActionLock (true);
				gameManager.playerInput.TogglePlayerInputLock(true);
				yield return StartCoroutine (SmoothMovement (movePath));
				movePath.Clear ();
			}
			//Deselect ();
			gameManager.combatManager.ResetTargets ();
			storedPath.Clear ();
			storedPathCost = 666;
			gameManager.combatManager.ToggleActionLock(false);
		}
	}
	//TODO: Remove code duplication

}
