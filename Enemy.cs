using UnityEngine;
using System.Collections;

public class Enemy : MovingObject {

	public int playerDamage = 10;
	public int wallDamage = 5;
	public int hp = 10;

	private Animator animator;
	private Transform target;
	private bool skipMove;
	private Seeker seeker;

	protected override void Start () {
		GameManager.instance.AddEnemyToList (this);
		animator = GetComponent<Animator> ();
		seeker = GetComponent<Seeker> ();
		base.Start ();
	}

	protected override void AttemptMove <T> (int xDir, int yDir) {
		if (skipMove) {
			skipMove = false;
			return;
		}

		base.AttemptMove<T> (xDir, yDir);

		skipMove = true;
	}

	public void MoveEnemy() {
		int xDir = 0;
		int yDir = 0;

		if (target == null) 
			target = GameObject.FindGameObjectWithTag ("Player").transform;

		if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
			yDir = target.position.y > transform.position.y ? 1 : -1;
		else
			xDir = target.position.x > transform.position.x ? 1 : -1;

		AttemptMove<Player> (xDir, yDir);
	}

	protected override void OnCantMove <T> (T component) {
		if (component != null) {
			if (component.tag == "Player") {
				Player hitPlayer = component as Player;
				hitPlayer.DamagePlayer (playerDamage);
			} else if (component.tag == "Wall") {
				Wall hitWall = component as Wall;
				hitWall.DamageWall (wallDamage);
			}
		}
	}
}
