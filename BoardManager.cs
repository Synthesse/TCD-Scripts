using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum direction {Up, Right, Down, Left, None};
public enum direction8 {Up, Right, Down, Left, UpLeft, UpRight, DownLeft, DownRight, None};
public enum corner {UpLeft, UpRight, DownLeft, DownRight};

public class BoardManager : MonoBehaviour {

	[Serializable] public class Count {
		public int minimum;
		public int maximum;

		public Count (int min, int max) {
			minimum = min;
			maximum = max;
		}
	}

	public int columns;
	public int rows;
	public int bufferSize = 2;
	public GameObject enemySpawnPoint;
	public GameObject player;
	public GameObject[] enemyTiles;
	public GameObject[] floorTiles;
	public GameObject[] indestructibleWallTiles;
	[HideInInspector] public Transform wallHolder;
	public GameObject[,] combatBlockingArray;

	public Sprite[] wallSprites;
	public GameObject megaWallPrefab;
	public GameObject wallPrefab;
	public GameObject warpPoint;

	private int layerMask = 1 << 8;
	private Transform boardHolder;
	private List <Vector2> gridPositions = new List<Vector2> ();


	void InitializeList() {
		combatBlockingArray = new GameObject[columns,rows];
		gridPositions.Clear ();

		for (int x = 0; x < columns; x++) {
			for (int y = 0; y < rows; y++) {
				gridPositions.Add (new Vector2 (x, y));
			}
		}
	}

	void BoardSetup (bool fixedStart) {
		if (fixedStart) {

		} else {
//			for (int x = (int)randomPosition.x - bufferSize; x < (int)randomPosition.x + bufferSize + 1; x++) {
//				for (int y = (int)randomPosition.y - bufferSize; y < (int)randomPosition.y + bufferSize + 1; y++) {
//					gridPositions.Remove (new Vector2 (x, y));
//				}
//			}

			Vector2 spawnLocation = new Vector2 (Random.Range (bufferSize+1, columns - (bufferSize+2)), Random.Range (bufferSize, rows - (bufferSize+2)));
			boardHolder = new GameObject ("Board").transform;
			wallHolder = new GameObject ("Walls").transform;
			for (int x = -1; x < columns + 1; x++) {
				for (int y = -1; y < rows + 1; y++) {
					GameObject toInstantiate;
					if (x == -1 || x == columns || y == -1 || y == rows)
						toInstantiate = indestructibleWallTiles [0];
					else if (x <= spawnLocation.x + (bufferSize+1) && x >= spawnLocation.x - (bufferSize+1) && y <= spawnLocation.y + (bufferSize+1) && y >= spawnLocation.y - (bufferSize+1)) {
						gridPositions.Remove (new Vector2 (x, y));
						if (x <= spawnLocation.x + bufferSize && x >= spawnLocation.x - bufferSize && y <= spawnLocation.y + bufferSize && y >= spawnLocation.y - bufferSize)
							toInstantiate = warpPoint;
						else 
							toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
					} else 
						toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
					

					GameObject instance = Instantiate (toInstantiate, new Vector2 (x, y), Quaternion.identity) as GameObject;
					instance.transform.SetParent (boardHolder);
				}
			}
			GameObject objInstance = Instantiate (enemySpawnPoint, spawnLocation, Quaternion.identity) as GameObject;
			objInstance.name = "elevator";
			Instantiate (player, spawnLocation - new Vector2(1,1), Quaternion.identity);
			//Camera.main.transform.position = new Vector3 (spawnLocation.x, spawnLocation.y, -20);
		}
	}

	Vector2 RandomPosition(bool requiresBuffer) {
		int randomIndex = Random.Range (0, gridPositions.Count);
		Vector2 randomPosition = gridPositions [randomIndex];
		gridPositions.RemoveAt (randomIndex);
		if (requiresBuffer)
			for (int x = (int)randomPosition.x - bufferSize; x < (int)randomPosition.x + bufferSize + 1; x++) {
				for (int y = (int)randomPosition.y - bufferSize; y < (int)randomPosition.y + bufferSize + 1; y++) {
					gridPositions.Remove (new Vector2 (x, y));
				}
			}
		return randomPosition;
	}

	private GameObject LayoutObjectAtRandom (GameObject objectTile, bool requiresBuffer, bool addToCombatArray) {
		Vector2 randomPosition = RandomPosition (requiresBuffer);
		GameObject objInstance = Instantiate (objectTile, randomPosition, Quaternion.identity) as GameObject;
		if (addToCombatArray) 
			combatBlockingArray [Mathf.RoundToInt(randomPosition.x), Mathf.RoundToInt(randomPosition.y)] = objInstance;
		return objInstance;	
	}

	void LayoutObjectsAtRandom (GameObject[] tileArray, int minimum, int maximum, bool requiresBuffer, bool addToCombatArray) {
		int objectCount = Random.Range (minimum, maximum + 1);
		for (int i = 0; i < objectCount; i++) {
			Vector2 randomPosition = RandomPosition (requiresBuffer);
			GameObject tileChoice = tileArray [Random.Range (0, tileArray.Length)];
			GameObject objInstance = Instantiate (tileChoice, randomPosition, Quaternion.identity) as GameObject;
			if (addToCombatArray) 
				combatBlockingArray [Mathf.RoundToInt(randomPosition.x), Mathf.RoundToInt(randomPosition.y)] = objInstance;
		}
	}

//	void FillRemainingSpaceWithWalls () {
//		for (int i = 0; i < gridPositions.Count; i++) {
//			if (Random.value > 0.7) {
//				GameObject tileChoice = wallTiles [Random.Range (0, wallTiles.Length)];
//				GameObject instance = Instantiate (tileChoice, gridPositions [i], Quaternion.identity) as GameObject;
//				instance.transform.SetParent (wallHolder);
//				combatBlockingArray [Mathf.RoundToInt(gridPositions [i].x), Mathf.RoundToInt(gridPositions [i].y)] = instance;
//			}
//		}
//		gridPositions.Clear ();
//	}

	void FillRemainingSpaceWithWalls () {
		for (int i = 0; i < gridPositions.Count; i++) {
			if (Random.value >= 0) {
				GameObject instance = Instantiate (megaWallPrefab, gridPositions [i], Quaternion.identity) as GameObject;
				instance.transform.SetParent (wallHolder);
				combatBlockingArray [Mathf.RoundToInt(gridPositions [i].x), Mathf.RoundToInt(gridPositions [i].y)] = instance;
			}
		}
		gridPositions.Clear ();
	}

	public bool ValidateInsideBounds(Vector2 vec2) {
		return (vec2.x < columns && vec2.y < rows && vec2.x >= 0 && vec2.y >= 0);
	}

	public void DefaultWaveSpawn(int maxNumSpawns) {
		SpawnObjectsAroundObject (GameObject.Find("elevator"), bufferSize, maxNumSpawns, enemyTiles);
	}

	private void SpawnObjectsAroundObject(GameObject objectToSpawnAround, int maxRadius, int maxNumSpawns, GameObject[] tileArray) {
		int numSpawns = 0;
		int locusX = Mathf.RoundToInt (objectToSpawnAround.transform.position.x);
		int locusY = Mathf.RoundToInt (objectToSpawnAround.transform.position.y);
		for (int i = 1; i <= maxRadius; i++) {
			for (int j = -i; j <= i; j++) {
				for (int k = -i; k <= i; k++) {
					Vector2 placeLocation = new Vector2 (locusX + j, locusY + k);
					if (numSpawns < maxNumSpawns && ValidateInsideBounds (placeLocation)) {
						Collider2D hitCollider = Physics2D.OverlapPoint (placeLocation, layerMask);
						if (hitCollider == null) {
							GameObject tileChoice = tileArray [Random.Range (0, tileArray.Length)];
							GameObject objInstance = Instantiate (tileChoice, placeLocation, Quaternion.identity) as GameObject;
							numSpawns++;

						}
					}
				}
			}
		}
	}

	public void SetupScene() {
		InitializeList ();
		BoardSetup (false);
		//FillRemainingSpaceWithWalls ();
		for (int i = 0; i < wallHolder.childCount; i++) {
			wallHolder.GetChild (i).GetComponent<MegaWall> ().FindNeighbors ();
		}
	}

	public float FindAngle(Vector2 start, Vector2 end) {
		int startX = Mathf.RoundToInt (start.x);
		int startY = Mathf.RoundToInt (start.y);
		int endX = Mathf.RoundToInt (end.x);
		int endY = Mathf.RoundToInt (end.y);
		float directionAngle = Mathf.Atan2 (endY - startY, endX - startX);
		return directionAngle;
	}

	public direction FindDirection (Vector2 start, Vector2 end) {
		float directionAngle = FindAngle (start, end);
		//Note: atan goes from -pi to pi
		if (directionAngle < Mathf.PI*-3f/4f || directionAngle > Mathf.PI*3f/4f )
			return direction.Left;
		else if (directionAngle <= Mathf.PI*-1f/4f)
			return direction.Down;
		else if (directionAngle < Mathf.PI*1f/4f)
			return direction.Right;
		else
			return direction.Up;
	}

	public direction8 FindDirection8 (Vector2 start, Vector2 end) {
		float directionAngle = FindAngle (start, end);
		//Note: atan goes from -pi to pi
		if (directionAngle <= Mathf.PI*-7f/8f || directionAngle >= Mathf.PI*7f/8f )
			return direction8.Left;
		else if (directionAngle < Mathf.PI*-5f/8f)
			return direction8.DownLeft;
		else if (directionAngle <= Mathf.PI*-3f/8f)
			return direction8.Down;
		else if (directionAngle < Mathf.PI*-1f/8f)
			return direction8.DownRight;
		else if (directionAngle <= Mathf.PI*1f/8f)
			return direction8.Right;
		else if (directionAngle < Mathf.PI*3f/8f)
			return direction8.UpRight;
		else if (directionAngle <= Mathf.PI*5f/8f)
			return direction8.Up;
		else
			return direction8.UpLeft;
	} 
}
