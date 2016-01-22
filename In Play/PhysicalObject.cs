using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	public List<Vector2> NearestOpenSpaces(Vector2 start) {
	//TODO: technically works for 2x1 and 2x2, really hacky
		start = new Vector2(Mathf.FloorToInt(start.x), Mathf.FloorToInt(start.y));
		int layerMask = 1 << 8;
		Dictionary<float,Vector2> locationReference = new Dictionary<float,Vector2>();
		List<float> sortingList = new List<float> ();
		List<Vector2> locList = new List<Vector2> ();

		for (int i = -1; i <= 1; i++) {
			for (int j = -1; j <= 1; j++) {
				if (i != 0 || j != 0) {
					Vector2 edgePosition = (Vector2)this.transform.position + new Vector2 (i, j);
					if (Physics2D.OverlapPoint (edgePosition, layerMask) == null) {
						float dist = Vector2.Distance (start, edgePosition)+(3*i+j)/1000f;
						sortingList.Add (dist);
						locationReference.Add (dist, edgePosition);
					}
				}
			}
		}
		sortingList.Sort ();
		foreach (float sortedDist in sortingList) {
			locList.Add (locationReference [sortedDist]);
		}
		return locList;
	}
}
