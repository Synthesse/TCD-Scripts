using UnityEngine;
using System.Collections;

public class MegaWall : MonoBehaviour {

	public GameObject[] neighbors;
	private Wall upLeftWall;
	private Wall upRightWall;
	private Wall downLeftWall;
	private Wall downRightWall;
	private int layerMask = (1 << 8);
	private GameManager gameManager;

	void Awake() {
		gameManager = GameManager.instance;
		neighbors = new GameObject[8] {null, null, null, null, null, null, null, null};

		GameObject upLeftWallObj = Instantiate (gameManager.boardManager.wallPrefab, this.transform.position + new Vector3(-0.25f,0.25f,0), this.transform.rotation) as GameObject;
		upLeftWallObj.transform.SetParent (transform);
		upLeftWall = upLeftWallObj.GetComponent<Wall> ();
		upLeftWall.Initialize (corner.UpLeft);

		GameObject upRightWallObj = Instantiate (gameManager.boardManager.wallPrefab, this.transform.position + new Vector3(0.25f,0.25f,0), this.transform.rotation) as GameObject;
		upRightWallObj.transform.SetParent (transform);
		upRightWall = upRightWallObj.GetComponent<Wall> ();
		upRightWall.Initialize (corner.UpRight);

		GameObject downLeftWallObj = Instantiate (gameManager.boardManager.wallPrefab, this.transform.position + new Vector3(-0.25f,-0.25f,0), this.transform.rotation) as GameObject;
		downLeftWallObj.transform.SetParent (transform);
		downLeftWall = downLeftWallObj.GetComponent<Wall> ();
		downLeftWall.Initialize (corner.DownLeft);

		GameObject downRightWallObj = Instantiate (gameManager.boardManager.wallPrefab, this.transform.position + new Vector3(0.25f,-0.25f,0), this.transform.rotation) as GameObject;
		downRightWallObj.transform.SetParent (transform);
		downRightWall = downRightWallObj.GetComponent<Wall> ();
		downRightWall.Initialize (corner.DownRight);
	}

	// Use this for initialization
	void Start () {
	}

	public void Kill() {
		upLeftWall.gameObject.SetActive (false);
		upRightWall.gameObject.SetActive (false);
		downLeftWall.gameObject.SetActive (false);
		downRightWall.gameObject.SetActive (false);
		foreach (GameObject neighbor in neighbors) {
			if (neighbor != null) {
				MegaWall neighborMW = neighbor.GetComponent<MegaWall> ();
				if (neighborMW != null)
					neighborMW.FindNeighbors ();
			}
		}
		Destroy (gameObject);
	}

	public void FindNeighbors() {
		int ind = 0;
		for (int i = -1; i <= 1; i++) {
			for (int j = -1; j <= 1; j++) {
				if (i != 0 || j != 0) {
					//Debug.Log ("INDEX = " + ind.ToString ());
					Collider2D hitCollider = Physics2D.OverlapPoint (new Vector2(this.transform.position.x + i, this.transform.position.y + j), layerMask);
					if (hitCollider != null && (hitCollider.tag == "Wall" || hitCollider.tag == "Boundary")) {
						if (hitCollider.tag == "Wall") {
							neighbors [ind] = hitCollider.gameObject.transform.parent.gameObject;
						} else {
							neighbors [ind] = hitCollider.gameObject;
						}
					} else {
						neighbors [ind] = null;
					}
					ind++;
				}
			}
		}
		PropogateNeighborStatus ();
	}

	private void PropogateNeighborStatus() {
		for (int i = 0; i < 8; i++) {
			bool state;
			if (neighbors [i] == null) {
				state = false;
			} else {
				state = true;
			}

			if (i == 0) {
				downLeftWall.diagonalNeighbor = state;
			} else if (i == 1) {
				downLeftWall.horizontalNeighbor = state;
				upLeftWall.horizontalNeighbor = state;
			} else if (i == 2) {
				upLeftWall.diagonalNeighbor = state;
			} else if (i == 3) {
				downLeftWall.verticalNeighbor = state;
				downRightWall.verticalNeighbor = state;
			} else if (i == 4) {
				upLeftWall.verticalNeighbor = state;
				upRightWall.verticalNeighbor = state;
			} else if (i == 5) {
				downRightWall.diagonalNeighbor = state;
			} else if (i == 6) {
				downRightWall.horizontalNeighbor = state;
				upRightWall.horizontalNeighbor = state;
			} else if (i == 7) {
				upRightWall.diagonalNeighbor = state;
			} 
		}
		upLeftWall.SetSprite ();
		upRightWall.SetSprite ();
		downLeftWall.SetSprite ();
		downRightWall.SetSprite ();
	}

}
