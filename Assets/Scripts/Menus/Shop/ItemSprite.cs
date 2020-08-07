using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

// The sprite for shop items, set's itself up based on the item name
public class ItemSprite : MonoBehaviour {
    ShopItem _curItem;

    Animator _mainAnimator;
    Animator _subAnimator;

    SpriteRenderer _spriteRenderer;
    SpriteRenderer _secondarySprite;

    private void Awake() {
        _mainAnimator = GetComponent<Animator>();
        _subAnimator = GetComponentInChildren<Animator>();

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _secondarySprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    void LoadPaletteData() {

    }

    // Update is called once per frame
    void Update() {
        
    }

    public void SetItem(ShopItem item) {
        _curItem = item;

        if(_curItem.ItemInfo.itemType == "Palette") {
            PaletteSetup();
        } else if(_curItem.ItemInfo.itemType == "Stage") {
            StageSetup();
        } else {
            BasicSetup();
        }
    }

    // Sets up a character palette item
    void PaletteSetup() {
        _mainAnimator.runtimeAnimatorController = _curItem.animator;
        _spriteRenderer.material = _curItem.icon.material;
        _secondarySprite.enabled = true;
    }

    void StageSetup() {

    }

    void BasicSetup() {
        _spriteRenderer.sprite = _curItem.icon.sprite;
        _spriteRenderer.color = _curItem.icon.color;
        _spriteRenderer.material = _curItem.icon.material;

        _secondarySprite.enabled = false;

        _mainAnimator.runtimeAnimatorController = null;
    }
}
