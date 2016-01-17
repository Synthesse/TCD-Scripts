using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BuildStateManager : MonoBehaviour {

	public GameManager gameManager;
	public bool buildModeEnabled = false;
	public GameObject buildGhostPrefab;
	public List<Sprite> buildableSprites;
	public List<GameObject> buildableObjects;
	private GameObject prefabToBuild;
	private GameObject buildGhost;
	private float xOffset = 0;
	private float yOffset = 0;
	private bool buildObjectSelected = false;
	private bool excavating = false;
	private int cost;
	private int layerMask = (1 << 8) | (1 << 9) ;

	void Start() {
		gameManager = GameManager.instance;
		buildGhost = Instantiate (buildGhostPrefab) as GameObject;
		buildGhost.SetActive (false);
	}

	private void RenderBuildGhost() {
		Vector3 mousePoint = gameManager.playerInput.GetMouseGridPosition ();
		if (mousePoint != gameManager.playerInput.currentMouseGridLoc && gameManager.boardManager.ValidateInsideBounds((Vector2) mousePoint)) {
			if (!buildGhost.activeSelf)
				buildGhost.SetActive (true);
			gameManager.playerInput.currentMouseGridLoc = mousePoint;
			buildGhost.transform.position = new Vector3 (mousePoint.x + xOffset, mousePoint.y + yOffset);
			if (excavating) {
				if (ValidateExcavation (buildGhost.transform.position)) 
					buildGhost.GetComponent<SpriteRenderer>().color = new Color (0.2f,0.2f,1f,0.7f);
				else
					buildGhost.GetComponent<SpriteRenderer>().color = new Color (1f,0.2f,0.2f,0.4f);

			} else {
				if (ValidateBuildLocation (buildGhost.transform.position))
					buildGhost.GetComponent<SpriteRenderer>().color = new Color (0.2f,0.2f,1f,0.7f);
				else
					buildGhost.GetComponent<SpriteRenderer>().color = new Color (1f,0.2f,0.2f,0.4f);
			}
		}
	}

	private bool ValidateBuildLocation(Vector2 location) {
		List<Vector2> locationsList = new List<Vector2> ();
		locationsList.Add (new Vector2 (Mathf.CeilToInt (location.x), Mathf.CeilToInt (location.y)));
		locationsList.Add (new Vector2 (Mathf.CeilToInt (location.x), Mathf.FloorToInt (location.y)));
		locationsList.Add (new Vector2 (Mathf.FloorToInt (location.x), Mathf.CeilToInt (location.y)));
		locationsList.Add (new Vector2 (Mathf.FloorToInt (location.x), Mathf.FloorToInt (location.y)));
		locationsList = locationsList.Distinct ().ToList ();
		foreach (Vector2 loc in locationsList) {
			Collider2D hitCollider = Physics2D.OverlapPoint (loc, layerMask);
			if (hitCollider != null) {
				return false;
			}
		}
		return true;
	}

	private bool ValidateExcavation(Vector2 location) {
		Collider2D hitCollider = Physics2D.OverlapPoint (location, layerMask);
		if (hitCollider != null && hitCollider.gameObject.tag == "Wall")
			return true;
		return false;
	}

	private void ClearBuildObject() {
		buildObjectSelected = false;
		xOffset = 0;
		yOffset = 0;
		buildGhost.transform.localScale = new Vector3 (1, 1, 1);
		excavating = false;
		prefabToBuild = null;
	}

	public virtual void ProcessBuildPanelClick(int buttonNum) {
		if (buildObjectSelected)
			ClearBuildObject ();
		buildObjectSelected = true;
		buildGhost.GetComponent<SpriteRenderer> ().sprite = buildableSprites [buttonNum];

		if (buttonNum > 0) {
			prefabToBuild = buildableObjects [buttonNum-1];
		}
		switch (buttonNum) {
		case 0:
			//Floor
			excavating = true;
			cost = 1;
			break;
		case 1:
			//Turret
			cost = 2;
			break;
		case 2:
			//Door
			cost = 3;
			yOffset = 0.5f;
			buildGhost.transform.localScale -= new Vector3 (0.2F, 0.2F, 0);
			break;
		case 3:
			//Lab Machine
			cost = 5;
			xOffset = 0.5f;
			yOffset = 0.5f;
			buildGhost.transform.localScale -= new Vector3 (0, 0.3F, 0);
			break;
		default:
			break;
		}
	}

	public void BuildObject() {

	}

	void Update() {
		if (buildModeEnabled && buildObjectSelected)
			RenderBuildGhost ();
	}
}
