using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaCreature : MonoBehaviour {
    public float moveSpeed;

    protected int species = 0; // 0-jellyfish, 1-octopus, 2-shark, 3-whale

    protected Animator _animator;

    protected void Awake() {
        _animator = GetComponentInChildren<Animator>();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
