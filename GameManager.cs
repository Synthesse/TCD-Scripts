using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class GameManager : MonoBehaviour {

	public float turnDelay = 0.1f;
	public static GameManager instance = null;
	public BoardManager boardScript;
	public bool combatMode = false;
	public bool buildMode = false;
	public bool gamePaused = false;
	[HideInInspector] public bool playersTurn = true;


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
		boardScript = GetComponent<BoardManager> ();

		InitGame ();

	}

	void InitGame() {
		
		boardScript.SetupScene ();
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
