using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seaweed : MonoBehaviour {
    public int type;

    Animator _animator;

    // Start is called before the first frame update
    void Start() {
        _animator = GetComponent<Animator>();
        _animator.SetInteger("Type", type);
    }

    // Update is called once per frame
    void Update() {
        
    }
}
