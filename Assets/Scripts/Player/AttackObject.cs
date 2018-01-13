using UnityEngine;
using System.Collections;

public class AttackObject : MonoBehaviour {
    public float attackDistance;
    public Vector2 attackVelocity;
    public int team;

    Rigidbody2D _rigidbody2D;
    PlayerController _playerController;

    bool _isAttacking;
    float _distFromPlayer;

    public bool IsAttacking {
        get { return _isAttacking; }
    }

    void Awake() {
        if (_rigidbody2D == null) {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _playerController = transform.parent.GetComponent<PlayerController>();
        }
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	    if(_isAttacking) {
            _distFromPlayer = Mathf.Abs(transform.position.x - _playerController.transform.position.x);
            if(_distFromPlayer >= attackDistance) {
                // Finish attack
                StopAttack();
            }
        }   
	}

    public void Attack() {
        if(_rigidbody2D == null) {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _playerController = transform.parent.GetComponent<PlayerController>();
        }
        gameObject.SetActive(true);
        transform.position = new Vector2(_playerController.transform.position.x + (0.1f * _playerController.direction), _playerController.transform.position.y);
        _rigidbody2D.velocity = new Vector2(attackVelocity.x * _playerController.direction, attackVelocity.y);
        _isAttacking = true;
    }

    void StopAttack() {
        _rigidbody2D.velocity = Vector2.zero;
        gameObject.SetActive(false);
        _isAttacking = false;
    }
}
