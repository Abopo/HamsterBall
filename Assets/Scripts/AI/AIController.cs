using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

// This script controls the input of the AI. It takes info from the brain
// and it's various scan scripts. It then generates input to
// fulfill the curAction and sends them to it's PlayerController script.
public class AIController : MonoBehaviour {
    InputState _input;
    InputState _prevInput;

    PlayerController _playerController;
    EntityPhysics _entityPhysics;
    AIBrain _aiBrain;
    AIMapScan _mapScan;

    AIAction _curAction;
    Vector2 _toNodeWant; // Vector from player to bubbleWant.

    // These timers give the AI a little time to think before shifting
    float toShiftTime = 0.3f;
    float toShiftTimer = 0f;

    // This determines how long the AI will pause before doing an action. Will change based on the difficulty
    float _actionTime = 0f; 
    float _actionTimer = 0f;

    bool _isMovingUp = false;
    int _moveDir = 0; // 1 - right, -1 - left
    Transform _targetPlatform; // the end cap of the platform we are trying to utilize for chase movement

    float _aimTime = 5.0f;
    float _aimTimer = 0f;
    int dumbFrameCount = 0;
    bool _isAimed = false;

    private void Awake() {
        _input = new InputState();
        _prevInput = new InputState();

        _playerController = GetComponent<PlayerController>();
        _playerController.aiControlled = true;
        _entityPhysics = GetComponent<EntityPhysics>();

        _aiBrain = GetComponent<AIBrain>();
        _mapScan = GetComponent<AIMapScan>();

        SetActionTime();

        _playerController.significantEvent.AddListener(SetActionTime);
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        ResetInput();

        _curAction = _aiBrain.curAction;
        if (_curAction == null) {
            return;
        }

        _actionTimer += Time.deltaTime;

        ShiftIfNeeded();

        if (_playerController.CurState != PLAYER_STATE.THROW && _playerController.CurState != PLAYER_STATE.SHIFT) {
            if (_actionTimer > _actionTime) {
                HorizontalMovement();
                Jumping();
            }
        } else if(_playerController.CurState == PLAYER_STATE.THROW) {
            if (_curAction.nodeWant != null && _playerController.heldBall != null) {
                _toNodeWant = _curAction.nodeWant.transform.position - _playerController.heldBall.transform.position;
            }
            AimThrow();
        }

        UpdateInput();

        // Send input to the player controller.
        _playerController.TakeInput(_input);

        GetPreviousInput();
	}

    void ResetInput() {
        // TODO: are these jump lines supposed to be gone?
        _input.jump.isDown = false;
        _input.jump.isJustReleased = false;
        _input.jump.isJustPressed = false;
        _input.left.isDown = false;
        _input.left.isJustPressed = false;
        _input.left.isJustReleased = false;
        _input.right.isDown = false;
        _input.right.isJustPressed = false;
        _input.right.isJustReleased = false;
        _input.down.isDown = false;
        _input.down.isJustPressed = false;
        _input.down.isJustReleased = false;
        _input.swing.isJustPressed = false;
        _input.shift.isDown = false;
        _input.shift.isJustPressed = false;
        _input.shift.isJustReleased = false;
        _input.attack.isDown = false;
        _input.attack.isJustPressed = false;
        _input.attack.isJustReleased = false;

        if (_entityPhysics.IsTouchingFloor) {
            _input.jump.isDown = false;
            _prevInput.jump.isDown = false;
        }
    }

    void UpdateInput() {
        if(_prevInput.jump.isJustPressed) {
            _input.jump.isJustPressed = false;
        } else if (!_prevInput.jump.isDown && _input.jump.isDown) {
            _input.jump.isJustPressed = true;
        } 

        if (_prevInput.jump.isJustReleased) {
            _input.jump.isJustReleased = false;
        } else if (_prevInput.jump.isDown && !_input.jump.isDown) {
            _input.jump.isJustReleased = true;
        }
    }

    void GetPreviousInput() {
        _prevInput.jump.isDown = _input.jump.isDown;
        _prevInput.jump.isJustReleased = _input.jump.isJustReleased;
        _prevInput.jump.isJustPressed = _input.jump.isJustPressed;
        _prevInput.left.isDown = _input.left.isDown;
        _prevInput.left.isJustPressed = _input.left.isJustPressed;
        _prevInput.left.isJustReleased = _input.left.isJustReleased;
        _prevInput.right.isDown = _input.right.isDown;
        _prevInput.right.isJustPressed = _input.right.isJustPressed;
        _prevInput.right.isJustReleased = _input.right.isJustReleased;
        _prevInput.down.isDown = _input.down.isDown;
        _prevInput.down.isJustPressed = _input.down.isJustPressed;
        _prevInput.down.isJustReleased = _input.down.isJustReleased;
        _prevInput.swing.isJustPressed = _input.swing.isJustPressed;
        _prevInput.shift.isDown = _input.shift.isDown;
        _prevInput.shift.isJustPressed = _input.shift.isJustPressed;
        _prevInput.shift.isJustReleased = _input.shift.isJustReleased;
        _prevInput.attack.isDown = _input.attack.isDown;
        _prevInput.attack.isJustPressed = _input.attack.isJustPressed;
        _prevInput.attack.isJustReleased = _input.attack.isJustReleased;
    }

    void ShiftIfNeeded() {
        if (_playerController.CanShift && _actionTimer > _actionTime) {
            if (_curAction.hamsterWant != null && _curAction.hamsterWant.team != _playerController.team && !_playerController.shifted) {
                toShiftTimer += Time.deltaTime;
            } else if (_playerController.heldBall != null && _curAction.bubbleWant != null && // if holding bubble and want a bubble and
                ((!_playerController.shifted && _curAction.bubbleWant.team != _playerController.team) || // not yet shifted and bubble want is on opposing side OR
                (_playerController.shifted && _curAction.bubbleWant.team == _playerController.team))) { // have shifted and bubble want is on our side
                toShiftTimer += Time.deltaTime;
            } else {
                toShiftTimer = 0f;
            }

            // Only shift if we have not shifted yet
            if (toShiftTimer >= toShiftTime) {
                _input.shift.isJustPressed = true;
                toShiftTimer = 0f;
                SetActionTime();
            }
        } else {
            toShiftTimer = 0f;
        }
    }

    void HorizontalMovement() {
        // Once we have a hamster caught, move to under the bubbleWant.
        if (_playerController.heldBall != null) {
            // if we are shifted with a hamster
            //if (_playerController.shifted) {
                // run from opponent
                RunMovement();
            //} else {
                ThrowMovement();
            //}
        } else {
            // If we don't have a bubble yet, go after a hamster.
            if (_curAction.hamsterWant != null) {
                ChaseMovement(_curAction.hamsterWant.gameObject);
            } else if (_curAction.opponent != null) {
                ChaseMovement(_curAction.opponent.gameObject);
            } else if (_curAction.waterBubble != null) {
                ChaseMovement(_curAction.waterBubble.gameObject);
            }
        }
    }

    // Run away from opposing player
    void RunMovement() {

    }

    // Walk based on Wants (generally chasing a hamster).
    void ChaseMovement(GameObject chaseObj) {
        // If we don't want to go anywhere don't move.
        if (_curAction.vertWant == 0) {
            // If we are here, there's probably a hamster we want on this level

            _isMovingUp = false;
            _input.left.isDown = false;
            _input.right.isDown = false;

            // If we are close to the object
            if (Mathf.Abs(chaseObj.transform.position.x - transform.position.x) < 0.65f) { 
                // If we are chasing a hamster and don't already have one
                if (_curAction.hamsterWant != null && _playerController.heldBall == null && _playerController.CurState != PLAYER_STATE.CATCH) {
                    // The hamster(or opponent) is right here! Catch it!
                    _input.swing.isJustPressed = true;
                    // Make a new decision based on what hamster we caught
                    _aiBrain.MakeDecision();
                // If we are chasing an opponent
                } else if (_curAction.opponent != null) {
                    // If the opponent is above us
                    if(_curAction.opponent.transform.position.y > transform.position.y) {
                        // Jump to hit them
                        _input.jump.isJustPressed = true;
                    }
                    // Punch 'em!
                    _input.attack.isJustPressed = true;

                    // Make new decision
                    _aiBrain.MakeDecision();
                } else if(_curAction.waterBubble != null) {
                    // If the bubble is above us
                    if (_curAction.waterBubble.transform.position.y > transform.position.y) {
                        // Jump to hit it
                        _input.jump.isJustPressed = true;
                    }
                    // Punch 'em!
                    _input.attack.isJustPressed = true;

                    // Make new decision
                    _aiBrain.MakeDecision();
                }
            } else if (_curAction.horWant == -1) {
                _input.left.isDown = true;
                _input.right.isDown = false;
            } else if (_curAction.horWant == 1) {
                _input.left.isDown = false;
                _input.right.isDown = true;
            }
        // If we want to go down, find closest drop and move towards it.
        } else if (_curAction.vertWant == -1) {
            // Move down towards want
            MovingDown();

        // If we want to go up, find closest step and move towards it.
        } else if (_curAction.vertWant == 1) {
            // if we're not already moving
            if(!_isMovingUp) {
                // move towards the closest jump
                BeginMovingUp();

            // If we're already moving
            } else {
                ContinueMovingUp();
            }
        }
    }

    void MovingDown() {
        _isMovingUp = false;

        if (_mapScan.LeftDropDistance < _mapScan.RightDropDistance) {
            // If we are currently moving right, 
            if(_input.right.isDown) {
                // make sure we aren't right on top of the drop
                if(_mapScan.LeftDropDistance > 1f) {
                    _input.left.isDown = true;
                    _input.right.isDown = false;
                }
            // Otherwise just head left
            } else {
                _input.left.isDown = true;
                _input.right.isDown = false;
            }
        } else {
            // If we are currently moving Left, 
            if (_input.left.isDown) {
                // make sure we aren't right on top of the drop
                if (_mapScan.RightDropDistance > 1f) {
                    _input.left.isDown = false;
                    _input.right.isDown = true;
                }
            // Otherwise just head right
            } else {
                _input.left.isDown = false;
                _input.right.isDown = true;
            }
        }

        // If we're are standing on a passthrough platform, just press down to fall through
        if (_mapScan.IsOnPassthrough) {
            _input.down.isJustPressed = true;
        }
    }

    void BeginMovingUp() {
        if(_mapScan.ClosestJump == null) {
            return;
        }

        // If it's to our left
        if (_mapScan.ClosestJump.transform.position.x < transform.position.x) {
            _input.left.isDown = true;
            _input.right.isDown = false;
            _moveDir = -1;
        // If it's to our right
        } else {
            _input.left.isDown = false;
            _input.right.isDown = true;
            _moveDir = 1;
        }

        _isMovingUp = true;
    }

    void ContinueMovingUp() {
        // If we aren't jumping
        if (_playerController.CurState != PLAYER_STATE.JUMP) {
            // Update where the closest jump is
            _targetPlatform = _mapScan.ClosestJump;
        }

        if (_targetPlatform != null) {
            // If the target is above us
            if (_targetPlatform.transform.position.y > transform.position.y) {
                // We should try to move toward it

                // Only change direction if we are not under a ceiling or jumping (or if we are touching a wall)
                // If the target is to our left
                if (_moveDir == 1 && _targetPlatform.transform.position.x < transform.position.x - 1f /*&&
                    (!_mapScan.IsUnderCeiling || _playerController.CurState == PLAYER_STATE.JUMP)*/) {
                    // Change direction to the left
                    _input.left.isDown = true;
                    _input.right.isDown = false;
                    _moveDir = -1;

                // If the target is to our right
                } else if (_moveDir == -1 && _targetPlatform.transform.position.x > transform.position.x + 1f /*&&
                           (!_mapScan.IsUnderCeiling || _playerController.CurState == PLAYER_STATE.JUMP)*/) {
                    // Change direction to the right
                    _input.left.isDown = false;
                    _input.right.isDown = true;
                    _moveDir = 1;

                } else {
                    // Keep moving the same direction
                    if (_moveDir == -1) {
                        _input.left.isDown = true;
                        _input.right.isDown = false;
                    } else if (_moveDir == 1) {
                        _input.left.isDown = false;
                        _input.right.isDown = true;
                    }
                }
            // If the target is below us
            } else if(_targetPlatform.GetComponent<PlatformEndCap>() != null) {
                // We should try to move to where the platform is
                if (_targetPlatform.GetComponent<PlatformEndCap>().platformSide == SIDE.LEFT) {
                    _input.left.isDown = true;
                    _input.right.isDown = false;
                    _moveDir = -1;
                } else {
                    _input.left.isDown = false;
                    _input.right.isDown = true;
                    _moveDir = 1;
                }
            } else {
                // Just move towards the platform? (as it's probably a fallthrough)

            }
        } else {
            Debug.Log("Missing Endcap!");
        }
    }

    void Jumping() {
        // If we want to move horizontally but there is a step in the way.
        if(_curAction.horWant == 1 && _mapScan.RightStepDistance > 0 && _mapScan.RightStepDistance < 0.5f) {
            if(_playerController.CurState != PLAYER_STATE.JUMP) {
                _input.jump.isDown = true;
            }
        } else if(_curAction.horWant == -1 && _mapScan.LeftStepDistance > 0 && _mapScan.LeftStepDistance < 0.5f) {
            if (_playerController.CurState != PLAYER_STATE.JUMP) {
                _input.jump.isDown = true;
            }
        }

        // If we are already jumping, go for the highest jump!
        // TODO: Don't do this for small steps
        if(_playerController.CurState == PLAYER_STATE.JUMP) {
            _input.jump.isDown = true;
        }
        // If you don't want to go up, don't jump
        if (_curAction.vertWant != 1) {
            return;
        }        
        if (_mapScan.LeftJumpDistance < 1.75f && _input.left.isDown && !_mapScan.IsUnderCeiling) {
            _input.jump.isDown = true;
        } else if(_mapScan.RightJumpDistance < 1.75f && _input.right.isDown && !_mapScan.IsUnderCeiling) {
            _input.jump.isDown = true;
        }
    }

    // Move to a good position to throw a bubble.
    void ThrowMovement() {
        if (_curAction.requiresShift && _playerController.heldBall.type == HAMSTER_TYPES.RAINBOW) {
            // don't throw a rainbow to the opponent
            _aiBrain.ChooseNewAction();
            return;
        }

        // We are in position to throw.
        if (_curAction.horWant == 0) {
            _input.left.isDown = false;
            _input.right.isDown = false;
            if (_actionTimer > _actionTime) {
                AimThrow();
            }
        } else if (_curAction.horWant == -1) {
            _input.left.isDown = true;
            _input.right.isDown = false;
        } else if (_curAction.horWant == 1) {
            _input.left.isDown = false;
            _input.right.isDown = true;
        }

        // Jump up if passing by a step since being higher is generally better
        if (_mapScan.LeftJumpDistance < 1.5f && _input.left.isDown && !_mapScan.IsUnderCeiling) {
            _input.jump.isDown = true;
        } else if (_mapScan.RightJumpDistance < 1.5f && _input.right.isDown && !_mapScan.IsUnderCeiling) {
            _input.jump.isDown = true;
        } else {
            _input.jump.isDown = false;
        }
    }

    void AimThrow() {
        // First put the player into throw state if not already there.
        if(_playerController.CurState != PLAYER_STATE.THROW) {
            _input.swing.isJustPressed = true;
            return;
        }

        // If we're not yet aiming at nodeWant
        Vector2 aimDirection = ((ThrowState)_playerController.currentState).AimDirection;
        float dot = Vector2.Dot(aimDirection.normalized, _toNodeWant.normalized);
        if (dot < 0.9999f && dumbFrameCount < 30 && !_isAimed) {
            // Rotate towards the node

            // If we've been aiming for too long, just go ahead and throw
            _aimTimer += Time.deltaTime;
            if(_aimTimer >= _aimTime) {
                _aimTimer = 0f;
                dumbFrameCount = 0;
                _input.swing.isJustPressed = true;
                _isAimed = false;
            }

            // If bubble is on the right
            if (AngleDir(_toNodeWant.normalized, aimDirection.normalized) < 0) {
                _input.right.isDown = true;
                // If bubble is on the left
            } else if (AngleDir(_toNodeWant.normalized, aimDirection.normalized) > 0) {
                _input.left.isDown = true;
            }

            // If we are aiming at the node
        } else if(_actionTimer > _actionTime || _isAimed) {
            // Throw
            dumbFrameCount++; // Waits for 20 frames to make sure aim is on point
            if (dumbFrameCount > 20) {
                _isAimed = true;
                OffsetAim();

                if (dumbFrameCount > 30) {
                    dumbFrameCount = 0;
                    _aimTimer = 0f;

                    _input.swing.isJustPressed = true;
                    _isAimed = false;
                }
            }
        }
    }

    void OffsetAim() {
        // Randomly offset aim depending on how stupid the AI is
        int chanceToOffset = _aiBrain.Difficulty * 10;
        if (Random.Range(0, 80) > chanceToOffset) {
            if (Random.Range(0, 2) == 1) {
                _input.right.isDown = true;
            } else {
                _input.left.isDown = true;
            }
        }
    }

    // This is used in an attempt to improve corner jumping around platforms
    // By waiting a small amount before turning around, it makes sure the the AI has walked far enough to make the jump back without hitting the ceiling
    IEnumerator ChangeDirection(bool left) {
        yield return new WaitForSeconds(0.05f);

        if (left) {
            _moveDir = -1;
        } else {
            _moveDir = 1;
        }
    }

    // This returns a negative number if B is left of A, positive if right of A, or 0 if they are perfectly aligned.
    float AngleDir(Vector2 A, Vector2 B) {
        return -A.x * B.y + A.y * B.x;
    }

    void SetActionTime() {
        _actionTime = (1f - 0.1f * (_aiBrain.Difficulty)) + Random.Range(-0.5f, 0.5f);
        if(_playerController.shifted) {
            _actionTime = 0;
        }
        _actionTimer = 0.0f;
    }
}
