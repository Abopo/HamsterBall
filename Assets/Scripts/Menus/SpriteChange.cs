using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteChange : MonoBehaviour {
    public bool on;

    public Sprite onSprite;
    public Sprite offSprite;

    SpriteRenderer _spriteRenderer;
    Image _image;

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _image = GetComponent<Image>();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void On() {
        on = true;

        if(_spriteRenderer != null) {
            _spriteRenderer.sprite = onSprite;
        }
        if(_image != null) {
            _image.sprite = onSprite;
        }
    }

    public void Off() {
        on = false;

        if (_spriteRenderer != null) {
            _spriteRenderer.sprite = offSprite;
        }
        if (_image != null) {
            _image.sprite = offSprite;
        }
    }
}
