﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterSprite : MonoBehaviour {
    Rigidbody2D _rigidbody;
    SpriteRenderer _spriteRenderer;

    bool _specialMove;
    Vector3 _targetPoint;
    float moveSpeed = 10f;

    HamsterSpawner _hamSpawner;

    // Use this for initialization
    void Start () {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

    }
	
	// Update is called once per frame
	void Update () {
        if (_specialMove) {
            // Move towards the target point
            Vector2 dir = _targetPoint - transform.position;
            dir.Normalize();
            _rigidbody.velocity = new Vector2(moveSpeed * dir.x, moveSpeed * dir.y);

            // Rotate sprite
            _spriteRenderer.transform.Rotate(0f, 0f, 500f * Time.deltaTime);

            // If we get close enough to the target point
            if (Vector2.Distance(_targetPoint, transform.position) < 10f) {
                // Slow down based on distance
                moveSpeed = Vector2.Distance(_targetPoint, transform.position);
                if (moveSpeed < 5f) {
                    moveSpeed = 5f;
                }
            }
            if (Vector2.Distance(_targetPoint, transform.position) < 0.5f) {
                // Spawn a special hamster
                _hamSpawner.NextHamsterType = (int)HAMSTER_TYPES.SPECIAL;
                _hamSpawner.SpawnHamster();

                // Destroy self
                Destroy(transform.parent.gameObject);
            }
        }
    }

    public void Pop() {
        // Make sure sprite is solid
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 5f);

        _rigidbody.GetComponent<Animator>().SetInteger("State", 1);

        _rigidbody.isKinematic = false;
        _rigidbody.gravityScale = 2;

        float rX = Random.Range(1f, 2f);
        float rY = Random.Range(-0.5f, 2f);

        PopForce(rX, rY);
    }

    public void PlasmaPop() {
        // We need to swap the animator
        Bubble parentBubble = transform.parent.GetComponent<Bubble>();
        Animator animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Art/Animations/Hamsters/AnimationObjects/Plasma/Plasma_HamsterSprite");
        animator.SetInteger("Type", (int)parentBubble.type);

        // Make sure sprite is solid
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 5f);

        _rigidbody.isKinematic = false;
        _rigidbody.gravityScale = 1;

        float rX = Random.Range(1f, 2f);
        float rY = Random.Range(0f, 2f);

        PopForce(rX, rY);
    }

    void PopForce(float xForce, float yForce) {
        Bubble bubble = transform.parent.GetComponent<Bubble>();
        float averageX = 0f;
        foreach (Bubble b in bubble.matches) {
            if (b != null) {
                averageX += b.transform.position.x;
            }
        }
        averageX = averageX / bubble.matches.Count;

        float dir = 1f;
        if (transform.position.x < averageX) {
            dir = -1f;
            GetComponent<SpriteRenderer>().flipX = true;
        }

        _rigidbody.velocity = new Vector2(xForce * dir, 6f + yForce);
    }

    // when a special ball is popped
    public void SpecialPop() {
        // Make sure sprite is solid
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 5f);
        GetComponent<Animator>().SetInteger("State", 1);

        _rigidbody.isKinematic = false;

        // Find a hamster spawner on our team
        HamsterSpawner[] hSpawners = FindObjectsOfType<HamsterSpawner>();
        // Find spawner on the same team
        foreach (HamsterSpawner hS in hSpawners) {
            if (hS.team == transform.parent.GetComponent<Bubble>().team) {
                _targetPoint = hS.SpawnPosition;
                _hamSpawner = hS;
            }
        }

        _specialMove = true;
    }

    public void ResetSprite() {
        // Make sure sprite is solid
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.8823529f);

        // TODO: reset animator to deal with plasma pop?

        transform.localPosition = new Vector3(0f, 0f, -0.5f);
        transform.localScale = new Vector3(0.14647f, 0.14647f, 0.14647f);

        _rigidbody.GetComponent<Animator>().SetInteger("State", 0);

        _rigidbody.isKinematic = true;
        _rigidbody.gravityScale = 0;

        _rigidbody.velocity = Vector2.zero;

        _specialMove = false;
    }
}
