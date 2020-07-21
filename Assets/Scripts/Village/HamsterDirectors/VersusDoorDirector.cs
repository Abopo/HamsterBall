using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersusDoorDirector : HamsterDirector {

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
            case HAMSTERROOMS.VERSUS:
                FaceHamsterUp();
                break;
            case HAMSTERROOMS.LEFT:
            case HAMSTERROOMS.NETWORK:
                FaceHamsterLeft();
                break;
            case HAMSTERROOMS.STORY:
            case HAMSTERROOMS.CHARACTER:
            case HAMSTERROOMS.SHOP:
            case HAMSTERROOMS.OPTIONS:
            case HAMSTERROOMS.MIDDLE:
            case HAMSTERROOMS.MUSHROOM:
                FaceHamsterRight();
                break;
        }
    }

    void FaceHamsterLeft() {
        // Hamster is heading left towards center
        // So set the hamster to our y position
        _directingHamster.transform.position = new Vector3(_directingHamster.transform.position.x,
                                                           transform.position.y,
                                                           _directingHamster.transform.position.z);

        // Then face the hamster left
        _directingHamster.FaceLeft();

        // Ground is flat to the Left so make sure the hamster rotation is flat
        transform.rotation = Quaternion.identity;
    }

    void FaceHamsterRight() {
        // Hamster is heading right towards mushroom exit
        // So set the hamster to our y position
        _directingHamster.transform.position = new Vector3(transform.position.x,
                                                            transform.position.y,
                                                            _directingHamster.transform.position.z);

        // Then face the hamster right
        _directingHamster.FaceRight();

        // Ground is flat to the right so make sure the hamster rotation is flat
        transform.rotation = Quaternion.identity;
    }

    void FaceHamsterUp() {
        // Hamster is heading up the hills
        // So set the hamster to our y position
        _directingHamster.transform.position = new Vector3(transform.position.x,
                                                            transform.position.y,
                                                            _directingHamster.transform.position.z);

        // Then face the hamster left
        _directingHamster.FaceRight();

        // There's the rock steps down so rotate to match them
        _directingHamster.transform.Rotate(0f, 0f, 32.5f, Space.Self);
    }
}