using UnityEngine;
using Pathfinding;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {

	public float moveTime = 0.1f;
	public LayerMask blockingLayer;

	protected BoxCollider2D boxCollider;
	protected Rigidbody2D rb2D;
	private float inverseMoveTime;
	protected Color storedColor;
	protected bool isSelected = false;

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

	protected void ScanPaths () {
		boxCollider.enabled = false;
		AstarPath.active.Scan ();
		boxCollider.enabled = true;
	}

	protected Vector2 GridLocate () {
		return (new Vector2 (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.y)));
	}

	protected Vector2[] GridLocate (bool giveCorners) {
		Vector2[] gridLocations = new Vector2[4];
		return (gridLocations);
	}

	protected void Select () {
		gameObject.GetComponent<SpriteRenderer> ().color = new Color (0,0.5f,0,1);
		GameManager.instance.selectedObject = gameObject;
		isSelected = true;
		GameManager.instance.playerInput.currentMouseGridLoc = (Vector3) GridLocate ();
		ScanPaths ();
		GameManager.instance.uiManager.ToggleSelectedUnitUI (true);
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
