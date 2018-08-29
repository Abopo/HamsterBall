using UnityEngine;
using System.Collections;

public class TeamSelectArrow : MonoBehaviour {
    public int side; // 0 - left side, 1 - right side
    public float moveSpeed;

    SpriteRenderer _spriteRenderer;

    float _moveTime = 0.5f;
    float _moveTimer;

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        // Do a little movement
        _moveTimer += Time.deltaTime;
        if(_moveTimer >= _moveTime) {
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
