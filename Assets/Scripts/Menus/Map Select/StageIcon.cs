using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageIcon : MonoBehaviour {

    public bool isLocked;
    public string stageName;

    SpriteRenderer _spriteRenderer;
    SpriteRenderer _lockSprite;

	// Use this for initialization
	void Start () {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _lockSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();

		// See if this stage is unlocked or not
        if(PlayerPrefs.GetInt(stageName) == 1) {
            isLocked = false;
            _spriteRenderer.color = Color.white;
            _lockSprite.enabled = false;
        } else {
            isLocked = true;
            _spriteRenderer.color = Color.black;
            _lockSprite.enabled = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
