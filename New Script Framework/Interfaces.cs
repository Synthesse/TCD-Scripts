using UnityEngine;
using System.Collections;

public interface IKillable {
	// Used for anything that can be destroyed
	void Kill();
	void KO();
}

public interface IDamageable {
	void Damage (int damageTaken);
	void ResetDamage ();
}

public interface IDisableable {
	void Disable (int disableStrength);
}

public interface IHinderable {
	void Hinder (int hinderStrength);
}

public interface IFlipable {
	void Flip (bool isNowEnemy);
}

public interface IInteractable {
	void Interact (GameObject interactor);
}

public interface IAbleToMove {
	void Move (Vector3 endLocation);
	void Slow (int slowStrength);
}

public interface ILocateable {
	Vector2 LocateCenter ();
	Vector2[] GridLocate ();
}

public interface ISelectable {
	void Select ();
	void Deselect ();
	void Target ();
	void Untarget ();
}

public interface IUnit : IDamageable, IDisableable, IHinderable, IFlipable, IAbleToMove {
}

public interface IMachine : IDamageable, IDisableable, IHinderable, IInteractable {
}