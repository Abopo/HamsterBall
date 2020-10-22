using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Chemical containers rock back and forth when the player runs by them
public class ChemicalContainer : MonoBehaviour {

    bool _wiggling;

    Animator _animator;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player") {
            // We only want to wiggle if the player is on our platform
            // So check that the player is above us and walking
            if(collision.transform.position.y > transform.position.y && collision.GetComponent<PlayerController>().CurState == PLAYER_STATE.WALK) {
                // Wiggle
                if (collision.transform.position.x < transform.position.x) {
                    Wiggle(true);
                } else {
                    Wiggle(false);
                }
            }
        }
    }

    void Wiggle(bool left) {
        if(_wiggling) {
            return;
        }

        _wiggling = true;

        if (left) {
            _animator.Play("ChemicalShake");
        } else {
            _animator.Play("ChemicalShakeRight");
        }
    }

    public void EndShake() {
        _wiggling = false;
    }
}
