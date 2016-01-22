using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Robot : Unit {

	public BoxCollider2D collider;
	private bool exploding;

	protected override void Awake() {
		currentHP = 10;
		maxHP = 10;
		currentAP = 8;
		maxAP = 8;
		atk = 15;
		def = 2;
		objectName = "Murderbot";
		special = "Electrocutes; Explodes upon death; Mind Immune";
		aiAttackRange = 1;
		exploding = false;
	}

	protected override void Start() {
		base.Start ();
		abilityList.Add (new Electrocute ());
		collider = GetComponent<BoxCollider2D> ();
	}

	protected override void ProcessCombatPanelClick (int buttonNum) {
		switch (buttonNum) {
		case 1:
			//Target Attack
			gameManager.combatManager.ActivateTargeting (abilityList [0]);
			break;
		case 2:
			break;
		default:
			break;
		}
	}

	protected override IEnumerator ShittyTestAttack (GameObject target) {
		Electrocute attack = new Electrocute ();
		yield return StartCoroutine(attack.Execute (this, target));
	}

	protected IEnumerator DeathExplosion () {
		Explode explosion = new Explode ();
		yield return StartCoroutine (explosion.Execute (this));
	}

	//TODO: REMOVE CODE DUPLICATION
	public override IEnumerator EnableCombatAI() {
		if (!gameManager.combatManager.actionLock) { //Might have to remove this. Lots of weird coroutine/lock interactions
			gameManager.combatManager.ToggleActionLock(true);
//			if (exploding) {
//				
//			} else {
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
					if (gameManager.combatManager.targetedObjects.Count > 0 && currentAP >= 2) {
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
				if (movePath.Count > 0) {
					gameManager.combatManager.ToggleActionLock (true);
					gameManager.playerInput.TogglePlayerInputLock (true);
					yield return StartCoroutine (SmoothMovement (movePath));
					movePath.Clear ();
				}
				//Deselect ();
				gameManager.combatManager.ResetTargets ();
				storedPath.Clear ();
				storedPathCost = 666;
			}
			gameManager.combatManager.ToggleActionLock(false);
		//}
	}

	public override void Damage (int damageTaken) {
		Mark mark = GetComponentInChildren<Mark> ();
		if (mark != null) {
			damageTaken += mark.strength;
		}
		currentHP -= Mathf.Max(damageTaken - def, 1);
		UpdateVitalsUIText ();
		if (currentHP <= 0) {
//			exploding = true;
//			spriteRenderer.color = new Color (1f,0.6f,0.2f,1f);
//			attackable = false;
			gameManager.combatManager.ToggleActionLock (true);
			gameManager.playerInput.TogglePlayerInputLock (true);
			StartCoroutine (DeathExplosion());
		}
	}

}
