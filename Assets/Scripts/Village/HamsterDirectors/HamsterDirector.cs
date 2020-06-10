using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Directs wandering hamsters towards HamsterRooms
// These are placed at each junction in the village
public class HamsterDirector : MonoBehaviour {
    protected WanderingHamster _directingHamster;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    protected void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Hamster") {
            DirectHamster(collision.GetComponent<WanderingHamster>());
        }
    }

    protected virtual void DirectHamster(WanderingHamster wHamster) {
        _directingHamster = wHamster;
    }
}
