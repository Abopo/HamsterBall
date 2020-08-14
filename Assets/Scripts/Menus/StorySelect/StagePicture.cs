using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StagePicture : MonoBehaviour {
    public Image locationImageMP;

    public Image locationImageTop;
    public Image locationImageMid;
    public Image locationImageBot;

    public Image border;

    Dictionary<string, Sprite> locationImagesMP = new Dictionary<string, Sprite>();
    Dictionary<string, Sprite> locationImagesSP = new Dictionary<string, Sprite>();
    Dictionary<string, Sprite> borderImages = new Dictionary<string, Sprite>();

    BoardDisplay _boardDisplay;

    private void Awake() {
        // Multiplayer images
        locationImagesMP["Forest"] = Resources.Load<Sprite>("Art/UI/Map Select/ForestImage");
        locationImagesMP["Mountain"] = Resources.Load<Sprite>("Art/UI/Map Select/MountainMap");
        locationImagesMP["Beach"] = Resources.Load<Sprite>("Art/UI/Map Select/Beach-Picture");
        locationImagesMP["City"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - City");
        locationImagesMP["Sewers"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - Sewers");
        locationImagesMP["Laboratory"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - Laboratory");
        locationImagesMP["Fungals"] = Resources.Load<Sprite>("Art/UI/Map Select/OneTube - Fungals");
        locationImagesMP["DarkForest"] = Resources.Load<Sprite>("Art/UI/Map Select/OneTube - DarkForest");
        locationImagesMP["Airship"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - Space");

        // Single player images
        locationImagesSP["Forest"] = Resources.Load<Sprite>("Art/UI/Map Select/ForestImage");
        locationImagesSP["Mountain"] = Resources.Load<Sprite>("Art/UI/Map Select/MountainBoard");
        locationImagesSP["Beach"] = Resources.Load<Sprite>("Art/UI/Map Select/Beach-Picture");
        locationImagesSP["City"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - City");
        locationImagesSP["Sewers"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - Sewers");
        locationImagesSP["Laboratory"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - Laboratory");
        locationImagesSP["Fungals"] = Resources.Load<Sprite>("Art/UI/Map Select/OneTube - Fungals");
        locationImagesSP["DarkForest"] = Resources.Load<Sprite>("Art/UI/Map Select/OneTube - DarkForest");
        locationImagesSP["Airship"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - Space");

        // Border images
        Sprite[] borders = Resources.LoadAll<Sprite>("Art/UI/Map Select/Stage-Selection-List");
        borderImages["Forest"] = borders[1];
        borderImages["Mountain"] = borders[2];
        borderImages["Beach"] = borders[0];

        _boardDisplay = FindObjectOfType<BoardDisplay>();
    }

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void UpdateImages(StoryButton storyButton) {
        if(storyButton.gameType == GAME_MODE.MP_VERSUS) {
            SetupMultiplayer(storyButton);
        } else {
            SetupSinglePlayer(storyButton);
        }

        SetBorder(storyButton);
    }

    void SetupMultiplayer(StoryButton storyButton) {
        // Hide single player images
        DeactivateSP();

        // Show the single player location image
        locationImageMP.transform.parent.gameObject.SetActive(true);
        // Set location image
        locationImageMP.sprite = locationImagesMP[storyButton.locationName];
    }

    void SetupSinglePlayer(StoryButton storyButton) {
        // Hide the multiplayer image
        locationImageMP.transform.parent.gameObject.SetActive(false);

        // Show the single player location image
        locationImageTop.transform.parent.gameObject.SetActive(true);
        // Set location image
        locationImageTop.sprite = locationImagesSP[storyButton.locationName];

        // If it's a clear stage
        if (storyButton.gameType == GAME_MODE.SP_CLEAR) {
            // Display the backer images

            // Show the single player location image
            locationImageMid.transform.parent.gameObject.SetActive(true);
            // Set location image
            locationImageMid.sprite = locationImagesSP[storyButton.locationName];
            // Show the single player location image
            locationImageBot.transform.parent.gameObject.SetActive(true);
            // Set location image
            locationImageBot.sprite = locationImagesSP[storyButton.locationName];
        } else {
            locationImageMid.transform.parent.gameObject.SetActive(false);
            locationImageBot.transform.parent.gameObject.SetActive(false);
        }

        // Display a preview of the stage
        //_boardDisplay.LoadBoard(storyButton.fileToLoad);
    }

    public void DeactivateSP() {
        locationImageTop.transform.parent.gameObject.SetActive(false);
        locationImageMid.transform.parent.gameObject.SetActive(false);
        locationImageBot.transform.parent.gameObject.SetActive(false);

        // Clear any previews that may be showing
        _boardDisplay.ClearBoard();
    }

    void SetBorder(StoryButton storyButton) {
        border.sprite = borderImages[storyButton.locationName];
    }
}
