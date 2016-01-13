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
	public int testStat {get; set;}
	public string status = "Normal";
	public string special = "Nothing";
	protected Rigidbody2D rb2D;
	public bool isAlly = true;
	public bool nonlethalAmmo = false;

	public Seeker seeker;
	//private Path path;
	private List<Vector3> storedPath;
	private int storedPathCost = 666;

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
		numCombatActions = 3;
		storedPath = new List<Vector3> ();
	}

	protected override void Select () {
		base.Select ();
		ScanPaths ();
	}

	protected override void Deselect() {
		base.Deselect ();
		gameManager.uiManager.UnrenderPathLine (); 
	}
		
	protected override void UpdateObjectUIText ()
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
		if (isSelected && isAlly && currentAP > 0 && !GameManager.instance.combatManager.targetingActive)
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

	protected void ScanPaths () {
		boxCollider.enabled = false;
		AstarPath.active.Scan ();
		boxCollider.enabled = true;
	}

	protected void ExecuteMove () {
		if (isSelected && currentAP > 0 && storedPath.Count > 0) {
			for (int i = 0; i < storedPath.Count; i++) {
				this.transform.Translate (new Vector2 (storedPath [i].x - this.transform.position.x, storedPath [i].y - this.transform.position.y));
			}

			DeductAP (storedPathCost);
			ResetPath ();
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



	// COMBAT METHODS
	public virtual void Damage (int damageTaken) {
		currentHP -= Mathf.Max(damageTaken - def, 1);
		UpdateVitalsUIText ();
		if (currentHP <= 0) {
			Kill ();
		}
	}

	protected virtual void Kill () {
		if (isAlly)
			gameManager.combatManager.activeAllies.Remove (gameObject);
		else { 
			gameManager.combatManager.activeEnemies.Remove (gameObject);
			gameManager.cash++;
			gameManager.uiManager.UpdateCashText ();
		}
		gameManager.combatManager.targetedObjects.Remove (gameObject);
		gameObject.SetActive (false);
	}

	protected void DeductAP (int loss) {
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




	// COMBAT AI METHODS

	public virtual void EnableCombatAI() {
		if (!gameManager.combatManager.actionLock) {
			gameManager.combatManager.actionLock = true;
			//Select ();
			StorePathToNearest ();
			int j = 0;
			while (currentAP > 0) {
				gameManager.combatManager.FindTargets (gameObject, false);
				Debug.Log ("Finding Targets: "+gameManager.combatManager.targetedObjects.Count);
				if (gameManager.combatManager.targetedObjects.Count > 0 && currentAP > 1) {
					Debug.Log ("Attacking");
					ShittyTestAttack (gameManager.combatManager.targetedObjects [0]);
					gameManager.combatManager.ResetTargets ();
					StorePathToNearest ();
				} else {
					if (storedPath.Count == 0) {
						DeductAP (currentAP);
					} else {
						ExecuteNextAIMove ();
						Debug.Log ("Moving");
					}
				}
			}
			//Deselect ();
			storedPath.Clear ();
			storedPathCost = 666;
			gameManager.combatManager.actionLock = false;
		}
	}

	protected virtual void ShittyTestAttack (GameObject target) {
		target.SendMessage ("Target");
		target.SendMessage ("Damage", atk);
		target.SendMessage ("Untarget", SendMessageOptions.DontRequireReceiver);
		DeductAP (2);
	}

	protected void ExecuteNextAIMove () {
		if (currentAP > 0) {
			Vector2 nextMoveLocation = (Vector2) storedPath [0];
			int pathCost = (int)(Mathf.Abs (nextMoveLocation.x - this.transform.position.x) + Mathf.Abs (nextMoveLocation.y - this.transform.position.y));
			if (currentAP == 1 && pathCost == 2) {
				this.transform.Translate(new Vector2(nextMoveLocation.x - this.transform.position.x, 0));
				DeductAP (1);
			} else {
				this.transform.Translate(new Vector2(nextMoveLocation.x - this.transform.position.x, nextMoveLocation.y - this.transform.position.y));
				DeductAP (pathCost);
			}
			storedPath.RemoveAt (0);
		}
	}

	protected virtual void StorePathToNearest () {
		ScanPaths ();
		List<GameObject> activeTargets = gameManager.combatManager.GetActors (false);
	
		foreach (GameObject target in activeTargets) {
			Vector3 targetLocation = new Vector3 (target.transform.position.x, target.transform.position.y, 0);

			Path path = seeker.StartPath (new Vector3 (transform.position.x, transform.position.y, 0), targetLocation);
			AstarPath.WaitForPath (path);
			int pathCost = CalculatePathCost (path, targetLocation);
			if (pathCost < storedPathCost && path != null) {
				storedPath = path.vectorPath;
				storedPath.RemoveAt (0);
				storedPath.RemoveAt (storedPath.Count-1);
				storedPathCost = pathCost;
			}
		}
	}
}
