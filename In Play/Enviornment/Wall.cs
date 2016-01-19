using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	private BoxCollider2D boxCollider;
	private corner megaWallCorner;
	private GameManager gameManager;
	public bool verticalNeighbor;
	public bool horizontalNeighbor;
	public bool diagonalNeighbor;

	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		boxCollider = GetComponent<BoxCollider2D> ();
		gameManager = GameManager.instance;
	}

	public void Initialize(corner setCorner) {
		megaWallCorner = setCorner;
		// Get Neighbors
		// Set 
	}

	public void SetSprite() {
		if (verticalNeighbor) {
			if (horizontalNeighbor) {
				if (diagonalNeighbor) {
					//WWW
					spriteRenderer.sprite = gameManager.boardManager.wallSprites [0];
				} else { 
					//WEW
					spriteRenderer.sprite = gameManager.boardManager.wallSprites [5 + (int)megaWallCorner];
				}
			} else {
				//W?E
				//spriteRenderer.sprite = gameManager.boardManager.wallSprites [Mathf.FloorToInt ((int)megaWallCorner / 2.0f) * 2 + 1];
				spriteRenderer.sprite = gameManager.boardManager.wallSprites [((int)megaWallCorner % 2) * 2 + 2];
			}
		} else if (horizontalNeighbor) {
			//E?W
			//spriteRenderer.sprite = gameManager.boardManager.wallSprites [((int)megaWallCorner % 2) * 2 + 2];
			spriteRenderer.sprite = gameManager.boardManager.wallSprites [Mathf.FloorToInt ((int)megaWallCorner / 2.0f) * 2 + 1];
		} else {
			//E?E
			spriteRenderer.sprite = gameManager.boardManager.wallSprites [9 + (int)megaWallCorner];
		}
	}
}
