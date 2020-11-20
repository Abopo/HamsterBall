using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsTab : MonoBehaviour {

    Image _bigTab;
    Image _smallTab;
    Transform _tabInfo;

    private void Awake() {
        _bigTab = transform.Find("Big Tab").GetComponent<Image>();
        _smallTab = transform.Find("Small Tab").GetComponent<Image>();
        _tabInfo = transform.Find("Tab Info");
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Select() {
        // Turn off small tab
        _smallTab.enabled = false;

        // Turn on big tab
        _bigTab.enabled = true;

        // Scale up info
        _tabInfo.localScale = new Vector3(1f, 1f, 1f);
    }

    public void Deselect() {
        // Turn off big tab
        _bigTab.enabled = false;

        // Turn on small tab
        _smallTab.enabled = true;

        // Scale down info
        _tabInfo.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }
}
