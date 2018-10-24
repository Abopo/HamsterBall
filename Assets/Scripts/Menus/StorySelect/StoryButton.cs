﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryButton : MenuOption {
    public bool hasCutscene;
    public string fileToLoad;

    public bool isLocked = true;

    public string sceneNumber;
    public string locationName;
    public GAME_MODE gameType;
    public string winCondition;

    StorySelectMenu _storySelectMenu;
    BoardLoader _boardLoader;

    private void Awake() {
        _storySelectMenu = FindObjectOfType<StorySelectMenu>();
        _boardLoader = FindObjectOfType<BoardLoader>();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();

        // Don't move to this button if it's locked
        if (isLocked) {
            isReady = false;
        } else {
            isReady = true;
        }
	}

    protected override void Select() {
        //base.Select();
        if(!isReady) {
            // TODO: Play some sound to indicate the stage is locked
            return;
        }

        FindObjectOfType<GameManager>().stage = sceneNumber;

        // Set the new story position to here
        PlayerPrefs.SetString("StoryPos", sceneNumber);

        if (hasCutscene) {
            // Load a cutscene
            CutsceneManager.fileToLoad = fileToLoad;
            SceneManager.LoadScene("Cutscene");
        } else {
            // Load a board
            _boardLoader.ReadBoardSetup(fileToLoad);
        }
    }

    public override void Highlight() {
        if(isReady) {
            base.Highlight();

            // Change UI to display stuff about this event
            _storySelectMenu.UpdateUI(this);
        }
    }

    public void Unlock() {
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        isLocked = false;
    }
}
