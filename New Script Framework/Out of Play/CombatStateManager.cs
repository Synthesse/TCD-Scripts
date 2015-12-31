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


	void Start () {
		gameManager = GameManager.instance;
	}

	public void StartCombat () {
		gameManager.DeselectObject ();
		combatModeEnabled = true;
		activeEnemies = new List<GameObject>(GameObject.FindGameObjectsWithTag ("Enemy"));
		activeAllies = new List<GameObject>(GameObject.FindGameObjectsWithTag ("Player"));
		gameManager.uiManager.ToggleCombatUI (true);
		getAPPool ();
	}

	void EndCombat () {
		gameManager.uiManager.ToggleCombatUI (false);
		combatModeEnabled = false;
		activeAllies.Clear ();
		activeEnemies.Clear ();
		if (targetingActive)
			DeactivateTargeting ();
	}

	private void getAPPool() {
		currentSideAPPool = 0;
		List<GameObject> iterList;
		if (isPlayerTurn)
			iterList = activeAllies;
		else
			iterList = activeEnemies;
		foreach (GameObject actor in iterList) {
			actor.SendMessage ("AddAPToPool");
		}
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

	private void FindTargets(GameObject source, bool targetEnemy) {
		string targetTag;
		string passthruTag;
		if (targetEnemy) {
			targetTag = "Enemy";
			passthruTag = "Player";
		} else {
			targetTag = "Player";
			passthruTag = "Enemy";
		}

		foreach (GameObject enemy in activeEnemies) {
			RaycastHit2D[] hitArray;
			hitArray = Physics2D.LinecastAll ((Vector2)source.transform.position, (Vector2)enemy.transform.position, layerMask);
			foreach (RaycastHit2D hit in hitArray) {
				if (hit.transform.tag == targetTag && !targetedObjects.Contains(hit.transform.gameObject)) {
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

	private void ResetTargets() {
		foreach (GameObject target in targetedObjects) {
			target.SendMessage ("Untarget");
		}
		targetedObjects.Clear ();
	}

	public void ProcessHitTarget(GameObject hitTarget) {
		gameManager.selectedObject.SendMessage (targetingMethodString, hitTarget);
		DeactivateTargeting ();
	}

	public void StartNextTurn() {
		//Temporary - use to proxy the attack button
		if (gameManager.selectedObject != null && gameManager.selectedObject.tag == "Player") {
			gameManager.selectedObject.SendMessage ("TargetTestAttack");
		}
	}

	// Update is called once per frame
	void Update () {
		if (combatModeEnabled) {
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
