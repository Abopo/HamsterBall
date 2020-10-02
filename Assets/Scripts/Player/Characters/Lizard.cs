using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script does anything to the player that is unique to the Lizard
public class Lizard : MonoBehaviour {

    PlayerController _playerController;
    BoxCollider2D _boxCollider;

    EntityPhysics _physics;
    float _baseSkinX;

    Vector2 _baseSize;
    Vector2 _ballSize = new Vector2(2.16f, 1.6f);

    // Use this for initialization
    void Start() {
        _playerController = GetComponent<PlayerController>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _physics = GetComponent<EntityPhysics>();

        _baseSkinX = _physics.SkinX;

        _baseSize = _boxCollider.size;
    }

    // Update is called once per frame
    void Update() {
        if (_playerController.heldBall != null) {
            // Expand the player's collision to keep the held ball in bounds
            //_boxCollider.offset = _ballOffset;
            //_boxCollider.size = _ballSize;

            // Instead of expanding the box collider, we should just adjust the skin for collisions
            _physics.SkinX = 0.4f;
        } else {
            // Return the collision to normal size
            //_boxCollider.offset = _baseOffset;
            //_boxCollider.size = _baseSize;

            // Return collision skin to default
            _physics.SkinX = _baseSkinX;
        }
    }
}
