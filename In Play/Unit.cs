using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

//SWITCH TO THIS ONCE WE FULLY IMPLEMENT INTERFACE REQUIREMENTS
//public class Unit : SelectableObject, IUnit {
public abstract class Unit : SelectableObject {

	public int currentHP;
	public int maxHP;
	public int currentAP;
	public int maxAP;
	public int atk;
	public int def;
	public string status = "Normal";
	public string special = "Nothing";
	protected Rigidbody2D rb2D;
	public bool isAlly = true;

	public Seeker seeker;
	//private Path path;
	protected List<Vector3> storedPath;
	protected int storedPathCost = 666;

	protected float inverseMoveTime;

	protected Animator animator;
	protected direction currentMovementDirection = direction.None;

	public List<Ability> abilityList;
	protected List<string> abilityTextList;

	protected int thrallIndex = -1;

	//TODO: Remove this. Refactor AI code to generate ranges based on abilities used.
	protected int aiAttackRange;


	// GENERAL METHODS
	protected virtual void Awake () {
		currentHP = 1;
		maxHP = 1;
		currentAP = 0;
		maxAP = 0;
		atk = 0;
		def = 0;
	}

	protected override void Start () {
		base.Start ();
		rb2D = GetComponent<Rigidbody2D>();
		seeker = GetComponent<Seeker> ();
		animator = GetComponent<Animator> ();
		animator.enabled = false;
		storedPath = new List<Vector3> ();
		abilityList = new List<Ability> ();
		currentFacing = direction.Down;
		currentFacing8 = direction8.None;
		inverseMoveTime = 1f / 0.45f;
	}

	protected override void Select () {
		base.Select ();
		ScanPaths ();
	}

	protected override void Deselect() {
		base.Deselect ();
		gameManager.uiManager.UnrenderPathLine (); 
	}
		
	public override void UpdateObjectUIText ()
	{
		base.UpdateObjectUIText ();
		gameManager.uiManager.UpdateVitalsText (currentHP, maxHP, currentAP, maxAP);
		gameManager.uiManager.UpdateDetailsText (status, maxHP, atk, def, maxAP, special);
	}

	protected void UpdateVitalsUIText() {
		if (isSelected)
			gameManager.uiManager.UpdateVitalsText (currentHP, maxHP, currentAP, maxAP);
	}

	protected virtual void Update() {
		if (!gameManager.playerInput.playerInputLock && isSelected && isAlly && currentAP > 0 && !GameManager.instance.combatManager.targetingActive)
			CheckMousePath ();
	}


	//PATH METHODS
	protected void CheckMousePath() {
		Vector3 mousePoint = gameManager.playerInput.GetMouseGridPosition ();
		if (mousePoint != gameManager.playerInput.currentMouseGridLoc) {
			gameManager.playerInput.currentMouseGridLoc = mousePoint;
			Path path = seeker.StartPath (new Vector3 (transform.position.x, transform.position.y, 0), mousePoint);
			AstarPath.WaitForPath (path);

			if (gameManager.combatManager.combatModeEnabled) {
				int pathCost = CalculatePathCost (path, mousePoint);
				if (ValidatePath (path, mousePoint) && (currentAP - pathCost) >= 0) {
					storedPath = path.vectorPath;
					storedPathCost = pathCost;
					gameManager.uiManager.RenderPathLine (path.vectorPath);
					gameManager.uiManager.UpdateVitalsText (currentHP, maxHP, currentAP - pathCost, maxAP, Color.yellow);
				} else if (storedPath.Count > 0) {
					ResetPath ();
				}
			} else {
				if (ValidatePath (path, mousePoint)) {
					storedPath = path.vectorPath;
					gameManager.uiManager.RenderPathLine (path.vectorPath);
				} else if (storedPath.Count > 0) {
					ResetPath ();
				}
			}
		}
	}

	protected bool ValidatePath (Path path, Vector3 endPoint) {
//		Vector2 pathEnd = new Vector2 (path.vectorPath [path.vectorPath.Count - 1].x, path.vectorPath [path.vectorPath.Count - 1].y);
//		Vector2 endPointv2 = new Vector2 (endPoint.x, endPoint.y);
//		return (path != null && (pathEnd == endPointv2));
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

	public void ScanPaths () {
		boxCollider.enabled = false;
		AstarPath.active.Scan ();
		boxCollider.enabled = true;
	}

	public IEnumerator ExecuteMove () {
		if (isSelected && currentAP > 0 && storedPath.Count > 0) {
			gameManager.playerInput.TogglePlayerInputLock (true);
			gameManager.combatManager.ToggleActionLock (true);
			DeductAP (storedPathCost);
			List<Vector3> movementVertices = storedPath.GetRange (1, storedPath.Count-1);
			ResetPath ();
			yield return StartCoroutine (SmoothMovement (movementVertices));
//			for (int i = 0; i < storedPath.Count; i++) {
//				this.transform.Translate (new Vector2 (storedPath [i].x - this.transform.position.x, storedPath [i].y - this.transform.position.y));
//			}
//
//			DeductAP (storedPathCost);
//			ResetPath ();
		}
	}

	protected void ResetPath () {
		storedPath.Clear ();
		gameManager.uiManager.UnrenderPathLine ();
		if (gameManager.combatManager.combatModeEnabled) {
			storedPathCost = 666;
			UpdateVitalsUIText ();
		}
	}



	protected void AnimateMovement(direction mdir) {
		if (!animator.enabled)
			animator.enabled = true;
		animator.SetTrigger ("walk"+mdir.ToString());
	}

	protected IEnumerator SmoothMovement (List<Vector3> movementList) {
		gameManager.soundManager.PlayWalkSFX ();
		direction currentDirection = direction.None;
		for (int i = 0; i < movementList.Count; i++) {
			direction newDirection = gameManager.boardManager.FindDirection (transform.position, movementList [i]);
			if (newDirection != currentDirection) {
				//Debug.Log (newDirection);
				AnimateMovement (newDirection);
				currentDirection = newDirection;
			}
			float sqrRemainingDistance = ((Vector2)transform.position - (Vector2)movementList [i]).sqrMagnitude;

			while (sqrRemainingDistance > float.Epsilon) {
				Vector2 newPosition = Vector2.MoveTowards (rb2D.position, (Vector2)movementList [i], inverseMoveTime * Time.deltaTime);
				this.transform.Translate (new Vector2 (newPosition.x - this.transform.position.x, newPosition.y - this.transform.position.y));
				sqrRemainingDistance = ((Vector2)transform.position - (Vector2)movementList [i]).sqrMagnitude;
				yield return null;
			}
		}

		gameManager.soundManager.StopSFXLoop ();
		animator.SetTrigger ("stand");
		animator.enabled = false;

		ChangeFacing (currentDirection);
		gameManager.playerInput.TogglePlayerInputLock(false);
		gameManager.combatManager.ToggleActionLock (false);
	}


	// COMBAT METHODS
	public virtual void Damage (int damageTaken) {
		Mark mark = GetComponentInChildren<Mark> ();
		if (mark != null) {
			damageTaken += mark.strength;
		}
		currentHP -= Mathf.Max(damageTaken - def, 1);
		UpdateVitalsUIText ();
		if (currentHP <= 0) {
			Kill ();
		}
	}

	public virtual void Heal (int heal) {
		currentHP = Mathf.Min (currentHP + heal, maxHP);
		UpdateVitalsUIText ();
	}

	public virtual void Kill () {
		if (isAlly) {
			gameManager.combatManager.activeAllies.Remove (gameObject);
			if (!gameManager.soundManager.raisedTension)
				gameManager.soundManager.raisedTension = true;
		} else { 
			gameManager.combatManager.activeEnemies.Remove (gameObject);
			gameManager.cash++;
			gameManager.uiManager.UpdateCashText ();
		}
		gameManager.combatManager.targetedObjects.Remove (gameObject);
		if (thrallIndex >= 0)
			FindObjectOfType<Leader> ().amplifiers [thrallIndex].Reset ();
		Destroy (gameObject);
	}

	public void DeductAP (int loss) {
		if (gameManager.combatManager.combatModeEnabled) {
			currentAP -= loss;
			gameManager.combatManager.currentSideAPPool -= loss;
			if (isSelected)
				UpdateVitalsUIText ();
		}
	}

	public void ResetAP () {
		currentAP = maxAP;
		if (isSelected)
			UpdateVitalsUIText ();
	}

	public void AddAPToPool () {
		gameManager.combatManager.currentSideAPPool += currentAP;
	}
		
	protected virtual void ProcessCombatPanelClick(int buttonNum) {
	}

	public void Flip(bool isNowAlly) {
		if (!isAlly && isNowAlly) {
			isAlly = true;
			gameObject.tag = "Ally";
			gameManager.combatManager.activeEnemies.Remove (gameObject);
			gameManager.combatManager.activeAllies.Add (gameObject);
		} else if (isAlly && !isNowAlly) {
			isAlly = false;
			gameObject.tag = "Ally";
			gameManager.combatManager.activeAllies.Remove (gameObject);
			gameManager.combatManager.activeEnemies.Add (gameObject);
		}
		AddAPToPool ();
		DeductAP (currentAP);
	}

	public void BecomeThrall(int ind, bool isNowAlly) {
		Flip (isNowAlly);
		objectName += " Thrall";
		thrallIndex = ind;
	}


	// COMBAT AI METHODS

	//TODO: MAKE THESE METHODS LESS SHIT
	public virtual IEnumerator EnableCombatAI() {
		if (!gameManager.combatManager.actionLock) { //Might have to remove this. Lots of weird coroutine/lock interactions
			gameManager.combatManager.ToggleActionLock(true);
			// Store path to neaest enemy
			StorePathToNearest ();
			List<Vector3> movePath = new List<Vector3> ();
			Vector3 currentLocation = this.transform.position;
			// While AP > 0
			while (currentAP > 0) {
				// Find targets in range
				boxCollider.enabled = false;
				gameManager.combatManager.FindTargets (currentLocation, false, aiAttackRange, false);
				boxCollider.enabled = true;
				Debug.Log ("Finding Targets: "+gameManager.combatManager.targetedObjects.Count);
				// If there are targets in range and current AP >= attack AP cost
				if (gameManager.combatManager.targetedObjects.Count > 0 && currentAP >= 2) {
					if (movePath.Count > 0) {
						gameManager.combatManager.ToggleActionLock (true);
						gameManager.playerInput.TogglePlayerInputLock(true);
						yield return StartCoroutine (SmoothMovement (movePath));
						movePath.Clear ();
					}
					Debug.Log ("Attacking");
					gameManager.combatManager.ToggleActionLock (true);
					gameManager.playerInput.TogglePlayerInputLock (true);
					yield return StartCoroutine(ShittyTestAttack (gameManager.combatManager.targetedObjects [0]));
					gameManager.combatManager.ResetTargets ();
					StorePathToNearest ();
				} else {
					if (storedPath.Count == 0) {
						Debug.Log ("Cant do anything, lose all AP");
						DeductAP (currentAP);
					} else {
						currentLocation = ExecuteNextAIMove (currentLocation);
						movePath.Add(currentLocation);
						Debug.Log ("Moving to ");
					}
				}
			}
			if (movePath.Count > 0) {
				gameManager.combatManager.ToggleActionLock (true);
				gameManager.playerInput.TogglePlayerInputLock(true);
				yield return StartCoroutine (SmoothMovement (movePath));
				movePath.Clear ();
			}
			//Deselect ();
			gameManager.combatManager.ResetTargets ();
			storedPath.Clear ();
			storedPathCost = 666;
			gameManager.combatManager.ToggleActionLock(false);
		}
	}

	protected virtual IEnumerator ShittyTestAttack (GameObject target) {
		Attack attack = new Attack (aiAttackRange);
		yield return StartCoroutine(attack.Execute (this, target));
	}

	protected Vector3 ExecuteNextAIMove (Vector3 currentLocation) {
		if (currentAP > 0) {
			Vector2 nextMoveLocation = (Vector2)storedPath [0];
			int pathCost = (int)(Mathf.Abs (nextMoveLocation.x - currentLocation.x) + Mathf.Abs (nextMoveLocation.y - currentLocation.y));
			storedPath.RemoveAt (0);
			if (currentAP == 1 && pathCost == 2) {
				DeductAP (1);
//				return new Vector3 (nextMoveLocation.x - currentLocation.x, 0);
				return new Vector3 (nextMoveLocation.x, currentLocation.y);
			} else {
				DeductAP (pathCost);
//				return new Vector3 (nextMoveLocation.x - currentLocation.x, nextMoveLocation.y - currentLocation.y);
				return new Vector3 (nextMoveLocation.x, nextMoveLocation.y);
			}
		} else
			return currentLocation;
	}

	protected virtual void StorePathToNearest () {
		ScanPaths ();
		List<GameObject> activeTargets = gameManager.combatManager.GetActors (false);
	
		foreach (GameObject target in activeTargets) {
			Vector3 targetLocation = new Vector3 (-1,-1,-1);
			//TODO: idea, give each AI unit a sorted/dictionary of target enemies, then update each AI list when the enemy in question moves. then AI can just pull from min of list
			Path path = null;

			foreach (Vector2 potentialEdgeLocation in target.GetComponent<PhysicalObject>().NearestOpenSpaces((Vector2)transform.position)) {
				path = seeker.StartPath (new Vector3 (transform.position.x, transform.position.y, 0), (Vector3)potentialEdgeLocation);
				AstarPath.WaitForPath (path);
				if (path != null && ValidatePath(path, (Vector3)potentialEdgeLocation)) {
					targetLocation = potentialEdgeLocation;
					break;
				}
			}
			if (targetLocation.x >= 0) {
				int pathCost = CalculatePathCost (path, targetLocation);
				if (pathCost < storedPathCost) {
					storedPath = path.vectorPath;
					storedPath.RemoveAt (0);
					//storedPath.RemoveAt (storedPath.Count - 1);
					storedPathCost = pathCost;
				}
			}
		}
	}
}
