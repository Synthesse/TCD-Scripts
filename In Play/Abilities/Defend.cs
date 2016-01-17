using UnityEngine;
using System.Collections;

public class Defend : Ability {

	public Defend() {
		apCost = 3;
		cooldown = 0;
		range = 666;
		friendlyTarget = true;
		abilityName = "Defend";
		abilityDescription = "Increases defense by 2";
		keyPress = "d";
		targetType = abilityTargetingTypes.Self;
	}

	public IEnumerator Execute(Unit self) {
		self.DeductAP (apCost);
		self.def += 2;
		self.UpdateObjectUIText ();
		yield return null;
	}
}
