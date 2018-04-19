﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterChangeArrow : MonoBehaviour {
    public int side; // 0 - top side, 1 - bottom side
    public float moveSpeed;

    SpriteRenderer _spriteRenderer;

    float _moveTime = 0.5f;
    float _moveTimer;

    // Use this for initialization
    void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        // Do a little movement
        _moveTimer += Time.deltaTime;
        if (_moveTimer >= _moveTime) {
            // switch movement direction
            moveSpeed = -moveSpeed;
            _moveTimer = 0f;
        }
        transform.Translate(moveSpeed * Time.deltaTime, 0f, 0f);
    }

    public void Activate() {
        _spriteRenderer.enabled = true;
    }

    public void Deactivate() {
        _spriteRenderer.enabled = false;
    }
}
