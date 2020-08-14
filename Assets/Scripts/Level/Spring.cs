﻿using UnityEngine;
using System.Collections;

// This is a level object that makes entities bounce up high when stepped on.
public class Spring : MonoBehaviour {
    public float springPower = 10;

    Animator _animator;

    private void Awake() {
        _animator = GetComponentInChildren<Animator>();
    }
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D collider) {
        // If collided with an entity
        Entity obj = collider.GetComponent<Entity>();
        if (obj != null) {
            // Set entity pos to spring pos to make sure bounces are consistent
            obj.transform.position = new Vector3(transform.position.x, obj.transform.position.y, obj.transform.position.z);
            // Bounce up the entity
            obj.Spring(springPower);

            // Play spring animation
            _animator.Play("Spring");
            FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.VillageSpring);

            // Play bounce sound
        }
    }
}
