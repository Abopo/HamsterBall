using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireButton : MonoBehaviour {

    public bool pressed;

    FireSystem _fireSystem;

    private void Awake() {
        _fireSystem = transform.parent.GetComponent<FireSystem>();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // If we're hit by any attack object
        if (collision.gameObject.layer == 12) {
            // If we are not already pressed
            if (!pressed) {
                // Press the button
                Press();
            }
        }
    }

    public void Press() {
        pressed = true;

        // Turn on the FIRE
        _fireSystem.StartFire();
    }
}
