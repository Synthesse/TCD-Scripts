using UnityEngine;
using System.Collections;

public class Explode : Ability {

	private int layerMask = (1 << 8) | (1 << 9) ;

	public Explode() {
		apCost = 1;
		cooldown = 0;
		range = 1;
		friendlyTarget = false;
		abilityName = "Explode";
		abilityDescription = "Triggers the mine to detonate, dealing dmg to everything in radius 1.";
		abilityButtonText = abilityName+"\nRadius "+range;
		keyPress = "b";
		currentCooldown = 0;
		targetType = abilityTargetingTypes.Area;
	}

	public override IEnumerator Execute(Defenses self) {
		self.GetComponent<RemoteMine> ().collider.enabled = false;
		Collider2D[] collidersHit = Physics2D.OverlapAreaAll ((Vector2)self.transform.position - new Vector2 (1, 1), (Vector2)self.transform.position + new Vector2 (1, 1), layerMask);
		gameManager.soundManager.PlayMineExplosionSFX ();
		GameObject explosion = GameObject.Instantiate (gameManager.combatManager.explosionObj, self.transform.position, self.transform.rotation) as GameObject;
		//yield return new WaitUntil (() => (explosion.GetComponent<Animator> ().GetNextAnimatorStateInfo (0).IsName ("finished")));
		yield return new WaitForSeconds(0.75f);
		foreach (Collider2D collider in collidersHit) {
			if (collider != null)
				collider.gameObject.SendMessage ("Damage", self.atk, SendMessageOptions.DontRequireReceiver);
		}
		self.DeductAP (apCost);
		self.GetComponent<RemoteMine> ().collider.enabled = true;
		gameManager.playerInput.TogglePlayerInputLock (false);
		gameManager.combatManager.ToggleActionLock (false);
		self.Kill ();
	}

	public override IEnumerator Execute(Unit self) {
		self.GetComponent<Robot> ().collider.enabled = false;
		Collider2D[] collidersHit = Physics2D.OverlapAreaAll ((Vector2)self.transform.position - new Vector2 (1, 1), (Vector2)self.transform.position + new Vector2 (1, 1), layerMask);
		gameManager.soundManager.PlayExplosionSFX ();
		GameObject explosion = GameObject.Instantiate (gameManager.combatManager.explosionObj, self.transform.position, self.transform.rotation) as GameObject;
		//yield return new WaitUntil (() => (explosion.GetComponent<Animator> ().GetNextAnimatorStateInfo (0).IsName ("finished")));
		yield return new WaitForSeconds(0.75f);
		foreach (Collider2D collider in collidersHit) {
			if (collider != null)
				collider.gameObject.SendMessage ("Damage", self.atk, SendMessageOptions.DontRequireReceiver);
		}
		self.DeductAP (apCost);
		self.GetComponent<Robot> ().collider.enabled = true;
		gameManager.playerInput.TogglePlayerInputLock (false);
		gameManager.combatManager.ToggleActionLock (false);
		self.Kill ();
	}
}
