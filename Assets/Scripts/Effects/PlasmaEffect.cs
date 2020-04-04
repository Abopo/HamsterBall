using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaEffect : MonoBehaviour {
    // 0 - top left, increases clockwise.
    public GameObject[] plasma = new GameObject[8];

    Bubble _bubble;

    private void Awake() {
        _bubble = transform.parent.GetComponent<Bubble>();
    }
    // Start is called before the first frame update
    void Start() {
        Deactivate();
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Activate() {
        // Turn on all the plasma
        foreach(GameObject pRen in plasma) {
            pRen.SetActive(true);
            pRen.GetComponent<SpriteRenderer>().enabled = true;
        }

        // Go through the bubbles adjbubbles, and turn off corresponding plasma

        // for top Left
        if(_bubble.adjBubbles[0] != null) {
            // turn off top left and top
            plasma[0].SetActive(false);
            plasma[1].SetActive(false);
        }
        // Top right
        if(_bubble.adjBubbles[1] != null) {
            // turn off top and top right
            plasma[1].SetActive(false);
            plasma[2].SetActive(false);
        }
        // For the left/right sides just turn off the sides
        if (_bubble.adjBubbles[2] != null) {
            plasma[3].SetActive(false);
        }
        if(_bubble.adjBubbles[5] != null) {
            plasma[7].SetActive(false);
        }
        // For bottom bubbles, just like tops
        if(_bubble.adjBubbles[3] != null) {
            plasma[4].SetActive(false);
            plasma[5].SetActive(false);
        }
        if(_bubble.adjBubbles[4] != null) {
            plasma[5].SetActive(false);
            plasma[6].SetActive(false);
        }
    }
    
    public void Deactivate() {
        // Turn on all the plasma
        foreach (GameObject pRen in plasma) {
            pRen.SetActive(false);
        }
    }
}
