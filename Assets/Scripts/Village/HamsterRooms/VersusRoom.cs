using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersusRoom : HamsterRoom {
    // Start is called before the first frame update
    void Start() {

    }

    protected override void DirectHamster() {
        base.DirectHamster();

        switch (_containedHamsters[0].targetRoom.room) {
            case HAMSTERROOMS.LEFT:
            case HAMSTERROOMS.NETWORK:
                FaceHamsterLeft();
                break;
            case HAMSTERROOMS.CHARACTER:
            case HAMSTERROOMS.MIDDLE:
            case HAMSTERROOMS.OPTIONS:
            case HAMSTERROOMS.SHOP:
            case HAMSTERROOMS.MUSHROOM:
            case HAMSTERROOMS.VERSUS:
            case HAMSTERROOMS.STORY:
                FaceHamsterRight();
                break;
        }
    }
}
