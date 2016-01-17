using UnityEngine;
using System.Collections;

public class Leader : Unit {

	protected override void Awake ()
	{
		objectName = "Valerie";
		currentHP = 20;
		maxHP = 20;
		currentAP = 6;
		maxAP = 6;
		atk = 6;
		def = 2;
	}

	protected override void Start() {
		base.Start ();
		abilityList.Add (new Attack ());
		abilityList.Add (new Defend ());
//		Sprite testSprite = Resources.Load<Sprite> ("lead_researcher_transparent_1");
//		directionalSprites.Add(testSprite);
//		directionalSprites.Add(Resources.Load<Sprite> ("lead_researcher_transparent_4"));
//		directionalSprites.Add(Resources.Load<Sprite> ("lead_researcher_transparent_7"));
//		directionalSprites.Add(Resources.Load<Sprite> ("lead_researcher_transparent_10"));

	}

	public override void Damage (int damageTaken)
	{
		base.Damage (damageTaken);
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
			//Target Attack
			gameManager.combatManager.ActivateTargeting (abilityList [0]);
			break;
		case 2:
			//Defend
			StartCoroutine(abilityList[1].Execute(this));
			break;
		default:
			break;
		}
	}
}
