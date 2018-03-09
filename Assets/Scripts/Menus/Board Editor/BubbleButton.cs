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
    }

    private void OnMouseOver() {
        if(Input.GetMouseButtonDown(0)) {
            // Create a bubble sprite obj
            GameObject bSprite = GameObject.Instantiate(hamsterSpriteObj, transform.position, Quaternion.identity);
            bSprite.GetComponent<BubbleSprite>().SetType((int)type);
            //bSprite.GetComponent<BubbleSprite>().SetIsGravity(false);
            bSprite.GetComponent<BubbleSprite>().isHeld = true;
        } else if(Input.GetMouseButtonDown(1)) {
            // Create a gravity bubble sprite obj
            GameObject bSprite = GameObject.Instantiate(hamsterSpriteObj, transform.position, Quaternion.identity);
            bSprite.GetComponent<BubbleSprite>().SetType((int)type + 11);
            //bSprite.GetComponent<BubbleSprite>().SetIsGravity(true);
            bSprite.GetComponent<BubbleSprite>().isHeld = true;
        }
    }
}
