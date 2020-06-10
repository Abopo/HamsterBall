﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderBottomDirector : HamsterDirector {

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
                // Face hamster left
                FaceHamsterUp();
                break;
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
                // Face hamster down
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
    }

    void FaceHamsterRight() {
        // Hamster is heading right towards shop
        // So set the hamster to our y position
        _directingHamster.transform.position = new Vector3(_directingHamster.transform.position.x,
                                                            transform.position.y,
                                                            _directingHamster.transform.position.z);

        // Then face the hamster down
        _directingHamster.FaceRight();
    }

    void FaceHamsterUp() {
        // Hamster is heading up to characters
        // So set the hamster to our y position
        _directingHamster.transform.position = new Vector3(transform.position.x,
                                                            _directingHamster.transform.position.y,
                                                            _directingHamster.transform.position.z);

        // First face the hamster left, then up
        // This makes sure their feet are against the ladder
        _directingHamster.FaceLeft();
        _directingHamster.FaceUp();
    }
}
