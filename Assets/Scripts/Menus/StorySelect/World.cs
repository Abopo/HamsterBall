using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public int worldNum;

    StoryButton[] _storyButtons;
    StorySelectMenu _storySelectMenu;

    private void Awake() {
        _storyButtons = GetComponentsInChildren<StoryButton>();
        _storySelectMenu = FindObjectOfType<StorySelectMenu>();
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Unlocks levels up to the passed level
    public void Unlock(int level) {
        for(int i = 0; i < level; ++i) {
            _storyButtons[i].GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
    }

    public void Activate(int level) {
        int l;
        if (worldNum == _storySelectMenu.FurthestWorld) {
            l = _storySelectMenu.FurthestLevel;
        } else {
            l = _storyButtons.Length;
        }

        for(int i = 0; i < l; ++i) {
            _storyButtons[i].isReady = true;
        }

        _storyButtons[level].Highlight();
    }

    public void Deactivate() {
        foreach (StoryButton sb in _storyButtons) {
            sb.isReady = false;
        }
    }
}
