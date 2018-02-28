using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleButton : MonoBehaviour {

    public HAMSTER_TYPES type;
    public GameObject hamsterSpriteObj;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown() {
        // Create a bubble sprite obj
        GameObject bSprite = GameObject.Instantiate(hamsterSpriteObj, transform.position, Quaternion.identity);
        bSprite.GetComponent<BubbleSprite>().SetType(type);
        bSprite.GetComponent<BubbleSprite>().isHeld = true;
    }
}
