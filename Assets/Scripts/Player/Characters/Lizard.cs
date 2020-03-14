using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script does anything to the player that is unique to the Lizard
public class Lizard : MonoBehaviour {

    Animator _linesAnimator;

    // Start is called before the first frame update
    void Start() {
        // Create a secondary sprite object for the outlines
        Object spritePrefab = Resources.Load("Prefabs/Entities/SecondarySprite");
        GameObject sprite = Instantiate(spritePrefab, transform) as GameObject;
        _linesAnimator = sprite.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        // Somehow keep the lines animator in sync with the main animator
    }
}
