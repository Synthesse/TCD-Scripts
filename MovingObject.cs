using UnityEngine;
using Pathfinding;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {

	public float moveTime = 0.1f;
	public LayerMask blockingLayer;

	public int currentHP = 10;
	public int maxHP = 10;
	public int currentAP = 5;
	public int maxAP = 5;
	public int atk = 5;
	public int def = 2;
	public string status = "Normal";
	public string special = "Nothing";
	protected BoxCollider2D boxCollider;
	protected Rigidbody2D rb2D;
	private float inverseMoveTime;
	protected Color storedColor;
	protected bool isSelected = false;
	protected string unitName = "";
	protected int numCombatActions = 3;

	protected virtual void Start () {
		boxCollider = GetComponent<BoxCollider2D>();
		rb2D = GetComponent<Rigidbody2D>();
		inverseMoveTime = 1f / moveTime;
		storedColor = gameObject.GetComponent<SpriteRenderer> ().color;
	}

	protected bool Move (int xDir, int yDir, out RaycastHit2D hit) {
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);

		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;

		if (hit.transform == null) {
			StartCoroutine (SmoothMovement (end));
			return true;
		}

		return false;
	}

	protected IEnumerator SmoothMovement (Vector2 end) {
		float sqrRemainingDistance = ((Vector2)transform.position - end).sqrMagnitude;

		while (sqrRemainingDistance > float.Epsilon) {
			Vector2 newPosition = Vector2.MoveTowards (rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition (newPosition);
			sqrRemainingDistance = ((Vector2)transform.position - end).sqrMagnitude;
			yield return null;
		}
	}

	protected bool ValidatePath (Path path, Vector3 endPoint) {
		return (path != null && ((Vector2)path.vectorPath [path.vectorPath.Count - 1] == (Vector2)endPoint));
	}

	protected int CalculatePathCost (Path path, Vector3 endPoint) {
		int runningCost = 0;
		for (int k = 1; k < path.vectorPath.Count; k++) {
			//Debug.Log ("(" + path.vectorPath [k - 1].x.ToString () + "," + path.vectorPath [k - 1].y.ToString () + ")  (" + path.vectorPath [k].x.ToString () + "," + path.vectorPath [k].y.ToString () + ")");
			runningCost += Mathf.Min(Mathf.RoundToInt((Mathf.Pow((path.vectorPath[k].x - path.vectorPath[k-1].x),2) + Mathf.Pow((path.vectorPath[k].y - path.vectorPath[k-1].y),2)))*2, 3);
		}
		return Mathf.CeilToInt (runningCost/2f);
	}

	protected void DeductAP (int loss) {
		if (GameManager.instance.combatManager.combatModeEnabled) {
			currentAP -= loss;
			GameManager.instance.combatManager.currentSideAPPool -= loss;
			GameManager.instance.uiManager.UpdateVitalsText (currentHP, maxHP, currentAP, maxAP);
		}
	}

	protected void ScanPaths () {
		boxCollider.enabled = false;
		AstarPath.active.Scan ();
		boxCollider.enabled = true;
	}

	protected Vector2 GridLocate () {
		return (new Vector2 (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.y)));
	}

	protected Vector2[] GridLocate (bool giveCorners) {
		// TODO: Make this work for objects which are larger than one square
		Vector2[] gridLocations = new Vector2[4];
		return (gridLocations);
	}

	protected void Select () {
		gameObject.GetComponent<SpriteRenderer> ().color = new Color (0,0.5f,0,0.5f);
		GameManager.instance.selectedObject = gameObject;
		isSelected = true;
		GameManager.instance.playerInput.currentMouseGridLoc = (Vector3) GridLocate ();
		ScanPaths ();
		UpdateUnitUIText ();
		GameManager.instance.uiManager.ToggleSelectedUnitUI (true, numCombatActions);
	}

	protected void Deselect () {
		gameObject.GetComponent<SpriteRenderer> ().color = storedColor;
		Debug.Log (storedColor);
		GameManager.instance.selectedObject = null;
		isSelected = false;
		GameManager.instance.uiManager.UnrenderPathLine ();
		GameManager.instance.uiManager.ToggleSelectedUnitUI (false);
		//GameManager.instance.playerInput.currentMouseGridLoc = null;
	}

	protected void Target () {
		gameObject.GetComponent<SpriteRenderer> ().color = new Color (0.5f,0,0,1);
	}

	protected void Untarget () {
		gameObject.GetComponent<SpriteRenderer> ().color = storedColor;
	}

	protected void AddAPToPool () {
		GameManager.instance.combatManager.currentSideAPPool += currentAP;
	}

	protected virtual void UpdateUnitUIText () {
		GameManager.instance.uiManager.UpdateNameText (unitName);
		GameManager.instance.uiManager.UpdateVitalsText (currentHP, maxHP, currentAP, maxAP);
		GameManager.instance.uiManager.UpdateDetailsText (status, maxHP, atk, def, maxAP, special);
	}

	protected virtual void ProcessCombatPanelClick(int buttonNum) {
	}

	protected virtual void AttemptMove <T> (int xDir, int yDir) 
		where T : Component {
		RaycastHit2D hit;
		bool canMove = Move (xDir, yDir, out hit);

		if (hit.transform == null) 
			return;

		//if (hit.transform.tag == "Player") {
			Player playerHitComponent = hit.transform.GetComponent<Player> ();
		//} else if (hit.transform.tag == "Wall") {
			Wall wallHitComponent = hit.transform.GetComponent<Wall> ();
		//} else {
			T hitComponent = hit.transform.GetComponent<T> ();
		//}

		if (!canMove) {
			if (wallHitComponent != null)
				OnCantMove (wallHitComponent);
			else if (playerHitComponent != null)
				OnCantMove (playerHitComponent);
			else
				OnCantMove (hitComponent);
		}
			
			
	}
		
	protected abstract void OnCantMove <T> (T component)
		where T : Component;

}
