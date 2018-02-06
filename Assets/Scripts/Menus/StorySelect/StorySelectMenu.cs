using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorySelectMenu : MonoBehaviour {

    public Text location;
    public Image locationImage;
    public Text gameType;
    public Text highscoreHeader;
    public Text highscore;
    public Text winCondition;

    Dictionary<string, Sprite> locationImages = new Dictionary<string, Sprite>();
	// Use this for initialization
	void Start () {
		locationImages["Forest"] = Resources.Load<Sprite>("Art/UI/OneTube - Forest");
        locationImages["Laboratory"] = Resources.Load<Sprite>("Art/UI/TwoTubes - Laboratory");
        locationImages["Sewers"] = Resources.Load<Sprite>("Art/UI/TwoTubes - Sewers");
        locationImages["City"] = Resources.Load<Sprite>("Art/UI/TwoTubes - City");
        locationImages["Fungals"] = Resources.Load<Sprite>("Art/UI/OneTube - Fungals");
        locationImages["DarkForest"] = Resources.Load<Sprite>("Art/UI/OneTube - DarkForest");
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void UpdateUI(StoryButton storyButton) {
        // Set location name and image
        location.text = storyButton.locationName;
        locationImage.sprite = locationImages[storyButton.locationName];

        // Set game type text
        switch(storyButton.gameType) {
            case GAME_MODE.MP_VERSUS:
                gameType.text = "Versus";
                highscoreHeader.text = "Highscore";
                highscore.text = PlayerPrefs.GetInt(storyButton.sceneNumber.ToString() + "Highscore").ToString();
                break;
            case GAME_MODE.SP_POINTS:
                gameType.text = "Point Challenge";
                highscoreHeader.text = "Best Time";
                highscore.text = PlayerPrefs.GetInt(storyButton.sceneNumber.ToString() + "Highscore").ToString("0:00");
                break;
            case GAME_MODE.SP_MATCH:
                gameType.text = "Match Challenge";
                highscoreHeader.text = "Best Time";
                highscore.text = PlayerPrefs.GetInt(storyButton.sceneNumber.ToString() + "Highscore").ToString("0:00");
                break;
        }

        // TODO: Set highscore


        // Set win condition text
        winCondition.text = storyButton.winCondition;
    }
}
