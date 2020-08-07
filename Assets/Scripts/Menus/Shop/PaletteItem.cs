using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class PaletteItem : ShopItem {

    protected override void Awake() {
        base.Awake();
    }
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }

    protected override void LoadIcon() {
        base.LoadIcon();

        string animatorPath = "Art/Animations/Player/";
        string palettePath = "";
        string paletteNum = new String(_itemInfo.itemName.Where(Char.IsDigit).ToArray());

        Sprite[] characterIcons = Resources.LoadAll<Sprite>("Art/UI/Character Select/Character-Icons-False-Colors-Master-File");
        if (_itemInfo.itemName.Contains("Kaden")) {
            icon.sprite = characterIcons[1];
            animatorPath += "Boy/Animation Objects/Boy1";
            palettePath = "Materials/Character Palettes/Boy/Boy" + paletteNum;
        } else if (_itemInfo.itemName.Contains("Quinn")) {
            icon.sprite = characterIcons[4];
            animatorPath += "Girl/Animation Objects/Girl1";
            palettePath = "Materials/Character Palettes/Girl/Girl" + paletteNum;
        } else if (_itemInfo.itemName.Contains("Gail")) {
            icon.sprite = characterIcons[8];
            animatorPath += "Owl/Animation Objects/Owl";
            palettePath = "Materials/Character Palettes/Owl/Owl" + paletteNum;
        } else if (_itemInfo.itemName.Contains("Bexal")) {
            icon.sprite = characterIcons[5];
            animatorPath += "Bexal/Animation Objects/Goat";
            palettePath = "Materials/Character Palettes/Goat/Goat" + paletteNum;
        } else if (_itemInfo.itemName.Contains("Don")) {
            icon.sprite = characterIcons[10];
            animatorPath += "Snail/Animation Objects/Snail1";
            palettePath = "Materials/Character Palettes/Snail/Snail" + paletteNum;
        } else if (_itemInfo.itemName.Contains("Jodi")) {
            icon.sprite = characterIcons[7];
            animatorPath += "Lizard/Animation Objects/Lizard1";
            palettePath = "Materials/Character Palettes/Lizard/Lizard" + paletteNum;
        } else if (_itemInfo.itemName.Contains("Rooben")) {
            icon.sprite = characterIcons[9];
            animatorPath += "Rooster/Animation Objects/Rooster1";
            palettePath = "Materials/Character Palettes/Rooster/Rooster" + paletteNum;
        } else if (_itemInfo.itemName.Contains("Carmela")) {
            icon.sprite = characterIcons[0];
            animatorPath += "Bat/Animation Objects/Bat1";
            palettePath = "Materials/Character Palettes/Bat/Bat" + paletteNum;
        } else if (_itemInfo.itemName.Contains("Lackey")) {
            icon.sprite = characterIcons[6];
            animatorPath += "Lackey/Animation Objects/Lackey" + paletteNum;
        }

        icon.SetNativeSize();
        if (palettePath != "") {
            icon.material = Resources.Load(palettePath) as Material;
        } else {
            icon.material = new Material(Shader.Find("Sprites/Default"));
        }

        animator = Resources.Load(animatorPath) as RuntimeAnimatorController;
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }
}
