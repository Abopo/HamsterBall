using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMarker : MonoBehaviour {
    public int team;
    public bool isFilledIn = false;

    public Sprite _emptySprite;
    public Sprite _filledInSprite;

    SpriteRenderer _spriteRenderer;

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_emptySprite == null) {
            _emptySprite = Resources.LoadAll<Sprite>("Art/UI/Level UI/Tally")[0];
        }
        if (_filledInSprite == null) {
            _filledInSprite = Resources.LoadAll<Sprite>("Art/UI/Level UI/Tally")[3];
        }
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if(isFilledIn) {
            //Debug.Log(isFilledIn.ToString());

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
    public void FillOut() {
        isFilledIn = false;
        _spriteRenderer.sprite = _emptySprite;
    }
}
