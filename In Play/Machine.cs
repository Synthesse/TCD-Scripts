using UnityEngine;
using System.Collections;

//SWITCH TO THIS ONCE WE FULLY IMPLEMENT INTERFACE REQUIREMENTS
//public class Machine : SelectableObject, IMachine {
public abstract class Machine : SelectableObject {

	public int currentHP;
	public int maxHP;
	public int def;
	public string status = "Normal";

	protected virtual void Awake () {
		currentHP = 1;
		maxHP = 1;
		def = 0;
	}

	// Use this for initialization
	void Start () {
		base.Start ();
	}

	public virtual void Damage (int damageTaken) {
		currentHP -= Mathf.Max (damageTaken - def, 1);
		if (currentHP <= 0) {
			Kill ();
		}
	}

	protected virtual void Kill () {
		gameManager.combatManager.targetedObjects.Remove (gameObject);
		gameObject.SetActive (false);
	}
}
