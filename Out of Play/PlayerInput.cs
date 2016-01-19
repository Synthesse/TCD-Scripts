using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Pathfinding;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour {

	public bool gameStarted = true;
	public GameObject selectedObject;
	public bool enableDrawMode = false;
	public Vector3 currentMouseGridLoc;
	public bool playerInputLock = false;
	private int inputLockThreads = 0;

	private GameManager gameManager;
	private int screenWidth;
	private int screenHeight;
	private int layerMask = (1 << 8) | (1 << 9) ;

	void Awake () {
		screenWidth = Screen.width;
		screenHeight = Screen.height;
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
			gameManager.buildManager.RotateBuildGhost();
		});
		gameManager.uiManager.restartGameButton.onClick.AddListener (() => {
			Destroy(GameObject.Find("PlayerPrefs"));
			SceneManager.LoadScene ("Title Screen");
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

		for (int i = 0; i < gameManager.uiManager.buildPanelButtons.Length; i++) {
			int j = i;
			gameManager.uiManager.buildPanelButtons [i].onClick.AddListener (() => {
				gameManager.buildManager.ProcessBuildPanelClick(j);
			});
		}


		gameManager.uiManager.buildToggle.onValueChanged.AddListener ((value) => {
			gameManager.buildManager.ToggleBuildMode(value);
		});
		gameManager.uiManager.detailToggle.onValueChanged.AddListener ((value) => {
			gameManager.uiManager.ToggleUnitDetail(value);
		});
	}

	public Vector3 GetMouseGridPosition() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Vector3 rayPoint = new Vector3 (Mathf.RoundToInt (ray.origin.x), Mathf.RoundToInt (ray.origin.y), 0);
		return rayPoint;
	}

	public void TogglePlayerInputLock(bool state) {
		if (state) {
			inputLockThreads++;
		} else {
			inputLockThreads--;
		}
		if (inputLockThreads > 0 && !playerInputLock) {
			playerInputLock = true;
			gameManager.uiManager.blockInput.SetActive (true);
		} else if (inputLockThreads <= 0 && playerInputLock) {
			playerInputLock = false;
			gameManager.uiManager.blockInput.SetActive (false);
			inputLockThreads = 0;
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("escape"))
			Application.Quit();
		if (!playerInputLock) {
			if (Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject ()) {
				if (gameManager.buildManager.buildModeEnabled && gameManager.buildManager.buildObjectSelected) {
					gameManager.buildManager.BuildObject ();
				} else if (gameManager.combatManager.combatModeEnabled && gameManager.combatManager.targetingActive) {
					// Confirm target selection during targeting
					int layerMask = (1 << 8);
					Collider2D hitCollider = Physics2D.OverlapPoint (GetMouseGridPosition (), layerMask);
					if (hitCollider != null) {
						gameManager.combatManager.ProcessHitTarget (hitCollider.gameObject);
					} 
				} else if (!gameManager.buildManager.buildModeEnabled){
					// Select/Deselect objects

					Collider2D hitCollider = Physics2D.OverlapPoint (GetMouseGridPosition (), layerMask);
					GameObject gameObjectHit = null;
					if (hitCollider != null) {
						gameManager.DeselectObject ();
						gameObjectHit = hitCollider.gameObject;
						gameObjectHit.SendMessage ("Select", SendMessageOptions.DontRequireReceiver);
					} 
					
					if (hitCollider == null || (gameObjectHit != gameManager.selectedObject)) {
						gameManager.DeselectObject ();

					}
				}



			}

			if (Input.GetMouseButtonDown (1) && !EventSystem.current.IsPointerOverGameObject ()) {
				if (gameManager.buildManager.buildModeEnabled && gameManager.buildManager.buildObjectSelected) {
					gameManager.buildManager.ClearBuildObject ();
				} else if (gameManager.combatManager.targetingActive) {
					gameManager.combatManager.DeactivateTargeting ();
				} else if (gameManager.selectedObject != null && gameManager.selectedObject.GetComponent<Unit> () != null && gameManager.selectedObject.GetComponent<Unit> ().currentAP > 0) {
					// Automove if object selected and AP > 0
					StartCoroutine (gameManager.selectedObject.GetComponent<Unit> ().ExecuteMove ());
					//gameManager.selectedObject.SendMessage ("ExecuteMove", SendMessageOptions.DontRequireReceiver);
				} else if (gameManager.selectedObject == null && gameManager.combatManager.combatModeEnabled) {
					Collider2D[] hitColliders = Physics2D.OverlapPointAll (GetMouseGridPosition (), layerMask);
					if (hitColliders.Length == 2) {
						gameManager.DeselectObject ();
						hitColliders[1].gameObject.SendMessage ("Select", SendMessageOptions.DontRequireReceiver);
					}
				} else if (gameManager.selectedObject != null) {
					gameManager.DeselectObject ();
				}
			}

			//PAN
			//if (Input.GetKey ("left") || Input.mousePosition.x <= 0) {
			if (Input.GetKey ("left")) {
				Camera.main.transform.position += new Vector3(Time.deltaTime*-9f,0,0);
			//} else if (Input.GetKey ("right") || Input.mousePosition.x >= screenWidth - 1)
			} else if (Input.GetKey ("right"))
				Camera.main.transform.position += new Vector3(Time.deltaTime*9f,0,0);

			//if (Input.GetKey ("up") || Input.mousePosition.y >= screenHeight-1) {
			if (Input.GetKey ("up")) {
				Camera.main.transform.position += new Vector3(0,Time.deltaTime*9f,0);
			//} else if (Input.GetKey ("down") || Input.mousePosition.y <= 0)
			} else if (Input.GetKey ("down"))
				Camera.main.transform.position += new Vector3(0,Time.deltaTime*-9f,0);

			//ZOOM
			if (Input.GetAxis ("Mouse ScrollWheel") < 0f && Camera.main.orthographicSize < 40f) {
				Camera.main.orthographicSize += 1f;
			} else if (Input.GetAxis ("Mouse ScrollWheel") > 0f && Camera.main.orthographicSize > 1f) {
				Camera.main.orthographicSize -= 1f;
			}


		}

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
