﻿using UnityEngine;
using System.Collections;

// This class scans the area around the AI to determine movement options.
// It does this by casting rays in various directions, to figure out what the
// nearby level geometry is.
public class AIMapScan : MonoBehaviour {
    public LayerMask collisionMask;

    PlayerController _playerController;

    float _leftWallDistance; // Distance to the closest wall to the left of the character.
    float _rightWallDistance; // Distance to the closest wall to the right of the character.
    float _leftStepDistance;
    float _rightStepDistance;
    float _leftDropDistance; // Distance to the closest dropoff to the left of the character.
    float _rightDropDistance; // Distance to the closest dropoff to the right of the character.
    float _leftJumpDistance; // Distance to the closest step up to the left of the character.
    float _rightJumpDistance; // Distance to the closest step up to the right of the character.

    bool _isUnderCeiling; // If there is a ceiling directly above the character.
    bool _isOnPassthrough; // If AI is standing on a passthrough platform

    Ray2D _leftWallCheckRay; // Ray for checking distances
    RaycastHit2D _leftWallCheckHit; // Result of ray checks
    Ray2D _rightWallCheckRay; // Ray for checking distances
    RaycastHit2D _rightWallCheckHit; // Result of ray checks
    Ray2D _stepCheckRay;
    RaycastHit2D _stepCheckHit;
    Ray2D _dropCheckRay; // Ray for checking distances
    RaycastHit2D _dropCheckHit; // Result of ray checks
    Ray2D _jumpCheckRay; // Ray for checking distances
    RaycastHit2D _jumpCheckHit; // Result of ray checks
    RaycastHit2D _jumpCheckHit2; // Result of ray checks

    Transform _closestJump;
    Transform _closestDrop;

    Vector2 _pos; // Transform position
    //float _scaledRadius;
    float _scaledRadiusX;
    float _scaledRadiusY;


    // Stage specific
    LayerMask _lightningMask;
    Ray2D _lightningRay;
    RaycastHit2D _lightningHit;
    bool _lightningOnLeft;
    float _lightningDistLeft;
    bool _lightningOnRight;
    float _lightningDistRight;

    public float LeftWallDistance {
        get { return _leftWallDistance; }
    }
    public float RightWallDistance {
        get { return _rightWallDistance; }
    }
    public float LeftDropDistance {
        get { return _leftDropDistance; }
    }
    public float RightDropDistance {
        get { return _rightDropDistance; }
    }
    public float LeftJumpDistance {
        get { return _leftJumpDistance; }
    }
    public float RightJumpDistance {
        get { return _rightJumpDistance; }
    }
    public bool IsUnderCeiling {
        get { return _isUnderCeiling; }
    }
    public bool IsOnPassthrough {
        get { return _isOnPassthrough; }
    }
    public float LeftStepDistance {
        get { return _leftStepDistance; }
    }
    public float RightStepDistance {
        get { return _rightStepDistance; }        
    }

    public Transform ClosestJump {
        get { return _closestJump; }
    }
    public Transform ClosestDrop {
        get { return _closestDrop; }
    }

    public bool LightningOnLeft { get => _lightningOnLeft; }
    public bool LightningOnRight { get => _lightningOnRight; }
    public float LightningDistLeft { get => _lightningDistLeft; }
    public float LightningDistRight { get => _lightningDistRight; }

    // Use this for initialization
    void Start() {
        _playerController = GetComponent<PlayerController>();
        //_scaledRadius = Mathf.Abs(GetComponent<CircleCollider2D>().radius * transform.localScale.x);
        BoxCollider2D _collider = GetComponent<BoxCollider2D>();
        _scaledRadiusX = Mathf.Abs(_collider.size.x / 2 * transform.localScale.x);
        _scaledRadiusY = Mathf.Abs(_collider.size.y / 2 * transform.localScale.y);

        _lightningMask = _lightningMask | (1 << 25);
    }

    // Update is called once per frame
    void Update() {
        CheckDistances();
    }

    void CheckDistances() {
        _pos = transform.position;
        // Wall distances
        CheckWallDistances();
        if (_playerController.CurState != PLAYER_STATE.JUMP/* && _playerController.CurState != PLAYER_STATE.FALL*/) {
            CheckJumpDistancesNew();
        }
        CheckDropDistances();

        // Stage Specific
        CheckForLightning();
    }

    void CheckWallDistances() {
        float y = _pos.y + GetComponent<Collider2D>().offset.y + _scaledRadiusY / 1.5f;
        // Check Right
        _rightWallCheckRay = new Ray2D(new Vector2(_pos.x, y), Vector2.right);
        _rightWallCheckHit = Physics2D.Raycast(_rightWallCheckRay.origin, _rightWallCheckRay.direction, 1000f, collisionMask);
        Debug.DrawRay(_rightWallCheckRay.origin, _rightWallCheckRay.direction* _rightWallCheckHit.distance);
        if (_rightWallCheckHit) {
            _rightWallDistance = _rightWallCheckHit.distance;
        }
        // Also check for steps on the right
        _stepCheckRay = new Ray2D(new Vector2(_pos.x, _pos.y), Vector2.right);
        _stepCheckHit = Physics2D.Raycast(_stepCheckRay.origin, _stepCheckRay.direction, 5f, collisionMask);
        Debug.DrawRay(_stepCheckRay.origin, _stepCheckRay.direction * _stepCheckHit.distance);
        if (_stepCheckHit && _stepCheckHit.distance < _rightWallDistance) {
           _rightStepDistance  = _stepCheckHit.distance;
        } else {
            _rightStepDistance = -1;
        }

        // Check Left
        _leftWallCheckRay = new Ray2D(new Vector2(_pos.x, y), Vector2.left);
        _leftWallCheckHit = Physics2D.Raycast(_leftWallCheckRay.origin, _leftWallCheckRay.direction, 1000f, collisionMask);
        Debug.DrawRay(_leftWallCheckRay.origin, _leftWallCheckRay.direction* _leftWallCheckHit.distance);
        if (_leftWallCheckHit) {
            _leftWallDistance = _leftWallCheckHit.distance;
        }
        // Also check for steps on the left
        _stepCheckRay = new Ray2D(new Vector2(_pos.x, _pos.y), Vector2.left);
        _stepCheckHit = Physics2D.Raycast(_stepCheckRay.origin, _stepCheckRay.direction, 5f, collisionMask);
        Debug.DrawRay(_stepCheckRay.origin, _stepCheckRay.direction * _stepCheckHit.distance);
        if (_stepCheckHit && _stepCheckHit.distance < _leftWallDistance) {
            _leftStepDistance = _stepCheckHit.distance;
        } else {
            _leftStepDistance = -1;
        }
    }

    void CheckJumpDistancesNew() {
        float rayOffsetX = 0;
        _rightJumpDistance = 100f;
        _leftJumpDistance = 100f;

        // Check if there's a ceiling above
        rayOffsetX = 0.05f;
        _jumpCheckRay = new Ray2D(new Vector2(_pos.x + rayOffsetX, _pos.y), Vector2.up);
        _jumpCheckHit = Physics2D.Raycast(_jumpCheckRay.origin, _jumpCheckRay.direction, 1f, collisionMask);
        rayOffsetX = -0.1f;
        _jumpCheckRay = new Ray2D(new Vector2(_pos.x + rayOffsetX, _pos.y), Vector2.up);
        _jumpCheckHit2 = Physics2D.Raycast(_jumpCheckRay.origin, _jumpCheckRay.direction, 1f, collisionMask);
        // If there is a ceiling above
        if (_jumpCheckHit && _jumpCheckHit.transform.gameObject.layer != LayerMask.NameToLayer("Passthrough") /*||
            _jumpCheckHit2 && _jumpCheckHit2.transform.gameObject.layer != LayerMask.NameToLayer("Passthrough")*/) {
            _isUnderCeiling = true;
        } else {
            _isUnderCeiling = false;
        }

        // Check Right
        rayOffsetX = 0.01f;
        while (rayOffsetX < _rightWallCheckHit.distance) {
            _jumpCheckRay = new Ray2D(new Vector2(_pos.x + rayOffsetX, _pos.y), Vector2.up);
            _jumpCheckHit = Physics2D.Raycast(_jumpCheckRay.origin, _jumpCheckRay.direction, 1.5f, collisionMask);

            // If the platform above us is a passthrough platform
            if (_jumpCheckHit && _jumpCheckHit.transform.gameObject.layer == LayerMask.NameToLayer("Passthrough")) {
                _rightJumpDistance = Mathf.Max(rayOffsetX, 0.5f);
                _closestJump = _jumpCheckHit.collider.transform;
                Debug.DrawRay(_jumpCheckRay.origin, _jumpCheckRay.direction * _jumpCheckHit.distance, Color.green);
                break;
            } else if (_jumpCheckHit && _jumpCheckHit.collider.tag == "Platform End Cap") {
                _rightJumpDistance = rayOffsetX;
                _closestJump = _jumpCheckHit.collider.transform;
                Debug.DrawRay(_jumpCheckRay.origin, _jumpCheckRay.direction * _jumpCheckHit.distance, Color.green);
                break;
            }

            rayOffsetX += 0.1f;
        }

        // Check Left
        rayOffsetX = 0.01f;
        while (rayOffsetX < _leftWallCheckHit.distance) {
            _jumpCheckRay = new Ray2D(new Vector2(_pos.x - rayOffsetX, _pos.y), Vector2.up);
            _jumpCheckHit = Physics2D.Raycast(_jumpCheckRay.origin, _jumpCheckRay.direction, 1.5f, collisionMask);

            if (_jumpCheckHit && _jumpCheckHit.transform.gameObject.layer == LayerMask.NameToLayer("Passthrough")) {
                _leftJumpDistance = Mathf.Max(rayOffsetX, 0.5f);
                if (_leftJumpDistance < _rightJumpDistance) {
                    _closestJump = _jumpCheckHit.collider.transform;
                }
                Debug.DrawRay(_jumpCheckRay.origin, _jumpCheckRay.direction * _jumpCheckHit.distance, Color.green);
                break;
            } else if (_jumpCheckHit && _jumpCheckHit.collider.tag == "Platform End Cap") {
                _leftJumpDistance = rayOffsetX;
                if(_leftJumpDistance < _rightJumpDistance) {
                    _closestJump = _jumpCheckHit.collider.transform;
                }
                Debug.DrawRay(_jumpCheckRay.origin, _jumpCheckRay.direction * _jumpCheckHit.distance, Color.green);
                break;
            }

            rayOffsetX += 0.1f;
        }
    }

    void CheckDropDistances() {
        // Check Right
        float rayOffsetX = 0;
        _rightDropDistance = 10f;
        _leftDropDistance = 10f;

        // Check at position first to determine what's below.
        _dropCheckRay = new Ray2D(new Vector2(_pos.x + rayOffsetX, _pos.y), Vector2.down);
        _dropCheckHit = Physics2D.Raycast(_dropCheckRay.origin, _dropCheckRay.direction, 1.5f, collisionMask);
        if (_dropCheckHit) {
            // If we are standing on a passthrough platform
            if(_dropCheckHit.transform.gameObject.layer == LayerMask.NameToLayer("Passthrough")) {
                _isOnPassthrough = true;
            } else {
                _isOnPassthrough = false;
            }
        }

        // Check right
        rayOffsetX = 0.1f;
        while (rayOffsetX < _rightWallCheckHit.distance) {
            _dropCheckRay = new Ray2D(new Vector2(_pos.x + rayOffsetX, _pos.y), Vector2.down);
            _dropCheckHit = Physics2D.Raycast(_dropCheckRay.origin, _dropCheckRay.direction, 1.5f, collisionMask);

            // If there is a passthrough platform nearby
            if (_dropCheckHit && _dropCheckHit.transform.gameObject.layer == LayerMask.NameToLayer("Passthrough")) {
                //_rightDropDistance = rayOffsetX;
                // Set the distance to the center of the passthrough platform instead of the edge
                _rightDropDistance = Mathf.Abs(transform.position.x - _dropCheckHit.transform.position.x);
                Debug.DrawRay(new Vector2(transform.position.x + _rightDropDistance, _dropCheckRay.origin.y), _dropCheckRay.direction * _dropCheckHit.distance, Color.blue);
                break;
            } else if (_dropCheckHit && _dropCheckHit.collider.tag == "Platform End Cap") {
                _rightDropDistance = rayOffsetX;
                Debug.DrawRay(_dropCheckRay.origin, _dropCheckRay.direction, Color.blue);
                break;
            }

            rayOffsetX += 0.1f;
        }

        // Check Left
        rayOffsetX = 0.1f;
        while (rayOffsetX < _leftWallCheckHit.distance) {
            _dropCheckRay = new Ray2D(new Vector2(_pos.x - rayOffsetX, _pos.y), Vector2.down);
            _dropCheckHit = Physics2D.Raycast(_dropCheckRay.origin, _dropCheckRay.direction, 1.5f, collisionMask);
            if (_dropCheckHit && _dropCheckHit.transform.gameObject.layer == LayerMask.NameToLayer("Passthrough")) {
                //_leftDropDistance = rayOffsetX;
                // Set the distance to the center of the passthrough platform instead of the edge
                _leftDropDistance = Mathf.Abs(transform.position.x - _dropCheckHit.transform.position.x);
                Debug.DrawRay(new Vector2(transform.position.x - _leftDropDistance, _dropCheckRay.origin.y), _dropCheckRay.direction * _dropCheckHit.distance, Color.blue);
                break;
            } else if (_dropCheckHit && _dropCheckHit.collider.tag == "Platform End Cap") {
                _leftDropDistance = rayOffsetX;
                Debug.DrawRay(_dropCheckRay.origin, _dropCheckRay.direction, Color.blue);
                break;
            }

            rayOffsetX += 0.1f;
        }

    }

    void CheckForLightning() {
        _lightningRay = new Ray2D(_pos, Vector2.right);
        _lightningHit = Physics2D.Raycast(_lightningRay.origin, _lightningRay.direction, 5f, _lightningMask);
        if(_lightningHit) {
            // Lightning to our right
            _lightningOnRight = true;
            _lightningDistRight = Mathf.Abs(transform.position.x - _lightningHit.transform.position.x);
        } else {
            _lightningOnRight = false;
            _lightningDistRight = 100;
        }

        _lightningRay = new Ray2D(_pos, -Vector2.right);
        _lightningHit = Physics2D.Raycast(_lightningRay.origin, _lightningRay.direction, 5f, _lightningMask);
        if (_lightningHit) {
            // Lightning to our left
            _lightningOnLeft = true;
            _lightningDistLeft = Mathf.Abs(transform.position.x - _lightningHit.transform.position.x);
        } else {
            _lightningOnLeft = false;
            _lightningDistRight = 100;
        }
    }
}
