using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour {

    public Sprite tabImageBig;
    public Sprite tabImageSmall;
    public Sprite iconImageBig;
    public Sprite iconImageSmall;

    Image _tabImage;
    Image _iconImage;

    Canvas _canvas;

    private void Awake() {
        _tabImage = GetComponent<Image>();
        _iconImage = transform.Find("Icon").GetComponent<Image>();
        _canvas = GetComponent<Canvas>();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Select() {
        // Set our images to the big size
        _tabImage.sprite = tabImageBig;
        _tabImage.SetNativeSize();
        _iconImage.sprite = iconImageBig;
        _iconImage.SetNativeSize();

        // Push our image forward
        _canvas.sortingOrder = -9;
    }

    public void Deselect() {
        // Set our images to the small size
        _tabImage.sprite = tabImageSmall;
        _tabImage.SetNativeSize();
        _iconImage.sprite = iconImageSmall;
        _iconImage.SetNativeSize();

        // Push our image back
        _canvas.sortingOrder = -11;
    }
}
