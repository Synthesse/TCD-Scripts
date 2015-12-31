using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Pathfinding;

public class PlayerInput : MonoBehaviour {

	public bool gameStarted = true;
	public GameObject selectedObject;
	public bool enableDrawMode = false;
	public Vector3 currentMouseGridLoc;

	private GameManager gameManager;

	void Awake () {
	}

	// Use this for initialization
	public void Initialize () {
		// TODO: Add listeners for pre-game UI elements

		gameManager = GameManager.instance; 

		gameManager.uiManager.startWaveButton.onClick.AddListener (() => {
			gameManager.combatManager.StartCombat();
		});
		gameManager.uiManager.nextTurnButton.onClick.AddListener (() => {
			gameManager.combatManager.StartNextTurn();
		});
		gameManager.uiManager.switchBuildMenusButton.onClick.AddListener (() => {
			Debug.Log("Switch Build Menus!"); 
		});

		gameManager.uiManager.combatPanelButtons [0].onClick.AddListener (() => {
			gameManager.combatManager.DeactivateTargeting();
		});

		for (int i = 1; i < gameManager.uiManager.combatPanelButtons.Length; i++) {
			int j = i;
			gameManager.uiManager.combatPanelButtons [i].onClick.AddListener (() => {
				gameManager.selectedObject.SendMessage("ProcessCombatPanelClick", j);
			});
		}


		gameManager.uiManager.buildToggle.onValueChanged.AddListener ((value) => {
			ToggleBuildMode(value);
		});
		gameManager.uiManager.detailToggle.onValueChanged.AddListener ((value) => {
			gameManager.uiManager.ToggleUnitDetail(value);
		});
	}

	void ToggleBuildMode(bool turnOn) {
		if (turnOn) {
			Debug.Log ("Enable Build Mode!");
			gameManager.uiManager.ToggleBuildUI (true);
			gameManager.buildMode = true;
			gameManager.DeselectObject ();
		} else {
			Debug.Log ("Disable Build Mode!");
			gameManager.uiManager.ToggleBuildUI (false);
			gameManager.buildMode = false;
		}
	}

	public Vector3 GetMouseGridPosition() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Vector3 rayPoint = new Vector3 (Mathf.RoundToInt (ray.origin.x), Mathf.RoundToInt (ray.origin.y), 0);
		return rayPoint;
	}



	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("escape"))
			Application.Quit();

		if (Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject ()) {
			if (gameManager.buildMode) {
				//Place build object via BuildManager
				GameObject tileChoice = gameManager.boardManager.wallTiles [Random.Range (0, gameManager.boardManager.wallTiles.Length)];
				GameObject instance = Instantiate (tileChoice, GetMouseGridPosition(), Quaternion.identity) as GameObject;
				instance.transform.SetParent (GameManager.instance.boardManager.wallHolder);
			} else if (gameManager.combatManager.combatModeEnabled && gameManager.combatManager.targetingActive) {
				// Confirm target selection during targeting
				int layerMask = (1 << 8);
				Collider2D hitCollider = Physics2D.OverlapPoint (GetMouseGridPosition(), layerMask);
				if (hitCollider != null) {
					gameManager.combatManager.ProcessHitTarget (hitCollider.gameObject);
				} 
			} else {
				// Select/Deselect objects
				int layerMask = (1 << 8) | (1 << 9);
				Collider2D hitCollider = Physics2D.OverlapPoint (GetMouseGridPosition(), layerMask);
				GameObject gameObjectHit = null;
				if (hitCollider != null) {
					gameManager.DeselectObject ();
					gameObjectHit = hitCollider.gameObject;
					gameObjectHit.SendMessage ("Select", SendMessageOptions.DontRequireReceiver);
				} 


				if (hitCollider == null || (gameObjectHit != gameManager.selectedObject)) {
					gameManager.DeselectObject ();

				}

				// Select object
			}



		}

		if (Input.GetMouseButtonDown (1) && !EventSystem.current.IsPointerOverGameObject ()) {
			if (gameManager.buildMode) {
				// Rotate object in build mode
			} else if (gameManager.combatManager.targetingActive) {
				Debug.Log ("Test");
				gameManager.combatManager.DeactivateTargeting ();
			} else if (gameManager.selectedObject != null) {
				gameManager.selectedObject.SendMessage ("ExecuteMove", SendMessageOptions.DontRequireReceiver);
				// Automove if object selected and AP > 0
			}

		}

		//ADD SCROLL

		//ADD CAMERA PAN

		/*if (Input.GetKeyDown ("d")) {
			if (enableDrawMode)
				enableDrawMode = false;
			else
				enableDrawMode = true;
		}

		if (enableDrawMode) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3 rayPoint = new Vector3 (Mathf.RoundToInt (ray.origin.x), Mathf.RoundToInt (ray.origin.y), 0);
			if (currentMouseGridLoc != rayPoint) {
				currentMouseGridLoc = rayPoint;
				Path path = gameManager.selectedUnit.seeker.StartPath (new Vector3 (gameManager.selectedUnit.transform.position.x, gameManager.selectedUnit.transform.position.y, 0), currentMouseGridLoc);
				AstarPath.WaitForPath (path);
				if (path != null && ((Vector2)path.vectorPath [path.vectorPath.Count - 1] == (Vector2)rayPoint))
					gameManager.uiManager.renderPathLine (path.vectorPath);
			}
		}
		*/

	}

	/* PSEUDOCODE
	 * 
	 * First priority - look for escape key
	 * Have a flag to collect mouse position. Collect on every update cycle if flag is true
	 * MAKE SURE SCRIPT ORDER EXECUTION HAS THIS SCRIPT IN FRONT OF ANY SCRIPT RELYING ON UPDATE CYCLE MOUSE POSITION
	 * Have a method to collect raycast from mouse clicks
	 * For other critical keys (build mode, pause, etc), trigger relevant methods
	*/


}
