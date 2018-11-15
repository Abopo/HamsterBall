using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimDot : MonoBehaviour {
    float moveSpeed = 10f;

    private float _rayPos;
    public float RayPos {
        get { return _rayPos; }
    }

    private bool _reflected;
    public bool Reflected {
        get { return _reflected; }
    }

    AimingLine _aimingLine;

    // Use this for initialization
    void Start () {
        _rayPos = 0f;
        _aimingLine = GetComponentInParent<AimingLine>();
	}
	
	// Update is called once per frame
	void Update () {
        //transform.Translate(moveSpeed * Time.deltaTime, 0f, 0f, Space.Self);

        _rayPos += moveSpeed * Time.deltaTime;
	}

    public void Reflect() {
        // Turn around
        float rotOffset = 90 - transform.rotation.eulerAngles.z;
        transform.Rotate(0f, 0f, rotOffset * 2, Space.World);

        _reflected = true;
        _rayPos = 0;
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if(collider.tag == "Bubble" || collider.tag == "Ceiling") {
            // Remove self from aiming line
            _aimingLine.RemoveAimDot(this);

            // Destory self
            DestroyObject(this.gameObject);
        }
    }
}
