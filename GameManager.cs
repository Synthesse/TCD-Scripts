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
	public TutorialManager tutorialManager;
	public SoundManager soundManager;
	public bool buildMode = false;
	public long cash = 0;
	public int income = 0;
	private int[] incomeRef;
	public int waveNumber = 0;
	public GameObject selectedObject;
	public bool targetingActive = false;
	public PlayerPrefs playerPrefs;
	public bool hardModeEnabled;
	//public int tutorialStage;


	// Use this for initialization
	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

//		GameObject playerPrefsGO = new GameObject ("PlayerPrefs");
//		playerPrefsGO.AddComponent<PlayerPrefs> ();
//		playerPrefs = playerPrefsGO.GetComponent<PlayerPrefs> ();
//		playerPrefs.leaderName = "Nayhomie";
//		playerPrefs.tutorialOn = false;
//		playerPrefs.customizeBaseOnStart = false;
		hardModeEnabled = false;

		//DontDestroyOnLoad (gameObject);
		uiManager = GetComponent<UIManager> ();
		boardManager = GetComponent<BoardManager> ();
		playerInput = GetComponent<PlayerInput> ();
		combatManager = GetComponent<CombatStateManager> ();
		buildManager = GetComponent<BuildStateManager> ();
		soundManager = FindObjectOfType<SoundManager> ();
		playerPrefs = FindObjectOfType<PlayerPrefs> ();
		tutorialManager = GetComponent<TutorialManager> ();
		uiManager.Initialize ();
		playerInput.Initialize ();
		InitGame ();

	}

	void InitGame() {
		boardManager.SetupScene (!playerPrefs.customizeBaseOnStart);
		FindObjectOfType<Leader> ().objectName = playerPrefs.leaderName;
		if (playerPrefs.leaderName == "Goose") {
			uiManager.dapperGoose.SetActive (true);
			cash += 111111;
		}
		selectedObject = null;
		cash += 75;
		if (playerPrefs.customizeBaseOnStart) {
			cash += 40;
		}
		incomeRef = new int[5] { 10, 15, 19, 21, 22 };
		uiManager.UpdateCashText ();
		if (playerPrefs.tutorialOn) {
			tutorialManager.Initialize ();
		} else {
			tutorialManager.enabled = false;
		}
		RefocusCamera ();
	}

	public void Income() {
		int incomeIndex = Mathf.Min (income, 4);
		if (hardModeEnabled)
			cash += Mathf.FloorToInt((incomeRef [incomeIndex] + income)/5.0f);
		else
			cash += Mathf.FloorToInt((incomeRef [incomeIndex] + income*2));
		uiManager.UpdateCashText ();
	}

	void Start() {
		AstarPath.active.Scan ();
	}

	public void RefocusCamera() {
		Vector3 loc = FindObjectOfType<Leader> ().transform.position;
		Camera.main.transform.position = new Vector3 (loc.x, loc.y, -20);
	}

	public void DeselectObject() {
		if (selectedObject != null)
			selectedObject.SendMessage ("Deselect");
	}

	public void WinGame() {
		soundManager.MuteAudio ();
		playerInput.TogglePlayerInputLock (true);
		uiManager.backdrop.SetActive (true);
		uiManager.gameOverText.text = "Game Over, You Win!\nKeep Playing in Hard Mode?";
		uiManager.gameOverText.enabled = true;
		uiManager.gameOverNoteText.enabled = true;
		uiManager.hardModeButton.gameObject.SetActive (true);
		uiManager.restartGameButton.gameObject.SetActive (true);
	}

	public void EnableHardMode() {
		hardModeEnabled = true;

		playerInput.TogglePlayerInputLock (false);
		soundManager.MuteAudio ();
		uiManager.backdrop.SetActive (false);
		uiManager.gameOverText.enabled = false;
		uiManager.gameOverNoteText.enabled = false;
		uiManager.hardModeButton.gameObject.SetActive (false);
		uiManager.restartGameButton.gameObject.SetActive (false);
	}

	public void GameOver() {
		StopAllCoroutines ();
		soundManager.MuteAudio ();
		playerInput.TogglePlayerInputLock (true);
		uiManager.backdrop.SetActive (true);
		uiManager.gameOverText.text = "Game Over\nReached Wave " + waveNumber.ToString ();
		uiManager.gameOverText.enabled = true;
		uiManager.gameOverNoteText.enabled = true;
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
