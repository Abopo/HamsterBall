using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubwayTrain : MonoBehaviour {
    float _moveSpd = 50f;

    bool _isMoving = false;

    float _moveTime = 2f;
    float _startTime = 20f;
    float _moveTimer = 0f;

    ShakeableTransform mainCamera;

    private void Awake() {
        mainCamera = FindObjectOfType<Camera>().GetComponent<ShakeableTransform>();
    }
    // Start is called before the first frame update
    void Start() {
        transform.position = new Vector3(15f, transform.position.y, transform.position.z);

        _startTime = Random.Range(15, 45);
    }

    // Update is called once per frame
    void Update() {
        if(_isMoving) {
            transform.Translate(-_moveSpd * Time.deltaTime, 0f, 0f);

            _moveTimer += Time.deltaTime;
            if(_moveTimer >= _moveTime) {
                StopMoving();
            }
        } else {
            // Maybe move?
            _moveTimer += Time.deltaTime;
            if(_moveTimer >= _startTime - 2f) {
                FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.SubwayPass);
                FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.SubwayRumble);
                mainCamera.StartShake(3f, 6f, new Vector2(0.03f, 0.03f));
            }
            if(_moveTimer >= _startTime) {
                StartMoving();
            }
        }
    }

    void StartMoving() {
        _isMoving = true;
        _moveTimer = 0f;
        mainCamera.StartShake(_moveTime, 8f, new Vector2(0.08f, 0.08f));
    }

    void StopMoving() {
        _isMoving = false;
        _moveTimer = 0f;
        transform.position = new Vector3(15f, transform.position.y, transform.position.z);
    }
}
