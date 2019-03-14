using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceButton : MonoBehaviour {
    public GameObject iceSpriteObj;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            // Create an ice sprite obj
            Instantiate(iceSpriteObj, transform.position, Quaternion.identity);
        }
    }
}
