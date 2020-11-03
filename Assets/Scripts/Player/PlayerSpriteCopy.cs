using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteCopy : MonoBehaviour {

    SpriteRenderer _parentSprite;
    SpriteRenderer _mySprite;

    private void Awake() {
        _parentSprite = transform.parent.GetComponent<SpriteRenderer>();
        _mySprite = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
    }

    private void LateUpdate() {
        _mySprite.sprite = _parentSprite.sprite;
    }
}
