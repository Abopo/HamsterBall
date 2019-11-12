using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoplight : MonoBehaviour {
    SpriteRenderer _spriteRenderer;
    BoxCollider2D _collider;

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
    }
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Go() {
        _spriteRenderer.color = Color.green;
        _collider.enabled = false;
    }

    public void Stop() {
        _spriteRenderer.color = Color.red;
        _collider.enabled = true;
    }
}
