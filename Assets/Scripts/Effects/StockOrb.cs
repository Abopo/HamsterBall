﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockOrb : MonoBehaviour {
    public Transform targetTransform;
    public int team;

    float _delayTime = 0.5f;
    float _delayTimer = 0.0f;

    float moveSpeed = 20.0f;

    bool _destroy = false;

    Rigidbody2D _rigidbody;
    GameManager _gameManager;

    SpriteRenderer _spriteRenderer;
    TrailRenderer _trailRenderer;

    public bool HamsterTravelStart = false;

	// Use this for initialization
	void Start () {
        Initialize();
        //Launch();
		SoundManager.mainAudio.HamsterTravelEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.HamsterTravel);
		FMODUnity.RuntimeManager.AttachInstanceToGameObject(SoundManager.mainAudio.HamsterTravelEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
	}
	
    public void Initialize() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _gameManager = FindObjectOfType<GameManager>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _trailRenderer = GetComponent<TrailRenderer>();

		FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.HamsterOrbCreate);
    }

    public void Launch(Transform target, int type)
    {
        targetTransform = target;

        // Set colors based on type
        SetColors(type);

        float velX, velY;
        velX = Random.Range(-10, 10);
        velY = Random.Range(5, 10);
        _rigidbody.velocity = new Vector2(velX, velY);
    }

    void SetColors(int type) {
        Color startColor = new Color();
        Color endColor = new Color();

        switch (type) {
            case 0: // Green
                startColor = new Color(51f / 255f, 255f / 255f, 77f / 255f);
                endColor = new Color(184f / 255f, 255f / 255f, 193f / 255f);
                break;
            case 1: // Red
                startColor = new Color(255f / 255f, 26f / 255f, 26f / 255f);
                endColor = new Color(255f / 255f, 144f / 255f, 144f / 255f);
                break;
            case 2: // Yellow
                startColor = new Color(255f / 255f, 255f / 255f, 98f / 255f);
                endColor = new Color(255f / 255f, 255f / 255f, 218f / 255f);
                break;
            case 3: // Gray
                startColor = new Color(193f / 255f, 193f / 255f, 193f / 255f);
                endColor = new Color(240f / 255f, 240f / 255f, 240f / 255f);
                break;
            case 4: // Blue
                startColor = new Color(98f / 255f, 157f / 255f, 255f / 255f);
                endColor = new Color(174f / 255f, 201f / 255f, 255f / 255f);
                break;
            case 5: // Pink
                startColor = new Color(255f / 255f, 111f / 255f, 221f / 255f);
                endColor = new Color(255f / 255f, 174f / 255f, 249f / 255f);
                break;
            case 6: // Purple
                startColor = new Color(181f / 255f, 64f / 255f, 255f / 255f);
                endColor = new Color(204f / 255f, 174f / 255f, 255f / 255f);
                break;
        }

        _spriteRenderer.color = startColor;
        _trailRenderer.startColor = startColor;
        _trailRenderer.endColor = endColor;
    }

	// Update is called once per frame
	void Update () {
        // Don't update if the game is over
        if (_gameManager.gameIsOver) {
            return;
        }

        _delayTimer += Time.deltaTime;
		if(_delayTimer >= _delayTime) {
            // Head towards target
            Vector2 dir = targetTransform.position - transform.position;
            dir.Normalize();
            _rigidbody.velocity = new Vector2(moveSpeed * dir.x, moveSpeed * dir.y);

            if(HamsterTravelStart == false) {
            	SoundManager.mainAudio.HamsterTravelEvent.start();
            	HamsterTravelStart = true;
			}

            // Rotate sprite
            _spriteRenderer.transform.Rotate(0f, 0f, 500f * Time.deltaTime);
        }
	}

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Tally" && collision.transform == targetTransform && !_destroy) {
            HamsterMeter hMeter = collision.transform.parent.GetComponent<HamsterMeter>();
            if (hMeter.team != team) {
                // Add stock to the hamster meter
                hMeter.IncreaseStock(1);

                DestroyObject(this.gameObject);

                _destroy = true;
            }
        }
    }
}
