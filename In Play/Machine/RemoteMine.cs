using UnityEngine;
using System.Collections;

public class RemoteMine : Defenses {

	public BoxCollider2D collider;

	protected override void Awake () {
		currentHP = 1;
		maxHP = 1;
		currentAP = 1;
		maxAP = 1;
		atk = 15;
		def = 0;
		objectName = "Remote Mine";
	}

	protected override void Start() {
		base.Start ();
		collider = GetComponent<BoxCollider2D> ();
		abilityList.Add(new Explode());
		numCombatActions = 1;
		attackable = false;
	}

	protected override void ProcessCombatPanelClick (int buttonNum) {
		switch (buttonNum) {
		case 1:
			if (abilityList [0].apCost <= currentAP) {
				gameManager.playerInput.TogglePlayerInputLock (true);
				gameManager.combatManager.ToggleActionLock (true);
				StartCoroutine (abilityList [0].Execute (this));
			}
			break;
		case 2:
			break;
		default:
			break;
		}
	}

}
