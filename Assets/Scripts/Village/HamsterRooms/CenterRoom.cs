using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterRoom : HamsterRoom {
    // Start is called before the first frame update
    void Start() {

    }

    protected override void DirectHamster() {
        base.DirectHamster();

        switch (_containedHamsters[0].targetRoom.room) {
            case HAMSTERROOMS.CHARACTER:
            case HAMSTERROOMS.MIDDLE:
            case HAMSTERROOMS.OPTIONS:
            case HAMSTERROOMS.SHOP:
                FaceHamsterLeft();
                break;
            case HAMSTERROOMS.MUSHROOM:
            case HAMSTERROOMS.LEFT:
            case HAMSTERROOMS.NETWORK:
            case HAMSTERROOMS.VERSUS:
            case HAMSTERROOMS.STORY:
                FaceHamsterRight();
                break;
        }
    }
}
