using UnityEngine;
using System.Collections;

public class DoorBarrierAbility : Ability { 

	private int layerMask = (1 << 8) ;

	public DoorBarrierAbility() {
		apCost = 1;
		cooldown = 0;
		range = 666;
		friendlyTarget = false;
		abilityName = "Activate";
		abilityDescription = "Engages the door barrier";
		abilityButtonText = abilityName+" force field";
		keyPress = "b";
		currentCooldown = 0;
		targetType = abilityTargetingTypes.Self;
	}

	public override IEnumerator Execute(Defenses self) {
		Collider2D checkAbove1 = Physics2D.OverlapPoint(new Vector2(Mathf.FloorToInt(self.transform.position.x),Mathf.FloorToInt(self.transform.position.y)), layerMask);
		Collider2D checkAbove2 = Physics2D.OverlapPoint(new Vector2(Mathf.CeilToInt(self.transform.position.x),Mathf.CeilToInt(self.transform.position.y)), layerMask);
		DoorBarrier doorBarrier = self.GetComponentInChildren<DoorBarrier> ();

		Debug.Log (checkAbove1);
		Debug.Log (checkAbove2);
		Debug.Log ((checkAbove1 == null || checkAbove1.gameObject.GetComponent<DoorBarrier>() != null));
		Debug.Log ((checkAbove2 == null || checkAbove2.gameObject.GetComponent<DoorBarrier>() != null));
		if (doorBarrier.barrierEnabled || ((checkAbove1 == null || checkAbove1.gameObject.GetComponent<DoorBarrier>() != null) && (checkAbove2 == null || checkAbove2.gameObject.GetComponent<DoorBarrier>() != null))) {
			Debug.Log ("pass");
			self.DeductAP (apCost);
			Animator doorAnimator = self.GetComponent<Animator> ();

			Animator barrierAnimator = doorBarrier.GetComponent<Animator> ();
			if (doorBarrier.barrierEnabled) {
				Debug.Log ("Activate door barrier");
				gameManager.soundManager.PlayDeactivateSFX ();
				barrierAnimator.SetTrigger ("disableBarrierAnimation");
				yield return new WaitUntil (() => barrierAnimator.GetNextAnimatorStateInfo (0).IsName ("rest"));
				doorAnimator.SetTrigger ("disableDoorAnimation");
				yield return new WaitUntil (() => doorAnimator.GetNextAnimatorStateInfo (0).IsName ("rest"));
				self.gameObject.layer = 9;
				doorBarrier.barrierEnabled = false;
				self.attackable = false;
				abilityName = "Activate";
			} else {
				Debug.Log ("Deactivate door barrier");
				gameManager.soundManager.PlayActivateSFX ();
				doorAnimator.SetTrigger ("enableDoorAnimation");
				yield return new WaitUntil (() => doorAnimator.GetNextAnimatorStateInfo (0).IsName ("enabled"));
				barrierAnimator.SetTrigger ("enableBarrierAnimation");
				yield return new WaitUntil (() => barrierAnimator.GetNextAnimatorStateInfo (0).IsName ("enabled"));
				self.gameObject.layer = 8;
				doorBarrier.barrierEnabled = true;
				self.attackable = true;
				abilityName = "Deactivate";
			}
		}
		gameManager.playerInput.TogglePlayerInputLock (false);
		gameManager.combatManager.ToggleActionLock (false);
	}
		
}
