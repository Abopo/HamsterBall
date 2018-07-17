using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Displays text of a power up's effects
public class PowerUpText : MonoBehaviour {

    Text _powerUpText;
    bool _isDisplaying;

    float _displayTime = 1.0f;
    float _displayTimer;

	// Use this for initialization
	void Start () {
        _powerUpText = GetComponent<Text>();
        _isDisplaying = false;
	}
	
	// Update is called once per frame
	void Update () {
        // If we're facing backwards
        if (transform.lossyScale.x < 0) {
            // Flip back
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        if (_isDisplaying) {
            transform.Translate(0f, 0.5f * Time.deltaTime, 0f);

            _displayTimer += Time.deltaTime;
            if(_displayTimer >= _displayTime) {
                HideText();
            }
        }
	}

    public void SetText(string text) {
        _powerUpText.text = text;
    }

    public void DisplayText() {
        _isDisplaying = true;
        _powerUpText.enabled = true;
        _displayTimer = 0f;
    }

    void HideText() {
        _isDisplaying = false;
        _powerUpText.enabled = false;

        DestroyObject(this.gameObject);
    }
}
