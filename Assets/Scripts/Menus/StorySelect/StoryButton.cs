using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryButton : MonoBehaviour {
    public bool hasCutscene;
    public string fileToLoad;

    public string locationName;
    public GAME_MODE gameType;
    public string winCondition;

    StorySelectMenu _storySelectMenu;
    BoardLoader _boardLoader;
	// Use this for initialization
	void Start () {
        _storySelectMenu = GameObject.FindObjectOfType<StorySelectMenu>();
        _boardLoader = GameObject.FindObjectOfType<BoardLoader>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Select() {
        if(hasCutscene) {
            // Load a cutscene
            CutsceneManager.fileToLoad = fileToLoad;
            SceneManager.LoadScene("Cutscene");
        } else {
            // Load a board
            _boardLoader.ReadBoardSetup(fileToLoad);
        }
    }

    public void Highlight() {
        // Change UI to display stuff about this event
        _storySelectMenu.UpdateUI(this);
    }
}
