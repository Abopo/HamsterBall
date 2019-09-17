﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaCreature : MonoBehaviour {
    public float moveSpeed;
    public bool left;

    public int species = 0; // 0-jellyfish, 1-octopus, 2-shark, 3-whale

    protected Animator _animator;

    protected void Awake() {
        _animator = GetComponentInChildren<Animator>();
    }
    // Use this for initialization
    protected void Start () {
        _animator.SetInteger("Species", species);

		if(!left) {
            moveSpeed = -Mathf.Abs(moveSpeed);
        } else {
            Flip();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    protected virtual void Flip() {
        // Face the other way
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
