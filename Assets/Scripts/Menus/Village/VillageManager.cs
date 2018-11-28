using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageManager : MonoBehaviour {

    GameManager _gameManager;

	// Use this for initialization
	void Start () {
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.isSinglePlayer = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
