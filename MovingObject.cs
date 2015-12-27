using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {

	public float moveTime = 0.1f;
	public LayerMask blockingLayer;

	protected BoxCollider2D boxCollider;
	protected Rigidbody2D rb2D;
	private float inverseMoveTime;

	protected virtual void Start () {
		boxCollider = GetComponent<BoxCollider2D>();
		rb2D = GetComponent<Rigidbody2D>();
		inverseMoveTime = 1f / moveTime;
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
