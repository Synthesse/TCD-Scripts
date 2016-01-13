﻿using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	[Serializable] public class Count {
		public int minimum;
		public int maximum;

		public Count (int min, int max) {
			minimum = min;
			maximum = max;
		}
	}

	public int columns = 20;
	public int rows = 10;
	public int bufferSize = 2;
	public GameObject enemySpawnPoint;
	public GameObject player;
	public GameObject[] enemyTiles;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] indestructibleWallTiles;
	[HideInInspector] public Transform wallHolder;
	public GameObject[,] combatBlockingArray;

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

	void BoardSetup () {
		boardHolder = new GameObject ("Board").transform;
		wallHolder = new GameObject ("Walls").transform;
		for (int x = -1; x < columns + 1; x++) {
			for (int y = -1; y < rows + 1; y++) {
				GameObject toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
				if (x == -1 || x == columns || y == -1 || y == rows)
					toInstantiate = indestructibleWallTiles [0];

				GameObject instance = Instantiate (toInstantiate, new Vector2 (x, y), Quaternion.identity) as GameObject;

				instance.transform.SetParent (boardHolder);
			}
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

	void FillRemainingSpaceWithWalls () {
		for (int i = 0; i < gridPositions.Count; i++) {
			if (Random.value > 0.7) {
				GameObject tileChoice = wallTiles [Random.Range (0, wallTiles.Length)];
				GameObject instance = Instantiate (tileChoice, gridPositions [i], Quaternion.identity) as GameObject;
				instance.transform.SetParent (wallHolder);
				combatBlockingArray [Mathf.RoundToInt(gridPositions [i].x), Mathf.RoundToInt(gridPositions [i].y)] = instance;
			}
		}
		gridPositions.Clear ();
	}

	private bool ValidateInsideBounds(Vector2 vec2) {
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
		BoardSetup ();
		InitializeList ();
		//LayoutObjectsAtRandom (enemyTiles, 1, 1, false, true);
		GameObject enemySpawn = LayoutObjectAtRandom (enemySpawnPoint, true, true);
		enemySpawn.name = "elevator";
		LayoutObjectAtRandom (player, false, true);
		FillRemainingSpaceWithWalls ();
	}
}
