﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; 

public class BoardManager : MonoBehaviour {
	[Serializable]
	public class Count {
		public int minimum;
		public int maximum;

		public Count (int min, int max) {
			minimum = min;
			maximum = max;
		}
	}

	public int columns = 8;
	public int rows = 8;
	public Count wallCount = new Count(5, 9);
	public Count foodCount = new Count(1, 5);
	public GameObject exit;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;

	private Transform boardHolder;
	private List <Vector3> gridPositions = new List<Vector3> ();
	[HideInInspector] public GameObject[] tiles;

	void InitialiseList() {
		gridPositions.Clear ();

		tiles = new GameObject[columns * rows];

		for (int x = 1; x < columns - 1; x++) {
			for (int y = 1; y < rows - 1; y++) {
				gridPositions.Add (new Vector3 (x, y, 0f));
			}
		}
	}

	void BoardSetup() {
		boardHolder = new GameObject ("Board").transform;

		for (int x = -1; x < columns + 1; x++) {
			for (int y = -1; y < rows + 1; y++) {
				GameObject toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];

				if (x == -1 || x == columns || y == -1 || y == rows) {
					toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
				}

				GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;

				instance.transform.SetParent (boardHolder);

				if (x != -1 && x != columns && y != -1 && y != rows) {
					tiles [y * rows + x] = instance;
				}

			}
		}
	}

	Vector3 RandomPosition() {
		int randomIndex = Random.Range (0, gridPositions.Count);
		Vector3 randomPosition = gridPositions [randomIndex];

		while (randomPosition == new Vector3 (columns - 1, rows - 1, 0F) || randomPosition == new Vector3 (0, 0, 0F)) {
			randomIndex = Random.Range (0, gridPositions.Count);
			randomPosition = gridPositions [randomIndex];
		}

		gridPositions.RemoveAt (randomIndex);

		return randomPosition;
	}

	void LayOutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum) {
		int objectCount = Random.Range (minimum, maximum + 1);

		for (int i = 0; i < objectCount; i++) {
			Vector3 randomPosition = RandomPosition ();

			GameObject tileChoice = tileArray [Random.Range (0, tileArray.Length)];
			GameObject instance = Instantiate (tileChoice, randomPosition, Quaternion.identity) as GameObject;
			tiles [(int) randomPosition.y * rows + (int) randomPosition.x] = instance;
		}
	}

	public void SetupScene (int level) {
		InitialiseList ();
		BoardSetup ();
		LayOutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
		LayOutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);
		int enemyCount = (int)Mathf.Log (level, 2f);
		LayOutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
		Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0F), Quaternion.identity);
	}
}
