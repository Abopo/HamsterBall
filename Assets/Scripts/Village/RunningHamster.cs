using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// These hamsters will run back and forth between two points
// They may or may not have dialogue text
public class RunningHamster : VillageHamster {

    public HAMSTER_TYPES type;

    public Transform[] boundries = new Transform[2];

    Animator _animator;

    float moveSpeed = 3;

    // Use this for initialization
    protected override void Start() {
        base.Start();

        _animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update() {

        // TODO: If not being talked to?
        transform.Translate(moveSpeed * Time.deltaTime, 0f, 0f);

        if(transform.position.x < boundries[0].position.x) {
            RunRight();
        }
        if(transform.position.x > boundries[1].position.x) {
            RunLeft();
        }
    }

    void RunLeft() {
        Vector3 theScale = transform.localScale;
        theScale.x = -Mathf.Abs(theScale.x);
        transform.localScale = theScale;

        moveSpeed = -3;
    }

    void RunRight() {
        moveSpeed = 3;

        Vector3 theScale = transform.localScale;
        theScale.x = Mathf.Abs(theScale.x);
        transform.localScale = theScale;
    }

    void Flip() {
        // Multiply the hamsters's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

        moveSpeed *= -1;
    }
}
