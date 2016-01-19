using UnityEngine;
using System.Collections;

public class DoorBarrierAbility : Ability { 

	public bool barrierEnabled;
	private int layerMask = (1 << 8) ;

	public DoorBarrierAbility() {
		apCost = 1;
		cooldown = 0;
		range = 666;
		friendlyTarget = false;
		abilityName = "Barrier";
		abilityDescription = "Engages the door barrier";
		keyPress = "b";
		currentCooldown = 0;
		targetType = abilityTargetingTypes.Self;
	}

	public override IEnumerator Execute(Defenses self) {
		Collider2D checkAbove1 = Physics2D.OverlapPoint(new Vector2(Mathf.FloorToInt(self.transform.position.x),Mathf.FloorToInt(self.transform.position.y)), layerMask);
		Collider2D checkAbove2 = Physics2D.OverlapPoint(new Vector2(Mathf.CeilToInt(self.transform.position.x),Mathf.CeilToInt(self.transform.position.y)), layerMask);

		if (barrierEnabled || (checkAbove1 == null && checkAbove2 == null)) {
			self.DeductAP (apCost);
			Animator doorAnimator = self.GetComponent<Animator> ();
			DoorBarrier doorBarrier = self.GetComponentInChildren<DoorBarrier> ();
			Debug.Log (doorBarrier);
			Animator barrierAnimator = doorBarrier.GetComponent<Animator> ();
			if (doorBarrier.barrierEnabled) {
				barrierAnimator.SetTrigger ("disableBarrierAnimation");
				yield return new WaitUntil (() => barrierAnimator.GetNextAnimatorStateInfo (0).IsName ("rest"));
				doorAnimator.SetTrigger ("disableDoorAnimation");
				yield return new WaitUntil (() => doorAnimator.GetNextAnimatorStateInfo (0).IsName ("rest"));
				self.gameObject.layer = 9;
				doorBarrier.barrierEnabled = false;
				self.attackable = false;
			} else {
				doorAnimator.SetTrigger ("enableDoorAnimation");
				yield return new WaitUntil (() => doorAnimator.GetNextAnimatorStateInfo (0).IsName ("enabled"));
				barrierAnimator.SetTrigger ("enableBarrierAnimation");
				yield return new WaitUntil (() => barrierAnimator.GetNextAnimatorStateInfo (0).IsName ("enabled"));
				self.gameObject.layer = 8;
				doorBarrier.barrierEnabled = true;
				self.attackable = true;
			}
		}
		gameManager.playerInput.TogglePlayerInputLock (false);
		gameManager.combatManager.ToggleActionLock (false);
	}
}
