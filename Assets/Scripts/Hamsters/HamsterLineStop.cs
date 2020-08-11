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

    private void OnTriggerStay2D(Collider2D collision) {
        if(collision.tag == "Hamster") {
            Hamster ham = collision.GetComponent<Hamster>();
            // If for some reason we have a hamster in line that's touching but not in idle
            if (ham.CurState != 0 && ham.inLine) {
                // Stop the line
                _hamsterSpawner.HamsterLineStop();
            }
        }
    }
}
