using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatStateManager : MonoBehaviour {

	public GameManager gameManager;
	public bool combatModeEnabled = false;
	public bool isPlayerTurn = true;
	public bool targetingActive = false;
	public int currentSideAPPool;
	public string targetingMethodString = null;
	public List<GameObject> activeEnemies;
	public List<GameObject> activeAllies;
	public List<GameObject> targetedObjects;
	private int layerMask = 1 << 8;
	public bool actionLock;


	void Start () {
		gameManager = GameManager.instance;
	}

	public void StartCombat () {
		gameManager.DeselectObject ();
		isPlayerTurn = true;
		combatModeEnabled = true;
		gameManager.waveNumber++;
		gameManager.uiManager.UpdateWaveNumberText ();
		activeEnemies = CreateWave();
		activeAllies = new List<GameObject>(GameObject.FindGameObjectsWithTag ("Ally"));
		Debug.Log ("Active allies" + activeAllies.Count.ToString ());
		gameManager.uiManager.ToggleCombatUI (true);
		GetAPPool ();
	}

	void EndCombat () {
		RefreshSideAP ();
		gameManager.uiManager.ToggleCombatUI (false);
		combatModeEnabled = false;
		activeAllies.Clear ();
		activeEnemies.Clear ();
		if (targetingActive)
			DeactivateTargeting ();
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
		Debug.Log ("Getting AP Pool");
		List<GameObject> iterList = GetActors (true);
		foreach (GameObject actor in iterList) {
			actor.GetComponent<Unit>().AddAPToPool();
		}
		Debug.Log (currentSideAPPool);
	}

	private void RefreshSideAP() {
		List<GameObject> iterList = GetActors (true);
		foreach (GameObject actor in iterList) {
			actor.GetComponent<Unit>().ResetAP();
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

	public void ActivateTargeting(string methodString) {
		FindTargets (gameManager.selectedObject, true);
		targetingActive = true;
		gameManager.uiManager.ToggleCombatPanelButtons ();
		targetingMethodString = methodString;
		gameManager.uiManager.UnrenderPathLine ();
	}

	public void DeactivateTargeting() {
		ResetTargets ();
		targetingActive = false;
		gameManager.uiManager.ToggleCombatPanelButtons ();
		targetingMethodString = null;
	}

	public void FindTargets(GameObject source, bool targetEnemy) {
		string targetTag;
		string passthruTag;
		List<GameObject> potentialTargets;
		if (targetEnemy) {
			targetTag = "Enemy";
			passthruTag = "Ally";
			potentialTargets = activeEnemies;
		} else {
			targetTag = "Ally";
			passthruTag = "Enemy";
			potentialTargets = activeAllies;
		}

		foreach (GameObject potentialTarget in potentialTargets) {
			RaycastHit2D[] hitArray;
			hitArray = Physics2D.LinecastAll ((Vector2)source.transform.position, (Vector2)potentialTarget.transform.position, layerMask);
			Debug.Log ("Number of hits: " + hitArray.Length);
			foreach (RaycastHit2D hit in hitArray) {
				Debug.Log ("Test Target Tag: " + (hit.transform.tag == targetTag));
				Debug.Log (hit.transform.tag);
				Debug.Log (targetTag);
				Debug.Log ("Test Already in Array: " + !targetedObjects.Contains(hit.transform.gameObject));
				if (hit.transform.tag == targetTag && !targetedObjects.Contains(hit.transform.gameObject)) {
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
			gameManager.selectedObject.SendMessage (targetingMethodString, hitTarget);
			DeactivateTargeting ();
		}
	}

	public void StartNextTurn() {
		//Temporary - use to proxy the attack button
		RefreshSideAP ();
		gameManager.DeselectObject();
		isPlayerTurn = (isPlayerTurn == true) ? false : true;
		GetAPPool ();
		if (!isPlayerTurn) {
			gameManager.playerInput.enabled = false;
			foreach (GameObject enemyGO in activeEnemies) {
				enemyGO.GetComponent<Unit> ().EnableCombatAI ();
			}
			gameManager.playerInput.enabled = true;
		}
	}

	// Update is called once per frame
	void Update () {
		if (combatModeEnabled && !actionLock) {
			if (activeEnemies.Count == 0) {
				EndCombat ();
			} else if (activeAllies.Count == 0) {
				gameManager.GameOver ();
			} else if (currentSideAPPool == 0) {
				StartNextTurn ();
			}
		}
	}
}
