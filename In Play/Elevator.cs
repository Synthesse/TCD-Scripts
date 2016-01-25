using UnityEngine;
using System.Collections;

public class Elevator : SelectableObject {

	void Start() {
		base.Start ();
		attackable = false;
		objectName = "Teleporter";
		special = "Invulnerable. Teleports enemy units every wave.";
	}
}
