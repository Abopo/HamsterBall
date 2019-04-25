using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script does anything to the player that is unique to the Rooster
public class Rooster : MonoBehaviour {

    PlayerController _playerController;
    BoxCollider2D _boxCollider;
    EntityPhysics _entityPhysics;

    Vector2 _baseOffset;
    Vector2 _baseSize;

    Vector2 _ballSize = new Vector2(2.16f, 1.6f);

	// Use this for initialization
	void Start () {
        _playerController = GetComponent<PlayerController>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _entityPhysics = GetComponent<EntityPhysics>();

        _baseOffset = _boxCollider.offset;
        _baseSize = _boxCollider.size;
	}
	
	// Update is called once per frame
	void Update () {
		if(_playerController.heldBall != null) {
            // Expand the player's collision to keep the held ball in bounds
            //_boxCollider.offset = _ballOffset;
            _boxCollider.size = _ballSize;
        } else {
            // Return the collision to normal size
            //_boxCollider.offset = _baseOffset;
            _boxCollider.size = _baseSize;
        }

        // Update the physic's collision data
        _entityPhysics.UpdateCollisionData();
    }
}
