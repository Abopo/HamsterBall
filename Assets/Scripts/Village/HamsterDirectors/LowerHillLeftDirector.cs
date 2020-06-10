using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerHillLeftDirector : HamsterDirector {

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
            case HAMSTERROOMS.SHOP:
            case HAMSTERROOMS.OPTIONS:
            case HAMSTERROOMS.MIDDLE:
            case HAMSTERROOMS.MUSHROOM:
                FaceHamsterRight();
                break;
            case HAMSTERROOMS.LEFT:
            case HAMSTERROOMS.NETWORK:
            case HAMSTERROOMS.STORY:
            case HAMSTERROOMS.VERSUS:
                FaceHamsterDown();
                break;
        }
    }

    void FaceHamsterRight() {
        // Hamster is heading right towards mushroom exit
        // So set the hamster to our y position
        _directingHamster.transform.position = new Vector3(transform.position.x,
                                                            transform.position.y,
                                                            _directingHamster.transform.position.z);

        // Then face the hamster right
        _directingHamster.FaceRight();

        // Platform is flat to the right so make sure the hamster rotation is flat
        transform.rotation = Quaternion.identity;
    }

    void FaceHamsterDown() {
        // Hamster is heading down the hills
        // So set the hamster to our y position
        _directingHamster.transform.position = new Vector3(transform.position.x,
                                                            transform.position.y,
                                                            _directingHamster.transform.position.z);

        // Then face the hamster left
        _directingHamster.FaceRight();

        // There's the rock steps down so rotate to match them
        _directingHamster.transform.Rotate(0f, 0f, -39f, Space.Self);
    }
}
