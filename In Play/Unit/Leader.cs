using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Leader : Unit {

	public List<NeuralAmplifier> amplifiers;
	public int numThralls;

	protected override void Awake ()
	{
		objectName = "Valerie";
		currentHP = 10;
		maxHP = 10;
		currentAP = 6;
		maxAP = 6;
		atk = 6;
		def = 2;
		numThralls = 0;
		special = "Is Awesome";
	}

	protected override void Start() {
		base.Start ();
		numCombatActions = 3;
		abilityList.Add (new Attack (3));
		abilityList.Add (new Inspire ());
		abilityList.Add (new MindControl ());
//		Sprite testSprite = Resources.Load<Sprite> ("lead_researcher_transparent_1");
//		directionalSprites.Add(testSprite);
//		directionalSprites.Add(Resources.Load<Sprite> ("lead_researcher_transparent_4"));
//		directionalSprites.Add(Resources.Load<Sprite> ("lead_researcher_transparent_7"));
//		directionalSprites.Add(Resources.Load<Sprite> ("lead_researcher_transparent_10"));

	}

	public override void Damage (int damageTaken)
	{
		base.Damage (damageTaken);
		if (!gameManager.soundManager.raisedTension)
			gameManager.soundManager.raisedTension = true;
		if (currentHP <= 0)
			gameManager.GameOver ();
	}
		
		


	protected void ExecuteTestAttack(GameObject hitTarget) {
		//abilityList [0].Execute (this, hitTarget);

//		int attackAPCost = 2;
//		hitTarget.SendMessage ("Damage", atk, SendMessageOptions.DontRequireReceiver);
//		DeductAP (attackAPCost);
//		ScanPaths ();
	}

	protected override void ProcessCombatPanelClick (int buttonNum) {
		switch (buttonNum) {
		case 1:
			//Attack
			if (abilityList [0].apCost <= currentAP)
				gameManager.combatManager.ActivateTargeting (abilityList [0]);
			break;
		case 2:
			//Haste
			if (abilityList [1].apCost <= currentAP)
				gameManager.combatManager.ActivateTargeting (abilityList [1]);
			break;
		case 3:
			//MC
			if (abilityList [2].apCost <= currentAP && numThralls < amplifiers.Count)
				gameManager.combatManager.ActivateTargeting (abilityList [2]);
			break;
		default:
			break;
		}
	}

	public void AddThrall(GameObject thrall) {
		numThralls++;
		foreach (NeuralAmplifier amp in amplifiers) {
			if (amp.controlledUnit == null) {
				amp.controlledUnit = thrall.GetComponent<Unit> ();
				amp.animator.enabled = true;
				break;
			}
		}
	}
}
