using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageIcon : MonoBehaviour {

    public bool isLocked;
    public string stageName;

    SpriteRenderer _lockSprite;

	// Use this for initialization
	void Start () {
        _lockSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();

		// See if this stage is unlocked or not
        if(PlayerPrefs.GetInt(stageName) == 1) {
            isLocked = false;
            _lockSprite.enabled = false;
        } else {
            isLocked = true;
            _lockSprite.enabled = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
