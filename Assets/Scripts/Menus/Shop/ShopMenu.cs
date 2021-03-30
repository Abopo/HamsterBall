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
    ConfirmPurchaseMenu _confirmMenu;

    ShopData _shopData;
    public ShopData ShopData {
        get { return _shopData; }
    }

    bool IsActive {
        get { return !_exitMenu.menuObj.activeSelf && !_confirmMenu.menuObj.activeSelf; }
    }

    protected void Awake() {
        _allPages = GetComponentsInChildren<ShopPage>();
        // Start on palettes
        _curPage = _allPages[0];
        _curPageIndex = 0;

        _exitMenu = FindObjectOfType<ExitMenu>();
        _confirmMenu = FindObjectOfType<ConfirmPurchaseMenu>();

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

        SetupShopItemAdjOptions();

        ShowFirstPage();

        DisableActionButton();
    }

    void SetupShopItemAdjOptions() {
        // Alright so we gotta have the shop items find their adjOptions
        // but if they're all active it gets fucked up, so we gotta one by one
        // disable all the other pages, then tell the items to find their stuff

        foreach(ShopPage page in _allPages) {
            // Disable all the other pages
            DisableAllPages();

            // Enable this page
            page.ShowContent();

            // Have all of this pages items find their adjOptions
            page.InitializeOptions();
        }

    }

    void DisableAllPages() {
        foreach(ShopPage page in _allPages) {
            page.HideContent();
        }
    }

    void ShowFirstPage() {
        // Show whichever page has focus first, hide the others
        foreach(ShopPage page in _allPages) {
            if(page.hasFocus) {
                page.ShowContent();
            } else {
                page.HideContent();
            }
        }
    }

    // Update is called once per frame
    protected void Update() {
        if (IsActive) {
            if (InputState.GetButtonOnAnyControllerPressed("Cancel")) {
                _exitMenu.Activate();
            }

            if (InputState.GetButtonOnAnyControllerPressed("PageLeft")) {
                // Move page left
                
                MovePageLeft();
            }
            if (InputState.GetButtonOnAnyControllerPressed("PageRight")) {
                // move page right
                MovePageRight();
            }

#if UNITY_EDITOR
            // Dev cheat
            if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.M)) {
                playerCurrency = 5000;
                currencyText.text = playerCurrency.ToString();
            }
#endif
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
        // Hide the curpage
        _curPage.HideContent();
        FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/Page Turn");
        // Change to new page
        _curPage = _allPages[_curPageIndex];
        _curPage.TakeFocus();
    }

    public void PurchaseCurItem() {
        _curPage.PurchaseCurItem();

        currencyText.GetComponent<NumberTick>().StartTick(playerCurrency, playerCurrency - _curPage.CurItem.ItemInfo.price);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/Buy Item");
        // Reduce player currency by the cost
        playerCurrency -= _curPage.CurItem.ItemInfo.price;
        ES3.Save<int>("Currency", playerCurrency);
    }

    public void EnableActionButton() {
        actionButton.SetActive(true);
    }
    public void DisableActionButton() {
        actionButton.SetActive(false);
    }
}
