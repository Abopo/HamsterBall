using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaEffect : MonoBehaviour {
    // 0 - top left, increases clockwise.
    public GameObject[] plasma = new GameObject[8];

    public bool active;

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

    // This is initially called by the plasma bubbles itself to Activate the effect 
    // on all it's supported bubbles recursively
    public void PlasmaEffectStart() {
        // Activate ourself
        Activate();

        // Activate all adjacent bubbles
        foreach(Bubble bub in _bubble.adjBubbles) {
            if(bub != null) {
                if (!bub.plasmaEffect.active) {
                    bub.plasmaEffect.PlasmaEffectStart();
                } else {
                    // Update in case adjBubbles has changed
                    Activate();
                }
            }
        }
    }
    
    public void Activate() {
        // Turn on all the plasma in open adj spots
        for(int i = 0; i < 6; ++i) {
            if(_bubble.adjBubbles[i] == null) {
                plasma[i].SetActive(true);
                plasma[i].GetComponent<SpriteRenderer>().enabled = true;
            } else {
                plasma[i].SetActive(false);
            }
        }

        active = true;
    }
    
    public void Deactivate() {
        // Turn on all the plasma
        foreach (GameObject pRen in plasma) {
            pRen.SetActive(false);
        }
    }
}
