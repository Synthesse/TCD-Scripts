using UnityEngine;
using System.Collections;

//SWITCH TO THIS ONCE WE FULLY IMPLEMENT INTERFACE REQUIREMENTS
//public class PhysicalObject : MonoBehaviour, ILocateable {
public abstract class PhysicalObject : MonoBehaviour {

	protected BoxCollider2D boxCollider;
	protected Vector2 location;
	protected GameManager gameManager;

	// Use this for initialization
	protected virtual void Start () {
		gameManager = GameManager.instance;
		boxCollider = GetComponent<BoxCollider2D>();
	}

	protected Vector2 GridLocate () {
		return (new Vector2 (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.y)));
	}

	protected Vector2[] GridLocate (bool giveCorners) {
		// TODO: Make this work for objects which are larger than one square
		Vector2[] gridLocations = new Vector2[4];
		return (gridLocations);
	}

}
