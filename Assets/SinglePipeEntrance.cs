using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePipeEntrance : MonoBehaviour {
    bool sendLeft;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // true is left, false is right
    public bool GetDirection() {
        bool sendDir;

        sendDir = sendLeft;
        sendLeft = !sendLeft;

        return sendDir;
    }
}
