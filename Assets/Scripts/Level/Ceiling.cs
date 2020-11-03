using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ceiling : MonoBehaviour {
    public int team;
    public BubbleManager bubbleManager;
    public bool bigCeiling;

    // Use this for initialization
    void Start () {
        GameObject tempBubbleObj = null;
		if(team == 0) {
            tempBubbleObj = GameObject.FindGameObjectWithTag("BubbleManager1");
        } else if(team == 1) {
            tempBubbleObj = GameObject.FindGameObjectWithTag("BubbleManager2");
        }

        if(tempBubbleObj != null) {
            bubbleManager = tempBubbleObj.GetComponent<BubbleManager>();
            bubbleManager.ceiling = transform;
        }

        if(bigCeiling && !GameManager.instance.isSinglePlayer) {
            gameObject.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
