using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RawImageScroll : MonoBehaviour {
    public float _scrollSpeedX;
    public float _scrollSpeedY;


    RawImage _background;

    float _offsetX = 0f;
    float _offsetY = 0f;

	// Use this for initialization
	void Start () {
        _background = GetComponent<RawImage>();
	}
	
	// Update is called once per frame
	void Update () {
        _offsetX += _scrollSpeedX * Time.deltaTime;
        _offsetY += _scrollSpeedY * Time.deltaTime;

        _background.uvRect = new Rect(_offsetX, _offsetY, _background.uvRect.width, _background.uvRect.height);
	}
}
