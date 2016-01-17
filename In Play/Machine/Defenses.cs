﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Defenses : Machine {

	public int currentAP;
	public int maxAP;
	public int atk;
	public string special = "Nothing";
	public bool isAlly = true;
	protected List<Ability> abilityList;

	// TODO: move particle objects created to their respective child classes
	public GameObject laserAttackObj;

	protected override void Awake () {
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
		gameManager.uiManager.UpdateVitalsText (currentHP, maxHP, currentAP, maxAP);
		gameManager.uiManager.UpdateDetailsText (status, maxHP, atk, def, maxAP, special);
	}

	protected void UpdateVitalsUIText() {
		if (isSelected)
			gameManager.uiManager.UpdateVitalsText (currentHP, maxHP, currentAP, maxAP);
	}


	//COMBAT METHODS
	public override void Damage (int damageTaken) {
		currentHP -= Mathf.Max (damageTaken - def, 1);
		UpdateVitalsUIText ();
		if (currentHP <= 0) {
			Kill ();
		}
	}

	protected override void Kill () {
		if (isAlly)
			gameManager.combatManager.activeAllies.Remove (gameObject);
		else { 
			gameManager.combatManager.activeEnemies.Remove (gameObject);
		}
		gameManager.combatManager.targetedObjects.Remove (gameObject);
		gameObject.SetActive (false);
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