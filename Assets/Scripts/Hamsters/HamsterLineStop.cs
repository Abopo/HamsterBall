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
        if (other.tag == "Hamster") {
            Hamster hamster = other.GetComponent<Hamster>();
            if (hamster.special) {
                _hamsterSpawner.ReleaseSpecificHamster(hamster);
            } else {
                hamster.inLine = true;
                _hamsterSpawner.HamsterLineStop();
            }
        }
        if (other.tag == "PowerUp") {
            other.GetComponent<PowerUp>().inLine = true;
            _hamsterSpawner.HamsterLineStop();
        }
    }
}
