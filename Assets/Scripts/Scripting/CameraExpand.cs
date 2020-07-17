using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraExpand : ScriptingController {

    Camera _camera;

    bool _expanding1;
    bool _expanding2;

    // Data for first expand size
    float yPos1 = 0.34f;
    float orthoSize1 = 7.45f;
    float x1 = 0.25f;
    float w1 = 0.47f;

    // Data for the second expand size
    float yPos2 = 0.83f;
    float xPos2 = 0;
    float orthoSize2 = 8f;
    float x2 = 0f;
    float w2 = 1f;

    // distance to desired size
    float moveYDist = 0f;
    float moveXDist = 0f;
    float orthoDist = 0f;
    float rectXDist = 0f;
    float rectWDist = 0f;

    protected override void Awake() {
        _camera = GetComponent<Camera>();
    }
    // Start is called before the first frame update
    protected override void Start() {
        
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if(_expanding1) {
            // Move y pos
            transform.Translate(0f, moveYDist * Time.deltaTime, 0f);

            Expand();

            if(Mathf.Abs(transform.position.y - yPos1) < 0.1f) {
                transform.position = new Vector3(transform.position.x, yPos1, transform.position.z);
                _camera.orthographicSize = orthoSize1;
                _camera.rect = new Rect(x1, _camera.rect.y, w1, _camera.rect.height);
                _expanding1 = false;

                FindObjectOfType<PlayerMoveUp>().Throw1();
            }
        } else if(_expanding2) {
            // Move x pos
            transform.Translate(moveXDist * Time.deltaTime, moveYDist * Time.deltaTime, 0f);

            Expand();

            if (Mathf.Abs(transform.position.x - xPos2) < 0.1f) {
                transform.position = new Vector3(xPos2, yPos2, transform.position.z);
                _camera.orthographicSize = orthoSize2;
                _camera.rect = new Rect(x2, _camera.rect.y, w2, _camera.rect.height);
                _expanding2 = false;

                FindObjectOfType<TrailerOpeningScript>().End();
            }
        }
    }

    void Expand() {
        // change ortho size
        _camera.orthographicSize += orthoDist * Time.deltaTime;

        // change rect
        _camera.rect = new Rect(_camera.rect.x + rectXDist * Time.deltaTime,
                                _camera.rect.y,
                                _camera.rect.width + rectWDist * Time.deltaTime,
                                _camera.rect.height);
    }

    public override void Begin() {
        base.Begin();
        Expand1();
    }

    void Expand1() {
        _expanding1 = true;

        moveYDist = yPos1 - transform.position.y;
        orthoDist = orthoSize1 - _camera.orthographicSize;
        rectXDist = x1 - _camera.rect.x;
        rectWDist = w1 - _camera.rect.width;
    }

    public void Expand2() {
        _expanding2 = true;

        moveYDist = yPos2 - transform.position.y;
        moveXDist = xPos2 - transform.position.x;
        orthoDist = orthoSize2 - _camera.orthographicSize;
        rectXDist = x2 - _camera.rect.x;
        rectWDist = w2 - _camera.rect.width;
    }

    void Wait() {
        _expanding1 = false;
    }
}
