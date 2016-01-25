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
	public bool buildObjectSelected = false;
	private bool excavating = false;
	private int cost;
	private int layerMask = (1 << 8) | (1 << 9) | (1 << 10);
	private bool rotated = false;
	public int neuralBuildCost;

	void Awake() {
		gameManager = GameManager.instance;
		buildGhost = Instantiate (buildGhostPrefab) as GameObject;
		buildGhost.SetActive (false);
		neuralBuildCost = 0;
	}

	public void ToggleBuildMode(bool turnOn) {
		if (turnOn) {
			gameManager.uiManager.ToggleBuildUI (true);
			buildModeEnabled = true;
			gameManager.DeselectObject ();
		} else {
			ClearBuildObject ();
			gameManager.uiManager.ToggleBuildUI (false);
			buildModeEnabled = false;
		}
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
		if (hitCollider != null && hitCollider.gameObject.tag == "Wall") {
			MegaWall megaWallHit = hitCollider.gameObject.transform.parent.GetComponent<MegaWall> ();
			GameObject[] hitWallNeighbors = (GameObject[])megaWallHit.neighbors.Clone ();
			foreach (GameObject neighbor in hitWallNeighbors) {
				if (neighbor == null) {
					return true;
				}
			}
		}
		return false;
	}

	public void ClearBuildObject() {
		buildObjectSelected = false;
		xOffset = 0;
		yOffset = 0;
		buildGhost.transform.localScale = new Vector3 (1, 1, 1);
		excavating = false;
		if (rotated)
			RotateBuildGhost ();
		prefabToBuild = null;
		buildGhost.SetActive (false);
		gameManager.uiManager.switchBuildMenusButton.gameObject.SetActive (false);
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
			cost = 20;
			break;
		case 2:
			//Door
			cost = 10;
			yOffset = 0.5f;
			gameManager.uiManager.switchBuildMenusButton.gameObject.SetActive (true);
			buildGhost.transform.localScale -= new Vector3 (0.2F, 0.2F, 0);
			break;
		case 3:
			//Lab Machine
			cost = 50;
			xOffset = 0.5f;
			yOffset = 0.5f;
			buildGhost.transform.localScale -= new Vector3 (0, 0.3F, 0);
			break;
		case 4:
			cost = 10;
			break;
		case 5:
			cost = neuralBuildCost;
			break;
		default:
			break;
		}
	}

	public void BuildObject() {
		if (buildObjectSelected && gameManager.boardManager.ValidateInsideBounds(buildGhost.transform.position) && gameManager.cash >= cost) {
			if (excavating) {
				if (ValidateExcavation (buildGhost.transform.position)) {
					Collider2D hitCollider = Physics2D.OverlapPoint (buildGhost.transform.position, layerMask);
					if (hitCollider != null && hitCollider.gameObject.tag == "Wall") {
						MegaWall megaWallHit = hitCollider.gameObject.transform.parent.GetComponent<MegaWall> ();
						GameObject[] hitWallNeighbors = (GameObject[]) megaWallHit.neighbors.Clone ();
						foreach (GameObject neighbor in hitWallNeighbors) {
							if (neighbor == null) {
								gameManager.soundManager.PlayBreakWallSFX ();
								hitCollider.gameObject.transform.parent.GetComponent<MegaWall> ().Kill ();
								gameManager.cash -= cost;
								gameManager.uiManager.UpdateCashText ();
								return;
							}
						}
						
					}
				}
			} else {
				if (ValidateBuildLocation (buildGhost.transform.position)) {
					if (prefabToBuild.name == "mine" || prefabToBuild.name == "barrierDoor")
						Instantiate (prefabToBuild, buildGhost.transform.position + new Vector3 (0, 0, 0.01f), buildGhost.transform.rotation);
					else
						Instantiate (prefabToBuild, buildGhost.transform.position + new Vector3 (0, 0, 0.01f), buildGhost.transform.rotation);
					gameManager.soundManager.PlayBuildMachineSFX ();
					gameManager.cash -= cost;
					gameManager.uiManager.UpdateCashText ();
				}
			}
		}
	}

	public void UpdateNeuralBuildCost() {
		neuralBuildCost = Mathf.FloorToInt(Mathf.Pow (10, FindObjectOfType<Leader> ().amplifiers.Count - 1));
		cost = neuralBuildCost;
		gameManager.uiManager.UpdateNeuralBuildButtonText (neuralBuildCost);
	}

	public void RotateBuildGhost() {
		if (rotated) {
			buildGhost.transform.Rotate (0, 0, -90);
			rotated = false;
		} else {
			buildGhost.transform.Rotate (0, 0, 90);
			rotated = true;
		}
		float temp = xOffset;
		xOffset = yOffset;
		yOffset = temp;
	}

	void Update() {
		if (buildModeEnabled && buildObjectSelected)
			RenderBuildGhost ();
	}
}
