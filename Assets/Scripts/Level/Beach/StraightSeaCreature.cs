using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightSeaCreature : SeaCreature {

    protected new void Awake() {
        base.Awake();
    }
    // Use this for initialization
    protected new void Start() {
        base.Start();
    }

    // Update is called once per frame
    void Update() {
        transform.Translate(moveSpeed * Time.deltaTime, 0f, 0f);
    }
}
