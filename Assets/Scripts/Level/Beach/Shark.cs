using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : SeaCreature {

    protected new void Awake() {
        base.Awake();
    }
    // Use this for initialization
    protected new void Start() {
        base.Start();

        species = 2;
        _animator.SetInteger("Species", species);
    }

    // Update is called once per frame
    void Update() {
        transform.Translate(moveSpeed * Time.deltaTime, 0f, 0f);
    }
}
