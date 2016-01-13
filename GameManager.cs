using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public BoardManager boardManager;
	public PlayerInput playerInput;
	public UIManager uiManager;
	public BuildStateManager buildManager;
	public CombatStateManager combatManager;
	public bool buildMode = false;
	public long cash = 0;
	public int waveNumber = 0;
	public GameObject selectedObject;
	public bool targetingActive = false;


	// Use this for initialization
	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		//DontDestroyOnLoad (gameObject);
		uiManager = GetComponent<UIManager> ();
		boardManager = GetComponent<BoardManager> ();
		playerInput = GetComponent<PlayerInput> ();
		combatManager = GetComponent<CombatStateManager> ();
		buildManager = GetComponent<BuildStateManager> ();
		uiManager.Initialize ();
		playerInput.Initialize ();

		InitGame ();

	}

	void InitGame() {
		boardManager.SetupScene ();
		selectedObject = null;
	}

	public void DeselectObject() {
		if (selectedObject != null)
			selectedObject.SendMessage ("Deselect");
	}

	public void GameOver() {
		uiManager.backdrop.SetActive (true);
		uiManager.gameOverText.text = "Game Over\nReached Wave " + waveNumber.ToString ();
		uiManager.gameOverText.enabled = true;
		uiManager.restartGameButton.gameObject.SetActive (true);
	}

	//Unused, until we have the ability to path through allies
	public void ToggleColliders(string objectType, bool turnOn) {
		List<GameObject> objectsToToggle;

		if (objectType == "enemy") {
			objectsToToggle = new List<GameObject>(GameObject.FindGameObjectsWithTag ("Enemy"));
		} else if (objectType == "ally") {
			objectsToToggle = new List<GameObject>(GameObject.FindGameObjectsWithTag ("Ally"));
		} else
			return;
		
		foreach (GameObject toToggle in objectsToToggle) {
			toToggle.GetComponent<BoxCollider2D>().enabled = turnOn;
		}
	}

}
