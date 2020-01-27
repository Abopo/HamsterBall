﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class CharacterSelector : MonoBehaviour {
    public CharacterWindow charaWindow;
    public CharacterIcon curCharacterIcon;

    public bool isActive = false;
    public int playerNum = -1;
    public bool lockedIn = false;
    public bool isReady = false;
    public bool takeInput = true;
    public bool isAI = false;
    public int charaColor = 1;

    SpriteRenderer[] _sprites;

    // Input
    Player _player;

    public bool Active {
        get { return gameObject.activeSelf; }
    }

    // these are only used by the first player to control the selections of ai players
    public List<CharacterSelector> aiList = new List<CharacterSelector>();
    CharacterSelector parentSelector;
    static int aiIndex = 0;
    bool frameskip = false;
    public CharacterSelector NextAI {
        get {
            if (aiList.Count > 0 && aiIndex < aiList.Count) {
                return aiList[aiIndex];
            } else {
                return null;
            }
        }
    }

    GameManager _gameManager;
    PlayerManager _playerManager;
    CharacterSelect _charaSelect;

    // Networking stuff
    PhotonView _photonView;
    public PhotonView PhotonView {
        get { return _photonView; }
    }
    public bool isLocal;
    public int ownerId;

    CharacterIcon[] _charaIcons;

    CharacterSelectResources _resources;

    private void Awake() {
        _photonView = GetComponent<PhotonView>();

        _charaSelect = FindObjectOfType<CharacterSelect>();
        _resources = FindObjectOfType<CharacterSelectResources>();

        _sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer sr in _sprites) {
            sr.enabled = false;
        }

        // If we have a playerNum already
        if (playerNum > -1) {
            CharacterWindow[] charaWindows = FindObjectsOfType<CharacterWindow>();
            foreach (CharacterWindow cw in charaWindows) {
                if (cw.num == playerNum) {
                    charaWindow = cw;
                    break;
                }
            }
        }

        _charaIcons = FindObjectsOfType<CharacterIcon>();
    }

    // Use this for initialization
    void Start () {
        _gameManager = FindObjectOfType<GameManager>();
        _playerManager = FindObjectOfType<PlayerManager>();

        aiIndex = 0;

        charaWindow.PlayerController.characterSelector = this;
    }

    // Currently only used when networking
    public void NetworkInitialize() {
        Debug.Log("Network Initialize");

        // Controller should just always be first player cuz it's online
        _player = ReInput.players.GetPlayer(0);
    }

    public void Activate(Player player) {
        isActive = true;
        isLocal = true;

        _player = player;
        charaWindow.PlayerController.SetInputPlayer(_player);

        foreach (SpriteRenderer sr in _sprites) {
            sr.enabled = true;
        }

        if (curCharacterIcon != null) {
            HighlightIcon(curCharacterIcon);
        }

        charaWindow.Activate(false, playerNum);
    }

    // For AI
    public void ActivateAsAI(CharacterSelector pSelector) {
        isActive = true;
        isLocal = true;

        isAI = true;

        parentSelector = pSelector;
        _player = pSelector._player;
        charaWindow.PlayerController.SetInputPlayer(pSelector._player);
        if (curCharacterIcon != null) {
            HighlightIcon(curCharacterIcon);
        }

        foreach (SpriteRenderer sr in _sprites) {
            sr.enabled = true;
        }

        charaWindow.Activate(true, playerNum);
    }

    // For Networking
    public void Activate(bool ai, bool local) {
        isActive = true;

        /*
        if (ai) {
            isAI = true;
            takeInput = false;

            _player = ReInput.players.GetPlayer(0);
            charaWindow.playerController.SetInputPlayer(0);

            _charaSelect.numAI++;
        } else {
            _player = ReInput.players.GetPlayer(playerNum);
        }
        */

        // Controller should always be 0 since we're online
        _player = ReInput.players.GetPlayer(0);
        charaWindow.PlayerController.SetInputPlayer(_player);

        foreach (SpriteRenderer sr in _sprites) {
            sr.enabled = true;
        }
        if (curCharacterIcon != null) {
            HighlightIcon(curCharacterIcon);
        }

        charaWindow.Activate(ai, playerNum);
        isLocal = local;

        _charaSelect.numPlayers++;

        // If we are the local player
        if(local) {
            takeInput = true;
            // Send an rpc to activate this selector for everyone else
            GetComponent<PhotonView>().RPC("NetworkActivate", PhotonTargets.OthersBuffered);
        }
    }

    [PunRPC]
    public void NetworkActivate() {
        Activate(false, false);
    }

    public void Deactivate() {
        isActive = false;

        foreach (SpriteRenderer sr in _sprites) {
            sr.enabled = false;
        }

        if (isAI) {
            _charaSelect.numAI--;
            // Give parent player input back
            parentSelector.charaWindow.PlayerController.underControl = true;
        } else {
            _charaSelect.numPlayers--;
            // Make this player available to be used again
            _charaSelect.RemovePlayer(_player);
        }

        curCharacterIcon.Unhighlight();

        _player = null;
        charaWindow.Deactivate();
    }
    
    // Update is called once per frame
    void Update () {
        // This is an attempt to prevent input from overflowing to ai selectors when activating them
        if (frameskip) {
            frameskip = false;
            return;
        }

        if (isActive) {
            // Make sure our icon stays highlighted
            if (!curCharacterIcon.isHighlighted) {
                curCharacterIcon.isHighlighted = true;
            }

            if (_player != null && takeInput && isLocal) {
                CheckInput();
            }
        }
    }

    public void CheckInput() {
        // Don't take any input if the player is being controlled
        if(charaWindow.PlayerController.underControl) {
            return;
        }

        if (!lockedIn) {
            // Right
            if (_player.GetButtonDown("Right")) {
                if (curCharacterIcon.adjOptions[0] != null && curCharacterIcon.adjOptions[0].IsReady) {
                    // move selector to adjOptions[0]
                    HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[0]);
                }
            }
            // Left
            if (_player.GetButtonDown("Left")) {
                if (curCharacterIcon.adjOptions[2] != null && curCharacterIcon.adjOptions[2].IsReady) {
                    // move selector to adjOptions[2]
                    HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[2]);
                }
            }
            // Up
            if (_player.GetButtonDown("Up")) {
                if (curCharacterIcon.adjOptions[3] != null && curCharacterIcon.adjOptions[3].IsReady) {
                    // move selector to adjOptions[3]
                    HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[3]);
                }
            }
            // Down
            if (_player.GetButtonDown("Down")) {
                if (curCharacterIcon.adjOptions[1] != null && curCharacterIcon.adjOptions[1].IsReady) {
                    // move selector to adjOptions[1]
                    HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[1]);
                }
            }
        } else if(lockedIn && !isReady) {
            if(_player.GetButtonDown("Right")) {
                // Change Color to the right
                ChangeColorRight();
            }
            if (_player.GetButtonDown("Left")) {
                // Change color to the left
                ChangeColorLeft();
            }
        }

        if ((_player.GetButtonDown("Submit") || _player.GetButtonDown("Shift")) && (!lockedIn || !isReady)) {
            if (!lockedIn) {
                // Lock in
                LockIn();
            } else {
                ShiftCSPlayer();
            }
        }
        if (_player.GetButtonDown("Cancel")) {
            if (lockedIn) {
                Unlock();
            } else {
                if(!_gameManager.isOnline) {
                    // Deactivate
                    Deactivate();
                } else {
                    // Ask disconnect message
                    _charaSelect.exitMenu.Activate();
                }
            }
        }
    }

    public void LockIn() {
        lockedIn = true;
        //curCharacterIcon.Lock();
        charaWindow.colorArrows.SetActive(true);

        // If the base color is taken, change to the right
        if(_resources.CharaAnimators[(int)curCharacterIcon.charaName][charaColor - 1].isTaken) {
            ChangeColorRight();
        }

        charaWindow.pullDownWindow.Show();

        // Play a sound
        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.MainMenuHighlight);
    }
    public void Unlock() {
        lockedIn = false;
        charaWindow.colorArrows.SetActive(false);
        // curCharacterIcon.Unlock();
        //_playerManager.RemovePlayerByNum(playerNum);

        charaWindow.pullDownWindow.Hide();

        // Reset color to default
        charaColor = 1;
    }
    public void ShiftCSPlayer() {
        // If the selected color isn't taken
        if (!_resources.CharaAnimators[(int)curCharacterIcon.charaName][charaColor - 1].isTaken) {
            // Take it
            _resources.CharaAnimators[(int)curCharacterIcon.charaName][charaColor - 1].isTaken = true;

            // Make sure player has correct controller
            charaWindow.PlayerController.SetInputPlayer(_player.id);

            // Shift this player into the play area
            charaWindow.PlayerController.ShiftIntoPlayArea();
            isReady = true;
        }
    }
    public void Unready() {
        isReady = false;
        takeInput = true;

        charaWindow.colorArrows.SetActive(true);

        // Free up that color
        _resources.CharaAnimators[(int)curCharacterIcon.charaName][charaColor - 1].isTaken = false;
    }

    void HighlightIcon(CharacterIcon charaIcon) {
        // Unhighlight current icon
        curCharacterIcon.Unhighlight();

        // Get new icon
        curCharacterIcon = charaIcon;

        // Move to that icon
        transform.position = new Vector3(charaIcon.transform.position.x, charaIcon.transform.position.y, charaIcon.transform.position.z - (2f + 0.1f * playerNum));

        // Change portrait to correct character
        charaWindow.charaPortrait.sprite = _resources.CharaPortraits[(int)charaIcon.charaName][0];
        // Change animator to correct character
        charaWindow.CharaAnimator.runtimeAnimatorController = _resources.CharaAnimators[(int)charaIcon.charaName][0].animator;
        // Change name to correct character
        charaWindow.charaName.text = _resources.CharaNames[(int)charaIcon.charaName];

        // Play idle animation
        charaWindow.CharaAnimator.SetInteger("PlayerState", 0);
        charaWindow.CharaAnimator.speed = 1;

        // Highlight the icon
        charaIcon.Highlight();

        // Play a sound
        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.SubMenuHighlight);
    }

    void ChangeColorRight() {
        int curColor = charaColor;

        // Iterate throught the colors until there is one that isn't already taken
        do {
            charaColor += 1;
            if (charaColor > _resources.CharaAnimators[(int)curCharacterIcon.charaName].Count) {
                charaColor = 1;
            }

            // If we've looped back to the same color
            if (charaColor == curColor) {
                // Break out of the loop
                break;
            }
        } while (_resources.CharaAnimators[(int)curCharacterIcon.charaName][charaColor - 1].isTaken);

        // Change portrait to correct character
        charaWindow.charaPortrait.sprite = _resources.CharaPortraits[(int)curCharacterIcon.charaName][charaColor - 1];
        // Change animator to correct character
        charaWindow.CharaAnimator.runtimeAnimatorController = _resources.CharaAnimators[(int)curCharacterIcon.charaName][charaColor - 1].animator;

        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.SubMenuHighlight);
    }
    void ChangeColorLeft() {
        int curColor = charaColor;

        // Iterate throught the colors until there is one that isn't already taken
        do {
            charaColor -= 1;
            if (charaColor < 1) {
                charaColor = _resources.CharaAnimators[(int)curCharacterIcon.charaName].Count;
            }

            // If we've looped back to the same color
            if (charaColor == curColor) {
                // Break out of the loop
                break;
            }
        } while (_resources.CharaAnimators[(int)curCharacterIcon.charaName][charaColor - 1].isTaken);

        // Change portrait to correct character
        charaWindow.charaPortrait.sprite = _resources.CharaPortraits[(int)curCharacterIcon.charaName][charaColor - 1];
        // Change animator to correct character
        charaWindow.CharaAnimator.runtimeAnimatorController = _resources.CharaAnimators[(int)curCharacterIcon.charaName][charaColor - 1].animator;

        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.SubMenuHighlight);
    }

    public void LoadCharacter() {
        CharaInfo tempInfo = new CharaInfo();
        tempInfo.name = curCharacterIcon.charaName;
        tempInfo.color = charaColor;
        tempInfo.team = charaWindow.PlayerController.team;
        // TODO: Make it so they can't get a team at all in the character select
        if(FindObjectOfType<GameManager>().gameMode == GAME_MODE.SURVIVAL) {
            tempInfo.team = -1;
        }

        // If we're online
        if (PhotonNetwork.connectedAndReady) {
            // We should store the owners id and add to the player manager
            _playerManager.AddPlayer(_player.id, tempInfo, _photonView.ownerId);
        } else {
            _playerManager.AddPlayer(_player.id, isAI, tempInfo);
        }
    }

    public void SetIcon(CharaInfo charaInfo) {
        if (_charaIcons != null && charaWindow != null) {
            foreach (CharacterIcon charaIcon in _charaIcons) {
                if (charaIcon.charaName == charaInfo.name) {
                    HighlightIcon(charaIcon);
                }
            }

            SetColor(charaInfo.color);
        }
    }

    public void SetColor(int color) {
        // Set to the new color
        charaColor = color;

        // Change portrait to correct character
        charaWindow.charaPortrait.sprite = _resources.CharaPortraits[(int)curCharacterIcon.charaName][charaColor - 1];
        // Change animator to correct character
        charaWindow.CharaAnimator.runtimeAnimatorController = _resources.CharaAnimators[(int)curCharacterIcon.charaName][charaColor - 1].animator;

        //_audioSource.Play();
    }
}
