using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScroll : MonoBehaviour {

    Material _material;

    float _currentScroll = 0;
    public float scrollSpeed;

    // Start is called before the first frame update
    void Start() {
        _material = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update() {
        _currentScroll += scrollSpeed * Time.deltaTime;
        _material.mainTextureOffset = new Vector2(_currentScroll, 0);
    }
}
