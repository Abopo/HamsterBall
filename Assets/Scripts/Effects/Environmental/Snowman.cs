using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowman : MonoBehaviour {

    public int color;

    Animator _animator;

    private void Awake() {
        _animator = GetComponentInChildren<Animator>();
    }

    // Use this for initialization
    void Start () {
        _animator.SetInteger("Color", color);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
