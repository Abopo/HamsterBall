using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class PalettesPage : ShopPage {

    protected override void Awake() {
        base.Awake();

        _shopItemObj = Resources.Load("Prefabs/Menus/Shop/PaletteItem") as GameObject;
    }

    // Start is called before the first frame update
    protected override void Start() {
        CreateShopItems(_shopMenu.ShopData.paletteData);

        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

    }

    public override void TakeFocus() {
        base.TakeFocus();

        _shopMenu.DisableActionButton();
    }

    protected override void UnlockItem() {
        base.UnlockItem();

        UnlockPalette();
    }

    void UnlockPalette() {
        string paletteString = new String(_curItem.ItemInfo.itemName.Where(Char.IsDigit).ToArray());
        int paletteNum = int.Parse(paletteString);

        bool[] characterPaletteData;

        // Save palette based on the character
        if (_curItem.ItemInfo.itemName.Contains("Kaden")) {
            paletteString = "BoyPalettes";
        } else if (_curItem.ItemInfo.itemName.Contains("Quinn")) {
            paletteString = "GirlPalettes";
        } else if (_curItem.ItemInfo.itemName.Contains("Gail")) {
            paletteString = "OwlPalettes";
        } else if (_curItem.ItemInfo.itemName.Contains("Bexal")) {
            paletteString = "GoatPalettes";
        } else if (_curItem.ItemInfo.itemName.Contains("Don")) {
            paletteString = "SnailPalettes";
        } else if (_curItem.ItemInfo.itemName.Contains("Jodi")) {
            paletteString = "LizardPalettes";
        } else if (_curItem.ItemInfo.itemName.Contains("Rooben")) {
            paletteString = "RoosterPalettes";
        } else if (_curItem.ItemInfo.itemName.Contains("Carmela")) {
            paletteString = "BatPalettes";
        } else if (_curItem.ItemInfo.itemName.Contains("Lackey")) {
            paletteString = "LackeyPalettes";
        } else {
            Debug.LogError("Character palette data not found");
            return;
        }

        characterPaletteData = ES3.Load<bool[]>(paletteString, new bool[0]);
        characterPaletteData[paletteNum - 2] = true;
        ES3.Save<bool[]>(paletteString, characterPaletteData);
    }
}
