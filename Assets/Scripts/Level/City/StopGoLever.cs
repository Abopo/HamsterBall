using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Basically just rotates the lever into place
public class StopGoLever : MonoBehaviour {
    public float rotSpeed;

    bool _isLeft;
    public bool IsLeft {
        get { return _isLeft; }
    }

    bool _isRotating;

    StopGoButton _button;


    // Start is called before the first frame update
    void Start() {
        _isLeft = true;
    }

    // Update is called once per frame
    void Update() {
        if(_isRotating) {
            if(_isLeft) {
                // Rotate to the right
                transform.Rotate(0f, 0f, -rotSpeed * Time.deltaTime);
                if (Mathf.Abs(transform.rotation.eulerAngles.z - 255f) < 2f) {
                    transform.rotation = Quaternion.Euler(0f, 0f, -105f);
                    _isLeft = false;
                    EndRotation();
                }
            } else {
                // Rotate left
                transform.Rotate(0f, 0f, rotSpeed * Time.deltaTime);
                if (transform.rotation.eulerAngles.z < 2f) {
                    transform.rotation = Quaternion.identity;
                    _isLeft = true;
                    EndRotation();
                }
            }
        }
    }

    public void ChangePosition(StopGoButton invoker) {
        _button = invoker;
        _isRotating = true;
    }

    void EndRotation() {
        _isRotating = false;
        _button.FinishPress();
    }
}
