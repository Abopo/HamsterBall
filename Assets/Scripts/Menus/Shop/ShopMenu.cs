using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour {

    public Image itemImage;
    public SuperTextMesh itemCost;
    public SuperTextMesh itemDescription;

    public int playerCurrency;
    public SuperTextMesh currencyText;
    public GameObject actionButton;

    ShopPage[] _allPages;
    ShopPage _curPage;
    int _curPageIndex;

    ExitMenu _exitMenu;

    ShopData _shopData;
    public ShopData ShopData {
        get { return _shopData; }
    }

    protected void Awake() {
        _allPages = GetComponentsInChildren<ShopPage>();
        // Start on palettes
        _curPage = _allPages[0];
        _curPageIndex = 0;

        _exitMenu = FindObjectOfType<ExitMenu>();

        // Load the shop data
#if UNITY_EDITOR
        _shopData = ShopData.Load(Path.Combine(Application.dataPath, "Resources/Text/Shop/ShopItemData.xml"));
#else
        _shopData = ShopData.Load(Path.Combine(Application.dataPath, "ShopItemData.xml"));
#endif
    }

    // Start is called before the first frame update
    protected void Start() {
        playerCurrency = ES3.Load<int>("Currency", 100);
        currencyText.text = playerCurrency.ToString();

        DisableActionButton();
    }
    // Update is called once per frame
    protected void Update() {
        if (InputState.GetButtonOnAnyControllerPressed("Cancel")) {
            _exitMenu.Activate();
        }

        if(Input.GetKeyDown(KeyCode.Q)) {
            // Move page left
            MovePageLeft();
        }
        if(Input.GetKeyDown(KeyCode.E)) {
            // move page right
            MovePageRight();
        }

        // Dev cheat
        if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.M)) {
            playerCurrency = 5000;
            currencyText.text = playerCurrency.ToString();
        }
    }

    void MovePageLeft() {
        _curPageIndex--;
        if (_curPageIndex < 0) {
            _curPageIndex = _allPages.Length-1;
        }

        UpdatePage();
    }

    void MovePageRight() {
        _curPageIndex++;
        if (_curPageIndex >= _allPages.Length) {
            _curPageIndex = 0;
        }

        UpdatePage();
    }

    void UpdatePage() {
        _curPage = _allPages[_curPageIndex];
        _curPage.TakeFocus();
    }

    public void PurchaseCurItem() {
        _curPage.PurchaseCurItem();

        // Reduce player currency by the cost
        playerCurrency -= _curPage.CurItem.ItemInfo.price;
        currencyText.text = playerCurrency.ToString();
        ES3.Save<int>("Currency", playerCurrency);
    }

    public void EnableActionButton() {
        actionButton.SetActive(true);
    }
    public void DisableActionButton() {
        actionButton.SetActive(false);
    }
}
