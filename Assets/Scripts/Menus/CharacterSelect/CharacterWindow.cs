using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWindow : MonoBehaviour {

    public int num;
    public CSPlayerController playerController;
    public SuperTextMesh pressAnyButton;
    public Animator charaAnimator;
    public SpriteRenderer charaPortrait;
    public SuperTextMesh charaName;
    public SuperTextMesh pNum;
    public SuperTextMesh comText;
    public PullDownWindow pullDownWindow;
    public GameObject colorArrows;

    bool _active;

    TeamBox[] _teamBoxes;

	// Use this for initialization
	void Start () {
        _teamBoxes = FindObjectsOfType<TeamBox>();

        // Make sure associated sprites are hidden
        Deactivate();
    }

    // Update is called once per frame
    void Update () {
        if (!_active && comText != null) {
            // TODO: Shouldn't really need to do this every frame
            CheckTeams();
        }
	}

    public void Activate(bool ai, int playerNum) {
        _active = true;

        charaAnimator.gameObject.SetActive(true);
        charaPortrait.enabled = true;
        charaName.gameObject.SetActive(true);
        pressAnyButton.enabled = false;
        pNum.gameObject.SetActive(true);
        comText.enabled = false;

        if (ai) {
            pNum.text = "C" + (playerNum+1);
        } else {
            pNum.text = "P" + (playerNum+1);
        }
    }
    public void Deactivate() {
        _active = false;

        charaAnimator.gameObject.SetActive(false);
        charaPortrait.enabled = false;
        charaName.gameObject.SetActive(false);
        pNum.gameObject.SetActive(false);
        pressAnyButton.enabled = true;
    }

    void CheckTeams() {
        // If any team has a player
        foreach (TeamBox tB in _teamBoxes) {
            if (tB.numPlayers > 0) {
                // Make sure our com text is showing
                comText.enabled = true;
                return;
            }
        }

        comText.enabled = false;
    }
}
