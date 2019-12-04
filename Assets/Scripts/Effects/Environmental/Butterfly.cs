using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterfly : MonoBehaviour {

    public float moveSpeed;
    public float rotSpeed;
    public float maxDist; // Maximum distance from the start point the butterfly is allowed to go

    Vector3 _startPoint; // Where the butterfly starts
    float _curDist;

    int _curRotDir;
    float _changeRotTime = 0.2f;
    float _changeRotTimer = 0f;

    // Start is called before the first frame update
    void Start() {
        _startPoint = transform.position;

        // Randomly choose color
        Animator animator = GetComponentInChildren<Animator>();
        animator.SetInteger("Color", Random.Range(0, 4));
    }

    // Update is called once per frame
    void Update() {
        _curDist = Vector2.Distance(transform.position, _startPoint);

        // If we are at or past the maxDistance
        if (_curDist >= maxDist) {
            // Turn back towards startPoint
            transform.right = Vector3.RotateTowards(transform.right, _startPoint - transform.position, 0.07f, 0.0f);
            //transform.right = _startPoint - transform.position;
        // Otherwise
        } else {
            _changeRotTimer += Time.deltaTime;
            if(_changeRotTimer >= _changeRotTime) {
                // Randomly choose new rot dir
                _curRotDir = Random.Range(0, 2);
                _changeRotTimer = 0f;
            }

            if (_curRotDir == 0) {
                // Rotate right
                transform.Rotate(0f, 0f, rotSpeed * Time.deltaTime);
            } else {
                // Rotate left
                transform.Rotate(0f, 0f, -rotSpeed * Time.deltaTime);
            }
        }

        // Fly "forward"
        transform.Translate(moveSpeed * Time.deltaTime, 0f, 0f);

        // Make sure we stay facing camera properly
        transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawIcon(_startPoint, "!");

        Gizmos.DrawWireSphere(_startPoint, maxDist);
    }
}
