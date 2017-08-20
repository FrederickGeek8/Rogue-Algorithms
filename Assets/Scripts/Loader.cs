using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

	public GameObject gameManager;

	// Use this for initialization
	void Awake () {
		Application.targetFrameRate = 10;
		if (GameManager.instance == null) {
			Instantiate (gameManager);
		}
	}
}
