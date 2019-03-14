using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// These hamsters just stand still in the village doing a single animation.
// They will have some text that they will speak to the player when interacted with.
public class StandingHamster : MonoBehaviour {

    public HAMSTER_TYPES type;
    public string dialogue;
    public string startAnimation;

    Animator _animator;

	// Use this for initialization
	void Start () {
        _animator = GetComponentInChildren<Animator>();

        if (startAnimation != "") {
            _animator.Play(startAnimation);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
