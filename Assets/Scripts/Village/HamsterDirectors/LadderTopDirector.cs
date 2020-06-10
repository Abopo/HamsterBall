using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderTopDirector : HamsterDirector {

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    protected override void DirectHamster(WanderingHamster wHamster) {
        base.DirectHamster(wHamster);

        // Based on the hamster room, direct the hamster
        switch(wHamster.targetRoom.room) {
            case HAMSTERROOMS.CHARACTER:
                // Face hamster left
                FaceHamsterLeft();
                break;
            case HAMSTERROOMS.LEFT:
            case HAMSTERROOMS.MIDDLE:
            case HAMSTERROOMS.MUSHROOM:
            case HAMSTERROOMS.NETWORK:
            case HAMSTERROOMS.OPTIONS:
            case HAMSTERROOMS.SHOP:
            case HAMSTERROOMS.STORY:
            case HAMSTERROOMS.VERSUS:
                // Face hamster down
                FaceHamsterDown();
                break;
        }
    }

    void FaceHamsterLeft() {
        // Hamster is heading left offscreen
        // So set the hamster to our y position
        _directingHamster.transform.position = new Vector3(_directingHamster.transform.position.x,
                                                           transform.position.y,
                                                           _directingHamster.transform.position.z);

        // Then face the hamster left
        _directingHamster.FaceLeft();
    }

    void FaceHamsterDown() {
        // Hamster is heading down the ladder
        // So set the hamster to our x position
        _directingHamster.transform.position = new Vector3(transform.position.x,
                                                           _directingHamster.transform.position.y,
                                                           _directingHamster.transform.position.z);

        // Then face the hamster down
        _directingHamster.FaceDown();
    }
}
