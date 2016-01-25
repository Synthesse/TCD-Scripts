using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Defenses : Machine {

	public int currentAP;
	public int maxAP;
	public int atk;
	public bool isAlly = true;
	public List<Ability> abilityList;

	protected override void Awake () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		storedColor = spriteRenderer.color;
		currentHP = 1;
		maxHP = 1;
		currentAP = 1;
		maxAP = 1;
		atk = 0;
		def = 0;
	}

	protected override void Start() {
		base.Start ();
		abilityList = new List<Ability> ();
		currentFacing = direction.Down;
		currentFacing8 = direction8.None;
	}

	public override void UpdateObjectUIText ()
	{
		base.UpdateObjectUIText ();
		if (currentAP == 0) {
			gameManager.uiManager.UpdateVitalsText (currentHP, maxHP, currentAP, maxAP, Color.yellow);
		} else { 
			gameManager.uiManager.UpdateVitalsText (currentHP, maxHP, currentAP, maxAP);
		}
		gameManager.uiManager.UpdateDetailsText (status, maxHP, atk, def, maxAP, special);
	}

	protected override void UpdateVitalsUIText() {
		if (isSelected) {
			if (currentAP == 0) {
				gameManager.uiManager.UpdateVitalsText (currentHP, maxHP, currentAP, maxAP, Color.yellow);
			} else { 
				gameManager.uiManager.UpdateVitalsText (currentHP, maxHP, currentAP, maxAP);
			}
		}
	}


	//COMBAT METHODS
	public override void Damage (int damageTaken) {
		currentHP -= Mathf.Max (damageTaken - def, 1);
		UpdateVitalsUIText ();
		if (currentHP <= 0) {
			Kill ();
		}
	}

	public override void Kill () {
		if (isAlly)
			gameManager.combatManager.activeAllies.Remove (gameObject);
		else { 
			gameManager.combatManager.activeEnemies.Remove (gameObject);
		}
		gameManager.combatManager.targetedObjects.Remove (gameObject);
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

}
