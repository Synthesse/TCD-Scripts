using UnityEngine;
using System.Collections;

public enum abilityTargetingTypes {None,Self,One,Many,Line,Area};

public abstract class Ability {

	//TODO: add auto-decrement all ability cooldowns to turn start

	protected abilityTargetingTypes targetType;
	public int apCost;
	protected int cooldown;
	public int range;
	public bool friendlyTarget;
	public string abilityName;
	protected string abilityDescription;
	public string abilityButtonText;
	protected string keyPress;
	protected int currentCooldown;
	public string alternateAbilityButtonText;

	protected GameManager gameManager;

	public Ability() {
		apCost = 1;
		cooldown = 0;
		range = 666;
		friendlyTarget = false;
		abilityName = "Generic Ability";
		abilityDescription = "Description?";
		abilityButtonText = "PLACEHOLDER";
		alternateAbilityButtonText = "PLACEHOLDER";
		keyPress = "a";
		targetType = abilityTargetingTypes.None;
		gameManager = GameManager.instance;
	}

	//For use with targetTypes: None
	public virtual IEnumerator Execute() {
		yield return null;
	}

	//For use with targetTypes: Self

	public virtual IEnumerator Execute(Unit self) {
		self.DeductAP (apCost);
		yield return null;
	}

	public virtual IEnumerator Execute(Defenses self) {
		self.DeductAP (apCost);
		yield return null;
	}

	//For use with targetTypes: One

	public virtual IEnumerator Execute(Unit self, GameObject target) {
		self.DeductAP (apCost);
		yield return null;
	}

	public virtual IEnumerator Execute(Defenses self, GameObject target) {
		self.DeductAP (apCost);
		yield return null;
	}


}
