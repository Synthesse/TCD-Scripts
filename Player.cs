using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Pathfinding;

public class Player : MovingObject {

	public int wallDamage = 5;
	public static Player instance = null;
	public int hp = 10;
	public int playerScore = 0;
	public Text scoreText; 
	public Text turnStatusText;
	public Text buildModeText;
	//public GameObject[] wallTiles;

	private Seeker seeker;
	private Path path;

	protected override void Start () {
		AstarPath.active.Scan ();

		scoreText = GameObject.Find("scoreText").GetComponent<Text>();
		scoreText.text = "Score: " + playerScore;
		turnStatusText = GameObject.Find("turnStatusText").GetComponent<Text>();
		turnStatusText.text = "Player Turn";
		buildModeText = GameObject.Find("buildModeText").GetComponent<Text>();
		buildModeText.enabled = false;
		seeker = GetComponent<Seeker> ();

		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		base.Start ();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("escape"))
			Application.Quit();

		if (Input.GetKeyDown ("p")) {
			GameManager.instance.gamePaused = GameManager.instance.gamePaused ? false : true;

		} else if (!GameManager.instance.gamePaused) {
			if (Input.GetKeyDown ("b")) {
				if (GameManager.instance.buildMode) {
					GameManager.instance.buildMode = false;
					buildModeText.enabled = false;
					turnStatusText.enabled = true;
				} else {
					GameManager.instance.buildMode = true;
					turnStatusText.enabled = false;
					buildModeText.enabled = true;
				}
			} else if (Input.GetKeyDown ("c")) {
				GameManager.instance.combatMode = GameManager.instance.combatMode ? false : true;
			}

			if (GameManager.instance.buildMode && Input.GetMouseButtonDown (0)) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				int rayx = Mathf.RoundToInt (ray.origin.x);
				int rayy = Mathf.RoundToInt (ray.origin.y);
				Debug.Log (GameManager.instance.boardScript.combatBlockingArray [rayx, rayy]);
				GameObject tileChoice = GameManager.instance.boardScript.wallTiles [Random.Range (0, GameManager.instance.boardScript.wallTiles.Length)];
				GameObject instance = Instantiate (tileChoice, new Vector2(rayx, rayy), Quaternion.identity) as GameObject;
				instance.transform.SetParent (GameManager.instance.boardScript.wallHolder);

			}

			if (!GameManager.instance.buildMode && GameManager.instance.playersTurn && Input.GetMouseButtonDown (0)) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Vector3 rayPoint = new Vector3 (Mathf.RoundToInt (ray.origin.x), Mathf.RoundToInt (ray.origin.y), 0);
				//this.boxCollider.enabled = false;
				GameManager.instance.ToggleColliders ("ally", false);
				AstarPath.active.Scan ();

				path = seeker.StartPath (new Vector3 (transform.position.x, transform.position.y, 0), rayPoint);
				AstarPath.WaitForPath (path);

				if (path != null && ( (Vector2) path.vectorPath[path.vectorPath.Count-1] == (Vector2) rayPoint)) {
					Debug.Log (path.vectorPath.Count);
					//Vector3 temp_position = this.transform.position;
					for (int i = 0; i < path.vectorPath.Count; i++) {
						Debug.Log (path.vectorPath [i]);
						//Debug.Log (this.transform.position);
						//AttemptMove<Wall> (Mathf.RoundToInt (path.vectorPath [i].x - this.transform.position.x), Mathf.RoundToInt (path.vectorPath [i].y - this.transform.position.y));

						//AttemptMove<Wall> (Mathf.RoundToInt (path.vectorPath [i].x - temp_position.x), Mathf.RoundToInt (path.vectorPath [i].y - temp_position.y));
						//temp_position = path.vectorPath [i];

						//StartCoroutine(SmoothMovement((Vector2) path.vectorPath [i]));


						//rb2D.MovePosition ((Vector2) path.vectorPath [i]);

						//Debug.Log(Vector2.Distance (path.vectorPath [i], this.transform.position));
						this.transform.Translate(new Vector2(path.vectorPath[i].x - this.transform.position.x, path.vectorPath[i].y - this.transform.position.y));

						/*int j = 0;
						while (Vector2.Distance (path.vectorPath [i], this.transform.position) > Mathf.Epsilon && j < 100) {
							Vector2 dir = (path.vectorPath [i] - this.transform.position).normalized;
							this.transform.Translate (dir);
							j++;
						}*/

					}

					GameManager.instance.ToggleColliders ("ally", true);
					//this.boxCollider.enabled = true;
					MoveExecuted ();
				}

				/*if (Mathf.Pow ((ray.origin.x - this.transform.position.x), 2) > Mathf.Pow ((ray.origin.y - this.transform.position.y), 2)) {
					AttemptMove<Wall> (Mathf.RoundToInt (ray.origin.x - this.transform.position.x), 0); 
				} else {
					AttemptMove<Wall> (0, Mathf.RoundToInt(ray.origin.y - this.transform.position.y)); 
				}*/

				//AttemptMove<Wall> (Mathf.RoundToInt (ray.origin.x - this.transform.position.x), Mathf.RoundToInt(ray.origin.y - this.transform.position.y)); 
			}
		}
	}

	public void OnPathComplete(Path p) {
		Debug.Log ("Path returned. Error? " + p.error);
		if (!p.error)
			path = p;
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
		hp -= loss;
		CheckIfGameOver ();
	}

	private void MoveExecuted() {
		playerScore++;
		scoreText.text = "Score: " + playerScore;

		CheckIfGameOver ();

		GameManager.instance.playersTurn = false;
		turnStatusText.text = "Enemy Turn";
	}

	private void CheckIfGameOver() {
		if (hp <= 0) {
			scoreText.text = "GAME OVER - Score: " + playerScore;
			//scoreText.text = "Stamina: 0/100. You can refill your stamina in the shop!";
			GameManager.instance.GameOver ();
		}
	}
}
