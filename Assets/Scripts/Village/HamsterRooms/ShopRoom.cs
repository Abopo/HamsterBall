using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRoom : HamsterRoom { 
    // Start is called before the first frame update
    void Start() {
        
    }

    protected override void DirectHamster() {
        base.DirectHamster();

        switch (_containedHamsters[0].targetRoom.room) {
            case HAMSTERROOMS.CHARACTER:
            case HAMSTERROOMS.MIDDLE:
            case HAMSTERROOMS.SHOP:
            case HAMSTERROOMS.MUSHROOM:
            case HAMSTERROOMS.LEFT:
            case HAMSTERROOMS.NETWORK:
            case HAMSTERROOMS.VERSUS:
            case HAMSTERROOMS.STORY:
                FaceHamsterLeft();
                break;
            case HAMSTERROOMS.OPTIONS:
                FaceHamsterRight();
                break;
        }
    }
}
