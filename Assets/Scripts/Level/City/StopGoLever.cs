using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Basically just rotates the lever into place
public class StopGoLever : MonoBehaviour {

    public FMOD.Studio.EventInstance LeverMoveEvent;
    public float rotSpeed;

    public StopGoButton rightButton;

    bool _isLeft;
    public bool IsLeft {
        get { return _isLeft; }
    }

    bool _isRotating;
    Vector3 leftPos = new Vector3(0f, 0f, 0f);
    Vector3 rightPos = new Vector3(0f, 0f, 255f);

    StopGoButton _button;

    // Auto time is used in singleplayer, automatically activates lever
    float _autoTime = 5.0f;
    float _autoTimer = 0f;

    GameManager _gameManager;
    Animator _animator;

    private void Awake() {
        _gameManager = GameManager.instance;
        _animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start() {
        _isLeft = true;
    }

    // Update is called once per frame
    void Update() {
        if (_isRotating) {
            if (_isLeft) {
                // Rotate to the right
                if (Vector3.Distance(transform.eulerAngles, rightPos) > 0.01f) {
                    float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, 255f, rotSpeed * Time.deltaTime);
                    transform.eulerAngles = new Vector3(0f, 0f, angle);
                } else {
                    transform.eulerAngles = rightPos;
                    _isLeft = false;
                    EndRotation();
                }
            } else {
                // Rotate left
                if (Vector3.Distance(transform.eulerAngles, leftPos) > 0.01f) {
                    float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, 0f, rotSpeed * Time.deltaTime);
                    transform.eulerAngles = new Vector3(0f, 0f, angle);
                } else {
                    transform.eulerAngles = leftPos;
                    _isLeft = true;
                    EndRotation();
                }
            }
        } else {
            // If we're playing a singleplayer stage
            if (_gameManager.isSinglePlayer) {
                // Since single player is always on the left side, if the lever is on the right run the auto timer
                if (!_isLeft) {
                    _autoTimer += Time.deltaTime;
                    if (_autoTimer >= _autoTime) {
                        // Automatically change position via right button
                        rightButton.Press();
                        _autoTimer = 0f;
                    }
                }
            }
        }
    }

    public void ChangePosition(StopGoButton invoker) {
        _button = invoker;
        _isRotating = true;

        FMODUnity.RuntimeManager.PlayOneShot("event:/Stages/LeverStart1");
        LeverMoveEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Stages/LeverMove1");
        LeverMoveEvent.start();
    }

    void EndRotation() {
        _isRotating = false;
        _button.FinishPress();

        LeverMoveEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        LeverMoveEvent.release();
        FMODUnity.RuntimeManager.PlayOneShot("event:/Stages/LeverStop1");

        if (_gameManager.isSinglePlayer) {
            // Since single player is always on the left side, if the lever is on the right run the auto timer
            if (!_isLeft) {
                _animator.Play("LeverMeterBuild");
            } else {
                _animator.Play("LeverIdle");
            }
        }
    }
}