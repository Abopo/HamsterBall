using System.Collections;
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

	// Use this for initialization
	void Start () {
        Initialize();
        //Launch();
	}
	
    public void Initialize() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _gameManager = FindObjectOfType<GameManager>();
        Debug.Log("Hamsters Moving Over");
    }

    public void Launch(Transform target)
    {
        targetTransform = target;
        float velX, velY;
        velX = Random.Range(-10, 10);
        velY = Random.Range(5, 10);
        _rigidbody.velocity = new Vector2(velX, velY);
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
