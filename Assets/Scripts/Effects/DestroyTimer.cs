using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple script to put on an object so that it destroys itself after a given period of time.
public class DestroyTimer : MonoBehaviour {

    public float effectTime = 1.0f;
    float _effectTimer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        _effectTimer += Time.deltaTime;
        if(_effectTimer > effectTime) {
            Destroy(this.gameObject);
        }
	}
}
