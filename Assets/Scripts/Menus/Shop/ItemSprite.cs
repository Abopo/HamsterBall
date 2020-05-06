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

    private void Awake() {
        _mainAnimator = GetComponent<Animator>();
        _subAnimator = GetComponentInChildren<Animator>();

        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void SetItem(ShopItem item) {
        _curItem = item;

        if(_curItem.ItemInfo.itemName.Contains("Palette")) {
            PaletteSetup();
        } else if(_curItem.ItemInfo.itemName.Contains("Stage")) {
            StageSetup();
        } else {
            BasicSetup();
        }
    }

    // Sets up a character palette item
    void PaletteSetup() {
        string animatorPath = "Art/Animations/Player/";
        string palettePath = "";

        string paletteNum = new String(_curItem.ItemInfo.itemName.Where(Char.IsDigit).ToArray());

        // Load animators based on the character
        if (_curItem.ItemInfo.itemName.Contains("Kaden")) {
            animatorPath += "Boy/Animation Objects/Boy1";
            palettePath = "Materials/Character Palettes/Boy/Boy" + paletteNum;
        } else if(_curItem.ItemInfo.itemName.Contains("Quinn")) {
            animatorPath += "Girl/Animation Objects/Girl1";
            palettePath = "Materials/Character Palettes/Girl/Girl" + paletteNum;
        } else if(_curItem.ItemInfo.itemName.Contains("Gail")) {
            animatorPath += "Owl/Animation Objects/Owl1";
            palettePath = "Materials/Character Palettes/Owl/Owl" + paletteNum;
        } else if (_curItem.ItemInfo.itemName.Contains("Bexal")) {
            animatorPath += "Bexal/Animation Objects/Goat1";
            palettePath = "Materials/Character Palettes/Goat/Goat" + paletteNum;
        } else if (_curItem.ItemInfo.itemName.Contains("Don")) {
            animatorPath += "Snail/Animation Objects/Snail1";
            palettePath = "Materials/Character Palettes/Snail/Snail" + paletteNum;
        } else if (_curItem.ItemInfo.itemName.Contains("Jodi")) {
            animatorPath += "Lizard/Animation Objects/Lizard1";
            palettePath = "Materials/Character Palettes/Lizard/Lizard" + paletteNum;
        } else if (_curItem.ItemInfo.itemName.Contains("Rooben")) {
            animatorPath += "Rooster/Animation Objects/Rooster1";
            palettePath = "Materials/Character Palettes/Rooster/Rooster" + paletteNum;
        } else if (_curItem.ItemInfo.itemName.Contains("Carmela")) {
            animatorPath += "Bat/Animation Objects/Bat1";
            palettePath = "Materials/Character Palettes/Bat/Bat" + paletteNum;
        } else if (_curItem.ItemInfo.itemName.Contains("Lackey")) {
            animatorPath += "Lackey/Animation Objects/Lackey" + paletteNum;
            // Make sure line sprite is turned off
            GetComponentInChildren<LinesSprite>().turnOff = true;
        }

        _mainAnimator.runtimeAnimatorController = Resources.Load(animatorPath) as RuntimeAnimatorController;
        // TODO: gotta set animator to idle

        if (palettePath != "") {
            _spriteRenderer.material = Resources.Load(palettePath) as Material;
        } else {
            _spriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
    }

    void StageSetup() {

    }

    void BasicSetup() {
        _spriteRenderer.sprite = _curItem.ItemInfo.itemSprite;
    }
}
