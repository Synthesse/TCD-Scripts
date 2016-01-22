using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatStateManager : MonoBehaviour {

	public GameManager gameManager;
	public bool combatModeEnabled = false;
	public bool isPlayerTurn = true;
	public bool targetingActive = false;
	public int currentSideAPPool;
	public Ability targetingAbility = null;
	public List<GameObject> activeEnemies;
	public List<GameObject> activeAllies;
	public List<GameObject> targetedObjects;
	private int layerMask = (1 << 8) | (1 << 9) ;
	public bool actionLock;
	public int actionLockThreads;

	public GameObject laserAttackObj;
	public GameObject mindControlObj;
	public GameObject magicEffectObj;
	public GameObject explosionObj;
	public GameObject slashAttackObj;
	public GameObject electrocuteAttackObj;
	public GameObject targetMarkObj;

	void Start () {
		gameManager = GameManager.instance;
	}

	public void StartCombat () {
		gameManager.DeselectObject ();
		gameManager.soundManager.SetCombatMusic ();
		isPlayerTurn = true;
		combatModeEnabled = true;
		gameManager.waveNumber++;
		gameManager.uiManager.UpdateWaveNumberText ();
		activeEnemies = CreateWave();
		activeAllies = new List<GameObject>(GameObject.FindGameObjectsWithTag ("Ally"));
		activeAllies.AddRange (new List<GameObject> (GameObject.FindGameObjectsWithTag ("Ally NoTarget")));
		gameManager.uiManager.ToggleCombatUI (true);
		GetAPPool ();
	}

	void EndCombat () {
		RefreshSideAP ();
		Mark[] markArray = FindObjectsOfType<Mark> ();
		foreach (Mark mark in markArray) {
			Destroy (mark.gameObject);
		}
		gameManager.uiManager.ToggleCombatUI (false);
		combatModeEnabled = false;
		activeAllies.Clear ();
		activeEnemies.Clear ();
		if (targetingActive)
			DeactivateTargeting ();
		gameManager.soundManager.SetNonCombatMusic ();
	}
		
	private void UpgradeWave (List<GameObject> waveList, int numUpgrades) {
		for (int i = 0; i < numUpgrades; i++) {
			int unitChoice = Random.Range (0, waveList.Count);
			int statChoice = Random.Range (1, 5);
			switch (statChoice) {
			case 1:
				waveList [unitChoice].GetComponent<Unit> ().atk++;
				break;
			case 2:
				waveList [unitChoice].GetComponent<Unit> ().def++;
				break;
			case 3:
				waveList [unitChoice].GetComponent<Unit> ().currentAP++;
				waveList [unitChoice].GetComponent<Unit> ().maxAP++;
				break;
			case 4:
				waveList [unitChoice].GetComponent<Unit> ().currentHP++;
				waveList [unitChoice].GetComponent<Unit> ().maxHP++;
				break;
			}
		}
	}

	private List<GameObject> CreateWave () {
		int numEnemies = Mathf.RoundToInt (Mathf.Pow (gameManager.waveNumber, 0.67f));
		int numUpgrades = Mathf.RoundToInt (gameManager.waveNumber / 1.5f);
		gameManager.boardManager.DefaultWaveSpawn (numEnemies);
		List<GameObject> waveList = new List<GameObject>(GameObject.FindGameObjectsWithTag ("Enemy"));
		UpgradeWave (waveList, numUpgrades);
		return waveList;
	}

	private void GetAPPool() {
		currentSideAPPool = 0;
		List<GameObject> iterList = GetActors (true);
		foreach (GameObject actor in iterList) {
			if (actor.GetComponent<Unit> () != null)
				actor.GetComponent<Unit> ().AddAPToPool ();
			else if (actor.GetComponent<Defenses> () != null)
				actor.GetComponent<Defenses> ().AddAPToPool ();
		}
	}

	private void RefreshSideAP() {
		List<GameObject> iterList = GetActors (true);
		foreach (GameObject actor in iterList) {
			if (actor.GetComponent<Unit> () != null)
				actor.GetComponent<Unit> ().ResetAP ();
			else if (actor.GetComponent<Defenses> () != null)
				actor.GetComponent<Defenses> ().ResetAP ();
		}
	}

	public List<GameObject> GetActors(bool getSameSide) {
		List<GameObject> iterList;
		if ((isPlayerTurn && getSameSide) || (!isPlayerTurn && !getSameSide))
			iterList = activeAllies;
		else
			iterList = activeEnemies;
		return iterList;
	}

	public void ActivateTargeting(Ability ability) {
		BoxCollider2D bc2d = gameManager.selectedObject.GetComponent<BoxCollider2D> ();
		if (bc2d != null)
			bc2d.enabled = false;
		
		if (ability.range > 0)
			//TODO: Remove mind control direct reference
			FindTargets ((Vector2)gameManager.selectedObject.transform.position, !ability.friendlyTarget, ability.range, ability.abilityName == "Mind Control");
		else 
			FindTargets ((Vector2)gameManager.selectedObject.transform.position, !ability.friendlyTarget);

		if (bc2d != null)
			bc2d.enabled = true;
		targetingActive = true;
		gameManager.uiManager.ToggleCombatPanelButtons ();
		targetingAbility = ability;
		gameManager.uiManager.UnrenderPathLine ();
	}

	public void DeactivateTargeting() {
		ResetTargets ();
		targetingActive = false;
		gameManager.uiManager.ToggleCombatPanelButtons ();
		targetingAbility = null;
	}

	public void FindTargets(Vector2 sourcePosition, bool targetEnemy, int range, bool requireBloodied) {
		string targetTag;
		string passthruTag;

		string alternateTargetTag = "";
		List<GameObject> potentialTargets;
		if (targetEnemy) {
			targetTag = "Enemy";
			passthruTag = "Ally";
			potentialTargets = activeEnemies;
		} else {
			targetTag = "Ally";
			passthruTag = "Enemy";
			potentialTargets = activeAllies.GetRange(0,activeAllies.Count);
			if (!isPlayerTurn) {
				//TODO: Make less shitty
				potentialTargets.AddRange (new List<GameObject> (GameObject.FindGameObjectsWithTag ("Targetable Machine")));
				alternateTargetTag = "Targetable Machine";
			}
		}
		//Debug.Log ("My position: " + sourcePosition.ToString ());
		//Debug.Log ("My range: " + range.ToString ());
		foreach (GameObject potentialTarget in potentialTargets) {
			//Debug.Log ("Identified potential target");
			//Debug.Log ("Their position: " + potentialTarget.transform.position.ToString ());
			//Debug.Log ("Distance: " + Vector2.Distance (sourcePosition, (Vector2)potentialTarget.transform.position).ToString ());

			//TODO: Change this to be less shitty (use interfaces)
			if (!potentialTarget.GetComponent<SelectableObject> ().attackable) {
				//Debug.Log ("Not attackable!");
				break;
			}
			if (requireBloodied && ((potentialTarget.GetComponent<Unit>() != null && potentialTarget.GetComponent<Unit>().currentHP >= potentialTarget.GetComponent<Unit>().maxHP/2.0f) || potentialTarget.GetComponent<Robot>() != null)) {
				//Debug.Log ("Immune to MC!");
				break;
			}
			if (Vector2.Distance (sourcePosition, new Vector2(Mathf.FloorToInt(potentialTarget.transform.position.x),Mathf.FloorToInt(potentialTarget.transform.position.y))) <= (range + 0.5f)) {
				//Debug.Log ("Start raycast");
				RaycastHit2D[] hitArray;
				hitArray = Physics2D.LinecastAll (sourcePosition, (Vector2)potentialTarget.transform.position, layerMask);
				foreach (RaycastHit2D hit in hitArray) {
					//Debug.Log ("Raycast hit");
					if ((hit.transform.tag == targetTag || hit.transform.tag == alternateTargetTag) && !targetedObjects.Contains (hit.transform.gameObject)) {
						if (isPlayerTurn)
							hit.transform.gameObject.SendMessage ("Target");
						targetedObjects.Add (hit.transform.gameObject);
						break;
					} else if (hit.transform.tag == passthruTag) {
						//Do nothing
					} else {
						break;	
					}
				}
			}
		}
	}

	public void FindTargets(Vector2 sourcePosition, bool targetEnemy) {
		FindTargets (sourcePosition, targetEnemy, 666, false);
	}

	public void ResetTargets() {
		if (isPlayerTurn) {
			foreach (GameObject target in targetedObjects) {
				target.SendMessage ("Untarget");
			}
		}
		targetedObjects.Clear ();
	}

	public void ProcessHitTarget(GameObject hitTarget) {
		if (targetedObjects.Contains (hitTarget)) {
			gameManager.playerInput.TogglePlayerInputLock (true);
			ToggleActionLock (true);
			if (gameManager.selectedObject.GetComponent<Unit> () != null)
				StartCoroutine (targetingAbility.Execute (gameManager.selectedObject.GetComponent<Unit> (), hitTarget));
			else if (gameManager.selectedObject.GetComponent<Defenses> () != null)
				StartCoroutine (targetingAbility.Execute (gameManager.selectedObject.GetComponent<Defenses> (), hitTarget));
		}
	}


	public void ToggleActionLock(bool state) {
		if (state) {
			actionLockThreads++;
		} else {
			actionLockThreads--;
		}
		if (actionLockThreads > 0 && !actionLock) {
			actionLock = true;
		} else if (actionLockThreads <= 0 && actionLock) {
			actionLock = false;
			//actionLockThreads = 0;
		}
	}

	private IEnumerator StartAICoroutines() {
		gameManager.playerInput.TogglePlayerInputLock (true);
		foreach (GameObject enemyGO in activeEnemies) {
			yield return StartCoroutine (enemyGO.GetComponent<Unit> ().EnableCombatAI());
		}
		gameManager.playerInput.TogglePlayerInputLock (false);
	}

	public void StartNextTurn() {
		//Temporary - use to proxy the attack button
		RefreshSideAP ();
		gameManager.DeselectObject();
		isPlayerTurn = (isPlayerTurn == true) ? false : true;
		gameManager.uiManager.ToggleTurnText (isPlayerTurn);
		GetAPPool ();
		if (!isPlayerTurn) {
			StartCoroutine (StartAICoroutines ());
		} else {
			gameManager.soundManager.CheckRaisedTension ();
		}
	}

	// Update is called once per frame
	void Update () {
		if (combatModeEnabled && !actionLock) {
			if (activeEnemies.Count == 0) {
				EndCombat ();
			} else if (activeAllies.Count == 0) {
				gameManager.GameOver ();
			} else if (currentSideAPPool <= 0) {
				StartNextTurn ();
			}
		}
	}
}
