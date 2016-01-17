using UnityEngine;
using System.Collections;

public class Soldier : Unit {

	protected override void Awake() {
		currentHP = 10;
		maxHP = 10;
		currentAP = 4;
		maxAP = 4;
		atk = 4;
		def = 1;
		objectName = "Sans";
	}
}
