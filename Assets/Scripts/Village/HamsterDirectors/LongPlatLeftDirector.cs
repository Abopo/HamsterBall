using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongPlatLeftDirector : HamsterDirector {

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    protected override void DirectHamster(WanderingHamster wHamster) {
        base.DirectHamster(wHamster);

        // Based on the hamster room, direct the hamster
        switch (wHamster.targetRoom.room) {
            case HAMSTERROOMS.CHARACTER:
            case HAMSTERROOMS.OPTIONS:
            case HAMSTERROOMS.SHOP:
                FaceHamsterRight();
                break;
            case HAMSTERROOMS.LEFT:
            case HAMSTERROOMS.MIDDLE:
            case HAMSTERROOMS.MUSHROOM:
            case HAMSTERROOMS.NETWORK:
            case HAMSTERROOMS.STORY:
            case HAMSTERROOMS.VERSUS:
                FaceHamsterLeft();
                break;
        }
    }

    void FaceHamsterLeft() {
        // Hamster is heading left towards network
        // So set the hamster to our y position
        _directingHamster.transform.position = new Vector3(_directingHamster.transform.position.x,
                                                           transform.position.y,
                                                           _directingHamster.transform.position.z);

        // Then face the hamster left
        _directingHamster.FaceLeft();

        // There's a rope on the left so rotate the hamster properly
        _directingHamster.transform.Rotate(0f, 0f, 31f, Space.Self);
    }

    void FaceHamsterRight() {
        // Hamster is heading right towards shop
        // So set the hamster to our y position
        _directingHamster.transform.position = new Vector3(_directingHamster.transform.position.x,
                                                            transform.position.y,
                                                            _directingHamster.transform.position.z);

        // Then face the hamster down
        _directingHamster.FaceRight();

        // Platform is flat to the right so make sure the hamster rotation is flat
        transform.rotation = Quaternion.identity;
    }
}
