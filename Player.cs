using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Pathfinding;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Player : MovingObject {

	public int wallDamage = 5;
	public int damage = 5;
	public static Player instance = null;
	public int playerScore = 0;
	public Text scoreText; 
	public Text turnStatusText;
	public Text buildModeText;

	public bool isAlly = true;
	private Path storedPath = null;
	//public GameObject[] wallTiles;

	public Seeker seeker;
	private Path path;
	private int storedPathCost = 666;
	public bool buttonMouseOver = false;


	protected override void Start () {
		AstarPath.active.Scan ();
	
		turnStatusText = GameObject.Find("turnStatusText").GetComponent<Text>();
		turnStatusText.text = "Player Turn";
		seeker = GetComponent<Seeker> ();
		//testButton.onClick.AddListener (() => { DamagePlayer(1); Debug.Log ("HAH");
		//});

		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		base.Start ();

		GameManager.instance.ToggleColliders ("ally", false);
		AstarPath.active.Scan ();
		GameManager.instance.ToggleColliders ("ally", true);

		unitName = "Valerie";
	}

	// Update is called once per frame
	void Update () { 
		if (isSelected && isAlly && currentAP > 0 && !GameManager.instance.combatManager.targetingActive) {
			Vector3 mousePoint = GameManager.instance.playerInput.GetMouseGridPosition ();
			if (mousePoint != GameManager.instance.playerInput.currentMouseGridLoc) {
				GameManager.instance.playerInput.currentMouseGridLoc = mousePoint;
				Path path = seeker.StartPath (new Vector3 (transform.position.x, transform.position.y, 0), mousePoint);
				AstarPath.WaitForPath (path);

				if (ValidatePath (path, mousePoint)) {

				}



				if (GameManager.instance.combatManager.combatModeEnabled) {
					int pathCost = CalculatePathCost (path, mousePoint);
					if (ValidatePath (path, mousePoint) && (currentAP - pathCost) >= 0) {
						storedPath = path;
						storedPathCost = pathCost;
						GameManager.instance.uiManager.RenderPathLine (path.vectorPath);
						GameManager.instance.uiManager.UpdateVitalsText (currentHP, maxHP, currentAP - pathCost, maxAP);
					} else if (storedPath != null) {
						ResetPath ();
					}
				} else {
					if (ValidatePath (path, mousePoint)) {
						storedPath = path;
						GameManager.instance.uiManager.RenderPathLine (path.vectorPath);
					} else if (storedPath != null) {
						ResetPath ();
					}
				}
			}
		}

//		if (Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject()) { 
//			Debug.Log ("HOO");
//		};
//
//		if (Input.GetKeyDown ("p")) {
//			GameManager.instance.gamePaused = GameManager.instance.gamePaused ? false : true;
//
//		} else if (!GameManager.instance.gamePaused) {
//			if (Input.GetKeyDown ("b")) {
//				if (GameManager.instance.buildMode) {
//					GameManager.instance.buildMode = false;
//					turnStatusText.enabled = true;
//				} else {
//					GameManager.instance.buildMode = true;
//					turnStatusText.enabled = false;
//				}
//			} else if (Input.GetKeyDown ("c")) {
//				GameManager.instance.combatMode = GameManager.instance.combatMode ? false : true;
//			}
//
//			/*if (GameManager.instance.buildMode && Input.GetMouseButtonDown (0)) {
//				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//				int rayx = Mathf.RoundToInt (ray.origin.x);
//				int rayy = Mathf.RoundToInt (ray.origin.y);
//				Debug.Log (GameManager.instance.boardManager.combatBlockingArray [rayx, rayy]);
//				GameObject tileChoice = GameManager.instance.boardManager.wallTiles [Random.Range (0, GameManager.instance.boardManager.wallTiles.Length)];
//				GameObject instance = Instantiate (tileChoice, new Vector2(rayx, rayy), Quaternion.identity) as GameObject;
//				instance.transform.SetParent (GameManager.instance.boardManager.wallHolder);
//
//			}*/
//
//			if (!GameManager.instance.buildMode && GameManager.instance.playersTurn && Input.GetMouseButtonDown (0)) {
//				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//				Vector3 rayPoint = new Vector3 (Mathf.RoundToInt (ray.origin.x), Mathf.RoundToInt (ray.origin.y), 0);
//				//this.boxCollider.enabled = false;
//				GameManager.instance.ToggleColliders ("ally", false);
//				AstarPath.active.Scan ();
//
//				path = seeker.StartPath (new Vector3 (transform.position.x, transform.position.y, 0), rayPoint);
//				AstarPath.WaitForPath (path);
//
//				if (path != null && ( (Vector2) path.vectorPath[path.vectorPath.Count-1] == (Vector2) rayPoint)) {
//					Debug.Log (path.vectorPath.Count);
//					//GameManager.instance.uiManager.renderPathLine (path.vectorPath);
//					//Vector3 temp_position = this.transform.position;
//					for (int i = 0; i < path.vectorPath.Count; i++) {
//						Debug.Log (path.vectorPath [i]);
//						//Debug.Log (this.transform.position);
//						//AttemptMove<Wall> (Mathf.RoundToInt (path.vectorPath [i].x - this.transform.position.x), Mathf.RoundToInt (path.vectorPath [i].y - this.transform.position.y));
//
//						//AttemptMove<Wall> (Mathf.RoundToInt (path.vectorPath [i].x - temp_position.x), Mathf.RoundToInt (path.vectorPath [i].y - temp_position.y));
//						//temp_position = path.vectorPath [i];
//
//						//StartCoroutine(SmoothMovement((Vector2) path.vectorPath [i]));
//
//
//						//rb2D.MovePosition ((Vector2) path.vectorPath [i]);
//
//						//Debug.Log(Vector2.Distance (path.vectorPath [i], this.transform.position));
//						this.transform.Translate(new Vector2(path.vectorPath[i].x - this.transform.position.x, path.vectorPath[i].y - this.transform.position.y));
//
//						/*int j = 0;
//						while (Vector2.Distance (path.vectorPath [i], this.transform.position) > Mathf.Epsilon && j < 100) {
//							Vector2 dir = (path.vectorPath [i] - this.transform.position).normalized;
//							this.transform.Translate (dir);
//							j++;
//						}*/
//
//					}
//					AstarPath.active.Scan ();
//					GameManager.instance.ToggleColliders ("ally", true);
//					//this.boxCollider.enabled = true;
//					MoveExecuted ();
//				}
//
//				/*if (Mathf.Pow ((ray.origin.x - this.transform.position.x), 2) > Mathf.Pow ((ray.origin.y - this.transform.position.y), 2)) {
//					AttemptMove<Wall> (Mathf.RoundToInt (ray.origin.x - this.transform.position.x), 0); 
//				} else {
//					AttemptMove<Wall> (0, Mathf.RoundToInt(ray.origin.y - this.transform.position.y)); 
//				}*/
//
//				//AttemptMove<Wall> (Mathf.RoundToInt (ray.origin.x - this.transform.position.x), Mathf.RoundToInt(ray.origin.y - this.transform.position.y)); 
//			}
//		}
	} 

	protected void ExecuteMove () {
		if (isSelected && currentAP > 0 && storedPath != null) {
			for (int i = 0; i < storedPath.vectorPath.Count; i++) {
				this.transform.Translate(new Vector2(storedPath.vectorPath[i].x - this.transform.position.x, storedPath.vectorPath[i].y - this.transform.position.y));
			}
			if (GameManager.instance.combatManager.combatModeEnabled)
				currentAP -= storedPathCost;
			ResetPath ();
		}
	}

	protected void ResetPath () {
		storedPath = null;
		GameManager.instance.uiManager.UnrenderPathLine ();
		if (GameManager.instance.combatManager.combatModeEnabled) {
			storedPathCost = 666;
			GameManager.instance.uiManager.UpdateVitalsText (currentHP, maxHP, currentAP, maxAP);
		}
	}

	public void OnPathComplete(Path p) {
		Debug.Log ("Path returned. Error? " + p.error);
		if (!p.error)
			path = p;
	}

	protected void TargetTestAttack() {
		GameManager.instance.combatManager.ActivateTargeting ("ExecuteTestAttack");
	}

	protected void ExecuteTestAttack(GameObject hitTarget) {
		hitTarget.SendMessage ("Damage", damage, SendMessageOptions.DontRequireReceiver);
		ScanPaths ();
	}

	protected override void ProcessCombatPanelClick (int buttonNum) {
		Debug.Log (buttonNum);
		switch (buttonNum) {
		case 1:
			TargetTestAttack ();
			break;
		default:
			break;
		}
	}

	protected override void AttemptMove <T> (int xDir, int yDir) {
		base.AttemptMove <T> (xDir, yDir);

		RaycastHit2D hit;
	}

	protected override void OnCantMove <T> (T component) {
		if (component != null && component.tag == "Wall") {
			Wall hitWall = component as Wall;
			hitWall.DamageWall (wallDamage);
		}
	}

	public void DamagePlayer (int loss) {
		currentHP -= loss;
		CheckIfGameOver ();
	}

	private void MoveExecuted() {
		playerScore++;

		CheckIfGameOver ();

		GameManager.instance.playersTurn = false;
		turnStatusText.text = "Enemy Turn";
	}

	private void CheckIfGameOver() {
		if (currentHP <= 0) {
			//scoreText.text = "Stamina: 0/100. You can refill your stamina in the shop!";
			GameManager.instance.GameOver ();
		}
	}
}
