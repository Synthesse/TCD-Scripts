using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//SWITCH TO THIS ONCE WE FULLY IMPLEMENT INTERFACE REQUIREMENTS
//public class SelectableObject : PhysicalObject, ISelectable, IKillable {
public abstract class SelectableObject : PhysicalObject {

	protected bool isSelected = false;
	protected Color storedColor;
	public string objectName = "";
	protected int numCombatActions = 0;

	protected direction currentFacing;
	protected direction8 currentFacing8;
	protected SpriteRenderer spriteRenderer;
	public bool attackable;
	public List<Sprite> directionalSprites;
	public string special = "Nothing";

	protected virtual void Awake () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		storedColor = spriteRenderer.color;
	}

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		attackable = true;
	}

	protected virtual void Select () {
		gameObject.GetComponent<SpriteRenderer> ().color = new Color (0.5f,1f,0.6f,0.9f);
		gameManager.selectedObject = gameObject;
		isSelected = true;
		gameManager.playerInput.currentMouseGridLoc = (Vector3) GridLocate ();
		UpdateObjectUIText ();
		gameManager.uiManager.ToggleSelectedUnitUI (true, numCombatActions);

	}

	protected virtual void Deselect () {
		gameObject.GetComponent<SpriteRenderer> ().color = storedColor;
		gameManager.selectedObject = null;
		isSelected = false;
		gameManager.uiManager.ToggleSelectedUnitUI (false);
		//gameManager.playerInput.currentMouseGridLoc = null;
	}

	protected void Target () {
		gameObject.GetComponent<SpriteRenderer> ().color = new Color (1f,0.5f,0.5f,1);
		Debug.Log ("Targeting " + objectName);
	}

	protected void CantTarget () {
		gameObject.GetComponent<SpriteRenderer> ().color = new Color (0.4f,0.4f,0.4f,0.75f);
		Debug.Log ("Cant Target " + objectName);
	}

	protected void Untarget () {
		gameObject.GetComponent<SpriteRenderer> ().color = storedColor;
		Debug.Log ("Untarget " + objectName);
	}

	public virtual void UpdateObjectUIText () {
		gameManager.uiManager.UpdateNameText (objectName);
		gameManager.uiManager.UpdateVitalsText ();
		gameManager.uiManager.UpdateDetailsText (special);
	}

	public void ChangeFacing(direction newFacing) {
		spriteRenderer.sprite = directionalSprites [(int)newFacing];
		currentFacing = newFacing;
	}

	public void ChangeFacing(direction8 newFacing) {
		spriteRenderer.sprite = directionalSprites [(int)newFacing];
	}
}
