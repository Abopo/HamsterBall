using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour {

    public int grassType;

    Animator _animator;

	// Use this for initialization
	void Start () {
        _animator = GetComponentInChildren<Animator>();

        _animator.SetInteger("Type", grassType);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
