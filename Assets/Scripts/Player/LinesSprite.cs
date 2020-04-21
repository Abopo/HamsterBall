using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinesSprite : MonoBehaviour {
    public bool turnOff;

    SpriteRenderer _parentSprite;
    SpriteRenderer _mySprite;

    // Start is called before the first frame update
    void Start() {
        _parentSprite = transform.parent.GetComponent<SpriteRenderer>();
        _mySprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        if(_mySprite.sortingOrder - _parentSprite.sortingOrder != 1) {
            _mySprite.sortingOrder = _parentSprite.sortingOrder + 1;
        }

        // Make sure sprite stays off if we don't want it to appear
        if(turnOff) {
            if(_mySprite.enabled) {
                _mySprite.enabled = false;
            }
        }
    }
}
