using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageIcon : MonoBehaviour {

    public bool isLocked;
    public string stageName;
    public string stageDescription;

    Vector3[] _startScale = new Vector3[3];
    bool _downScaled = false;

    SpriteRenderer _lockSprite;

    private void Awake() {
        // Get the inital scales of children
        for (int i = 0; i < transform.childCount; ++i) {
            _startScale[i] = transform.GetChild(i).localScale;
        }
    }

    // Use this for initialization
    void Start () {
        _lockSprite = transform.GetChild(2).GetComponent<SpriteRenderer>();

		// See if this stage is unlocked or not
        if(PlayerPrefs.GetInt(stageName) == 1) {
            isLocked = false;
            //_spriteRenderer.color = Color.white;
            _lockSprite.enabled = false;
        } else {
            isLocked = true;
            //_spriteRenderer.color = Color.black;
            _lockSprite.enabled = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
        // Scale children based on distance from 0?
        float scaleFactor;
        float x = (Mathf.Abs(transform.position.x) / 3.5f) * 0.25f;
        scaleFactor = 1f - x;
        if(scaleFactor < 0.75f) {
            scaleFactor = 0.75f;
        }
        for (int i = 0; i < transform.childCount; ++i) {
            transform.GetChild(i).localScale = new Vector3(_startScale[i].x * scaleFactor, _startScale[i].y * scaleFactor, _startScale[i].z);
        }
    }

    public void ScaleDown() {
        if (!_downScaled) {
            Vector3 tempScale;
            for (int i = 0; i < transform.childCount; ++i) {
                tempScale = transform.GetChild(i).localScale;
                transform.GetChild(i).localScale = new Vector3(tempScale.x * 0.75f, tempScale.y * 0.75f, tempScale.z);
            }

            _downScaled = true;
        }
    }

    public void ScaleUp() {
        // Revert back to initial scale
        for (int i = 0; i < transform.childCount; ++i) {
            transform.GetChild(i).localScale = _startScale[i];
        }

        _downScaled = false;
    }
}
