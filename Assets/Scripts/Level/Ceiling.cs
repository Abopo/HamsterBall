using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ceiling : MonoBehaviour {
    public int team;
    public BubbleManager bubbleManager;

	// Use this for initialization
	void Start () {
		if(team == 0) {
            bubbleManager = GameObject.FindGameObjectWithTag("BubbleManager1").GetComponent<BubbleManager>();
        } else {
            bubbleManager = GameObject.FindGameObjectWithTag("BubbleManager2").GetComponent<BubbleManager>();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
