using UnityEngine;
using System.Collections;
using Pathfinding;

//SWITCH TO THIS ONCE WE FULLY IMPLEMENT INTERFACE REQUIREMENTS
//public class Unit : SelectableObject, IUnit {
public abstract class Unit : SelectableObject {

	public int currentHP = 10;
	public int maxHP = 10;
	public int currentAP = 5;
	public int maxAP = 5;
	public int atk = 5;
	public int def = 2;
	public string status = "Normal";
	public string special = "Nothing";
	protected Rigidbody2D rb2D;
	public bool isAlly = true;
	public bool nonlethalAmmo = false;

	public Seeker seeker;
	private Path path;
	private Path storedPath = null;
	private int storedPathCost = 666;

	// GENERAL METHODS
	protected override void Start () {
		base.Start ();
		rb2D = GetComponent<Rigidbody2D>();
		seeker = GetComponent<Seeker> ();
		numCombatActions = 3;
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
					storedPath = path;
					storedPathCost = pathCost;
					gameManager.uiManager.RenderPathLine (path.vectorPath);
					gameManager.uiManager.UpdateVitalsText (currentHP, maxHP, currentAP - pathCost, maxAP, Color.yellow);
				} else if (storedPath != null) {
					ResetPath ();
				}
			} else {
				if (ValidatePath (path, mousePoint)) {
					storedPath = path;
					gameManager.uiManager.RenderPathLine (path.vectorPath);
				} else if (storedPath != null) {
					ResetPath ();
				}
			}
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

	protected void ScanPaths () {
		boxCollider.enabled = false;
		AstarPath.active.Scan ();
		boxCollider.enabled = true;
	}

	protected void ExecuteMove () {
		if (isSelected && currentAP > 0 && storedPath != null) {
			for (int i = 0; i < storedPath.vectorPath.Count; i++) {
				this.transform.Translate(new Vector2(storedPath.vectorPath[i].x - this.transform.position.x, storedPath.vectorPath[i].y - this.transform.position.y));
			}

			DeductAP (storedPathCost);
			ResetPath ();
		}
	}

	protected void ResetPath () {
		storedPath = null;
		gameManager.uiManager.UnrenderPathLine ();
		if (gameManager.combatManager.combatModeEnabled) {
			storedPathCost = 666;
			UpdateVitalsUIText ();
		}
	}



	// COMBAT METHODS
	public virtual void Damage (int damageTaken) {
		currentHP -= damageTaken;
		UpdateVitalsUIText ();
		if (currentHP <= 0) {
			if (isAlly) 
				gameManager.combatManager.activeAllies.Remove (gameObject);
			else 
				gameManager.combatManager.activeEnemies.Remove (gameObject);
			gameManager.combatManager.targetedObjects.Remove (gameObject);
			gameObject.SetActive (false);
		}
	}

	protected void DeductAP (int loss) {
		if (gameManager.combatManager.combatModeEnabled) {
			currentAP -= loss;
			gameManager.combatManager.currentSideAPPool -= loss;
			UpdateVitalsUIText ();
		}
	}

	protected void AddAPToPool () {
		gameManager.combatManager.currentSideAPPool += currentAP;
	}
		
	protected virtual void ProcessCombatPanelClick(int buttonNum) {
	}

}
