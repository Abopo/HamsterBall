using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMarker : MonoBehaviour {
    public int team;
    public bool isFilledIn = false;

    Sprite _filledInSprite;

    SpriteRenderer _spriteRenderer;

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    void Start () {
        _filledInSprite = Resources.LoadAll<Sprite>("Art/UI/Level UI/Tally")[3];
	}
	
	// Update is called once per frame
	void Update () {
        if(isFilledIn) {
            //Debug.Log(isFilledIn.ToString());

            if (Input.GetKeyDown(KeyCode.O)) {
                FillOut();
                Debug.Log(_spriteRenderer.sprite.ToString());
            }
            if (Input.GetKeyDown(KeyCode.I)) {
                FillIn();
                Debug.Log(_spriteRenderer.sprite.ToString());
            }
            if (_spriteRenderer.sprite == null || _spriteRenderer.sprite != _filledInSprite) {
                Debug.Log("Sprite wrong, resetting");
                _spriteRenderer.sprite = _filledInSprite;
            }
        }
    }

    public void FillIn() {
        //Debug.Log("Filled In");
        isFilledIn = true;
        //Debug.Log(isFilledIn.ToString());
        _spriteRenderer.sprite = _filledInSprite;
    }
    void FillOut() {
        isFilledIn = false;
        _spriteRenderer.sprite = Resources.LoadAll<Sprite>("Art/UI/Level UI/Tally")[0];
    }
}
