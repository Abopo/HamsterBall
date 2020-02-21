﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWindow : MonoBehaviour {

    public int num;
    public SuperTextMesh pressAnyButton;
    public SpriteRenderer charaPortrait;
    public SuperTextMesh charaName;
    public SuperTextMesh pNum;
    public SuperTextMesh comText;
    public PullDownWindow pullDownWindow;
    public GameObject colorArrows;

    public CSPlayerController PlayerController {
        get { return pullDownWindow.PlayerController; }
    }
    public Animator CharaAnimator {
        get { return pullDownWindow.PlayerController.Animator; }
    }

    bool _active = false;

    TeamBox[] _teamBoxes;

    private void Awake() {
    }
    // Use this for initialization
    void Start () {

        _teamBoxes = FindObjectsOfType<TeamBox>();

        // If we're on the right side
        if (num >= 2) {
            // Make sure the character is facing left properly
            pullDownWindow.PlayerController.FaceLeft();
        }

        if (!_active) {
            // Make sure associated sprites are hidden
            Deactivate();
        }
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

        Debug.Log("Chara Window Activate");

        pullDownWindow.PlayerController.Animator.gameObject.SetActive(true);
        pullDownWindow.PlayerController.Animator.SetBool("FacingRight", pullDownWindow.PlayerController.FacingRight);

        charaPortrait.enabled = true;
        charaName.gameObject.SetActive(true);
        pressAnyButton.gameObject.SetActive(false);
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
        Debug.Log("Chara Window Deactivate");

        pullDownWindow.PlayerController.Animator.gameObject.SetActive(false);
        charaPortrait.enabled = false;
        charaName.gameObject.SetActive(false);
        pNum.gameObject.SetActive(false);
        pressAnyButton.gameObject.SetActive(true);
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
