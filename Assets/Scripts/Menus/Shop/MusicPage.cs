using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPage : ShopPage {

    protected override void Awake() {
        base.Awake();

        _shopItemObj = Resources.Load("Prefabs/Menus/Shop/MusicItem") as GameObject;
    }

    // Start is called before the first frame update
    protected override void Start() {
        CreateShopItems(_shopMenu.ShopData.musicData);

        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

    }

    public override void TakeFocus() {
        base.TakeFocus();

        _shopMenu.EnableActionButton();
    }

    protected override void UnlockItem() {
        base.UnlockItem();

        UnlockMusic();
    }

    void UnlockMusic() {
        ES3.Save<bool>(_curItem.ItemInfo.itemName + " Track", true);
    }
}
