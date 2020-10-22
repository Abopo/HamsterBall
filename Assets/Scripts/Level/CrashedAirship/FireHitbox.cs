using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FireHitbox : MonoBehaviour {

    Transform _sizer;
    BoxCollider2D _collider;
    Vector2 _startPos;   

    bool _isActive;
    bool _isExpanding;
    bool _isShrinking;

    // temp variables
    float tempCenter;
    float tempWidth;

    private void Awake() {
        _sizer = transform.GetChild(0);
        _collider = GetComponent<BoxCollider2D>();
        _startPos = transform.localPosition;
    }
    // Start is called before the first frame update
    void Start() {
        _collider.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        if (_isActive) {
            if (_isExpanding) {
                // Move the sizer forward (speed should match the fire's particle effect)
                _sizer.transform.Translate(30f * Time.deltaTime, 0f, 0f);
                if (_sizer.transform.localPosition.x >= 10f) {
                    _sizer.transform.localPosition = new Vector3(10f, _sizer.transform.localPosition.y, _sizer.transform.localPosition.z);
                    _isExpanding = false;
                }
            }
            if (_isShrinking) {
                // Move self forward (see above) {
                transform.Translate(30f * Mathf.Sign(transform.localScale.x) * Time.deltaTime, 0f, 0f);
                if (transform.localPosition.x >= 10f || transform.localPosition.x <= -10f) {
                    transform.localPosition = new Vector3(10f, transform.localPosition.y, transform.localPosition.z);

                    // Sequence is finished
                    ResetState();
                }
            }

            DetermineSize();
        }
    }

    void ResetState() {
        _isActive = false;
        _isExpanding = false;
        _isShrinking = false;

        transform.localPosition = _startPos;
        _sizer.transform.localPosition = new Vector3(1f, 0f);

        _collider.enabled = false;
    }

    void DetermineSize() {
        // The size of the fire hitbox is based on the distance between itself and the sizer

        // We need to position the collider in the center point between
        // Since the sizer is a child, we can just use half it's X pos to find the center
        tempCenter = _sizer.localPosition.x / 2f;
        tempCenter = Mathf.Abs(tempCenter);
        _collider.offset = new Vector2(tempCenter, _collider.offset.y);

        // We can use the same value to set the width as well?
        _collider.size = new Vector2(tempCenter*2f, _collider.size.y);
    }

    public void FireStart() {
        _isActive = true;
        _isExpanding = true;

        _collider.enabled = true;
    }

    public void FireEnd() {
        _isShrinking = true;
    }
}
