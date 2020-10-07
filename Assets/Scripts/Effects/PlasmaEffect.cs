using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaEffect : MonoBehaviour {
    // 0 - top left, increases clockwise.
    public GameObject[] plasma = new GameObject[8];

    public bool active;

    Bubble _bubble;
    Animator[] _animators;

    private void Awake() {
        _bubble = transform.parent.GetComponent<Bubble>();
        _animators = GetComponentsInChildren<Animator>();

        Deactivate();
    }
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Activate(int type) {
        // Turn on all the plasma in open adj spots
        if (_bubble != null) {
            for (int i = 0; i < 6; ++i) {
                if (_bubble.adjBubbles[i] == null) {
                    plasma[i].SetActive(true);
                    plasma[i].GetComponent<SpriteRenderer>().enabled = true;
                } else {
                    plasma[i].SetActive(false);
                }
            }
        }
        // Set animator type
        foreach (Animator anim in _animators) {
            anim.SetInteger("Type", type);
        }

        active = true;
    }
    
    public void Deactivate() {
        // Turn on all the plasma
        foreach (GameObject pRen in plasma) {
            pRen.SetActive(false);
        }
    }

    public void ForceActivate(int type) {
        for (int i = 0; i < 6; ++i) {
            plasma[i].SetActive(true);
            plasma[i].GetComponent<SpriteRenderer>().enabled = true;
        }

        // Set animator type
        foreach (Animator anim in _animators) {
            anim.SetInteger("Type", type);
        }
    }
}
