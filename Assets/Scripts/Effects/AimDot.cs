using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimDot : MonoBehaviour {
    float moveSpeed = 10f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(moveSpeed * Time.deltaTime, 0f, 0f, Space.Self);	
	}

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Wall") {
            // Turn around
            float rotOffset = 90 - transform.rotation.eulerAngles.z;
            transform.Rotate(0f, 0f, rotOffset * 2, Space.World);

            /*
            if (collider.transform.position.x < transform.position.x) {
                transform.Rotate(0f, 0f, rotOffset * 2, Space.World);
            } else {
                transform.Rotate(0f, 0f, rotOffset * 2, Space.World);
                transform.Rotate(0f, 0f, 90f * Mathf.Sign(transform.lossyScale.x));
            }
            */
        } else if(collider.tag == "Bubble" ||
                  collider.tag == "Ceiling") {
            // Destory self
            DestroyObject(this.gameObject);
        }
    }
}
