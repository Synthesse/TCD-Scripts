using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shielder : Unit {

	private direction damageDirection;

	protected override void Awake() {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		storedColor = spriteRenderer.color;
		currentHP = 15;
		maxHP = 15;
		currentAP = 3;
		maxAP = 3;
		atk = 3;
		def = 1;
		objectName = "Captain";
		special = "Directional Shield; Marks Targets";
		aiAttackRange = 3;
		damageDirection = direction.None;
	}

	protected override void Start() {
		base.Start ();
		abilityList.Add (new Attack (3));
		abilityList.Add (new MarkTarget (10));
		numCombatActions = 2;
		ChangeFacing(gameManager.boardManager.FindDirection((Vector2)FindObjectOfType<Elevator>().transform.position, (Vector2)transform.position));
	}

	public void SetDamageDirection(Vector2 loc) {
		damageDirection = gameManager.boardManager.FindDirection ((Vector2)transform.position, loc);
		Debug.Log (damageDirection);
	}

	public override void Damage (int damageTaken) {
		Mark mark = GetComponentInChildren<Mark> ();
		if (mark != null) {
			damageTaken += mark.strength;
		}
		Debug.Log ("taking damage");
		Debug.Log ("Current facing" + currentFacing.ToString());
		if (damageDirection == currentFacing) {
			currentHP -= Mathf.Max (damageTaken - (def + 2), 1);
		} else { 
			currentHP -= Mathf.Max (damageTaken - def, 1);
		}
		UpdateVitalsUIText ();
		if (currentHP <= 0) {
			Kill ();
		}
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
			if (abilityList [1].apCost <= currentAP) {
				gameManager.combatManager.ActivateTargeting (abilityList [1]);
			}
			break;
		default:
			break;
		}
	}


	protected  IEnumerator AIMarkTarget (GameObject target) {
		MarkTarget attack = new MarkTarget (10);
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
					List<GameObject> validMarkTargets = new List<GameObject> ();
					foreach (GameObject potentialTarget in gameManager.combatManager.targetedObjects) {
						if (potentialTarget.GetComponentInChildren<Mark>() == null) {
							validMarkTargets.Add (potentialTarget);
						}
					}

					if (currentAP >= 3 && validMarkTargets.Count > 0) {
						if (movePath.Count > 0) {
							gameManager.combatManager.ToggleActionLock (true);
							gameManager.playerInput.TogglePlayerInputLock (true);
							yield return StartCoroutine (SmoothMovement (movePath));
							movePath.Clear ();
						}
						gameManager.combatManager.ToggleActionLock (true);
						gameManager.playerInput.TogglePlayerInputLock (true);
						yield return StartCoroutine (AIMarkTarget (validMarkTargets [0]));
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
