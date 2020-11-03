﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryButton : MenuButton {
    public bool hasCutscene;
    public string fileToLoad;

    public bool isLocked = true;

    public int[] stageNumber = new int[2];
    public string locationName;
    public GAME_MODE gameType;
    public string winCondition;

    // These are the required time/throws/score the player has to reach in order to earn flowers for the stage
    public int spFlower2Requirement;
    public int spFlower3Requirement;
    public int cpFlower2Requirement;
    public int cpFlower3Requirement;

    public Image flower;

    StorySelectMenu _storySelectMenu;
    BoardLoader _boardLoader;
    GameManager _gameManager;

    protected override void Awake() {
        base.Awake();

        _storySelectMenu = FindObjectOfType<StorySelectMenu>();
        _boardLoader = FindObjectOfType<BoardLoader>();
        _gameManager = GameManager.instance;
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        
        // Don't move to this button if it's locked
        if (isLocked) {
            isReady = false;
        } else {
            isReady = true;
            Unlock();
            SetFlowerData();
        }
    }

    void SetFlowerData() {
        int[,] flowerData;

        if (!_storySelectMenu.storyPlayerInfo.IsCoop) {
            flowerData = ES3.Load<int[,]>("SoloFlowers");
        } else {
            flowerData = ES3.Load<int[,]>("CoopFlowers");
        }

        int flowerCount = flowerData[stageNumber[0] - 1, stageNumber[1] - 1];
        if (flowerCount > 0) {
            flower.gameObject.SetActive(true);
            flower.sprite = _storySelectMenu.resources.hangingFlowerSprites[flowerCount - 1];
        } else {
            flower.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();

        if (isLocked) {
            isReady = false;
        } else {
            isReady = true;
        }
    }

    protected override void Select() {
        //base.Select();
        if(!IsReady) {
            // TODO: Play some sound to indicate the stage is locked
            return;
        }

        // Set the stage in the game manager
        GameManager.instance.stage = stageNumber;

        if (_gameManager.demoMode) {
            ES3.Save<int>("DemoPos", stageNumber[1]);
        } else {
            // Set the new story position to here
            ES3.Save<int[]>("StoryPos", stageNumber);
        }

        // Load the players
        FindObjectOfType<StoryPlayerInfo>().LoadPlayers();

        // TODO: Add separate coop requirements
        // Hold onto the flower requirements
        _gameManager.flowerRequirement1 = spFlower2Requirement;
        _gameManager.flowerRequirement2 = spFlower3Requirement;

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
        if(IsReady) {
            base.Highlight();

            // Change UI to display stuff about this event
            if(_storySelectMenu == null) {
                _storySelectMenu = FindObjectOfType<StorySelectMenu>();
            }
            _storySelectMenu.UpdateUI(this);
        }
    }

    public void Unlock() {
        transform.Find("Locked_Overlay").GetComponent<Image>().enabled = false;
        isLocked = false;
    }
}
