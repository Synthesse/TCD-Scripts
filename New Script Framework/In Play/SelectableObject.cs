using UnityEngine;
using System.Collections;

//SWITCH TO THIS ONCE WE FULLY IMPLEMENT INTERFACE REQUIREMENTS
//public class SelectableObject : PhysicalObject, ISelectable, IKillable {
public abstract class SelectableObject : PhysicalObject {

	protected bool isSelected = false;
	protected Color storedColor;
	protected string objectName = "";
	protected int numCombatActions = 0;

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		storedColor = gameObject.GetComponent<SpriteRenderer> ().color;
	}

	protected virtual void Select () {
		gameObject.GetComponent<SpriteRenderer> ().color = new Color (0,0.5f,0,0.5f);
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
		gameObject.GetComponent<SpriteRenderer> ().color = new Color (0.5f,0,0,1);
	}

	protected void Untarget () {
		gameObject.GetComponent<SpriteRenderer> ().color = storedColor;
	}

	protected virtual void UpdateObjectUIText () {
		gameManager.uiManager.UpdateNameText (objectName);
	}
}
