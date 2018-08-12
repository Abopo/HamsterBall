using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RawImageScroll : MonoBehaviour {

    RawImage _background;

    float _scrollSpeed = -0.15f;

    float _offsetX = 0f;
    float _offsetY = 0f;

	// Use this for initialization
	void Start () {
        _background = GetComponent<RawImage>();
	}
	
	// Update is called once per frame
	void Update () {
        _offsetX += _scrollSpeed * Time.deltaTime;
        _offsetY += _scrollSpeed * Time.deltaTime;

        _background.uvRect = new Rect(_offsetX, _offsetY, _background.uvRect.width, _background.uvRect.height);
	}
}
