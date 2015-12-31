using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class GameManager : MonoBehaviour {

	public float turnDelay = 0.1f;
	public static GameManager instance = null;
	public BoardManager boardManager;
	public PlayerInput playerInput;
	public UIManager uiManager;
	public BuildStateManager buildManager;
	public CombatStateManager combatManager;
	public bool combatMode = false;
	public bool buildMode = false;
	public bool gamePaused = false;
	public bool playersTurn = true;
	public long cash = 0;
	public int waveNumber = 0;
	public GameObject selectedObject;
	public Player selectedUnit;
	public bool targetingActive = false;


	private List<Enemy> enemyList;
	private bool enemiesMoving;


	// Use this for initialization
	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);
		enemyList = new List<Enemy> ();
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
		selectedUnit = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
		selectedObject = null;
	}

	public void DeselectObject() {
		if (selectedObject != null)
			selectedObject.SendMessage ("Deselect");
	}

	public void GameOver() {
		enabled = false;
	}

	// Update is called once per frame
	void Update () {
		if (playersTurn || enemiesMoving)
			return;

		StartCoroutine (MoveEnemies ());
	}

	public void AddEnemyToList (Enemy script) {
		enemyList.Add (script);
	}

	IEnumerator MoveEnemies() {
		enemiesMoving = true;
		yield return new WaitForSeconds (turnDelay);
		if (enemyList.Count == 0) {
			yield return new WaitForSeconds (turnDelay);
		}

		for (int i = 0; i < enemyList.Count; i++) {
			enemyList [i].MoveEnemy ();
			yield return new WaitForSeconds (enemyList[i].moveTime); 
		}

		playersTurn = true;
		Player.instance.turnStatusText.text = "Player Turn";
		enemiesMoving = false;
	}

	public void ToggleColliders(string objectType, bool turnOn) {
		List<GameObject> objectsToToggle;

		if (objectType == "enemy") {
			objectsToToggle = new List<GameObject>(GameObject.FindGameObjectsWithTag ("Enemy"));
		} else if (objectType == "ally") {
			objectsToToggle = new List<GameObject>(GameObject.FindGameObjectsWithTag ("Ally"));
			objectsToToggle.Add (GameObject.FindGameObjectWithTag ("Player"));
		} else
			return;
		
		foreach (GameObject toToggle in objectsToToggle) {
			toToggle.GetComponent<BoxCollider2D>().enabled = turnOn;
		}
	}
}
