using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterLineStop : MonoBehaviour {

    HamsterSpawner _hamsterSpawner;

	// Use this for initialization
	void Start () {
        _hamsterSpawner = transform.GetComponentInParent<HamsterSpawner>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Hamster") {
            other.GetComponent<Hamster>().inLine = true;
            _hamsterSpawner.HamsterLineStop();
        }
    }
}
