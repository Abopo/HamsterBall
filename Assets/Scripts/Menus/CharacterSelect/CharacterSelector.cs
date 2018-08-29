using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour {

    public Animator characterAnimator;
    public CharacterIcon curCharacterIcon;
    public GameObject readySprite;

    public int playerNum;
    public bool lockedIn = false;
    public bool takeInput = true;
    public bool isAI = false;

    InputState _inputState = new InputState();
    public int ControllerNum {
        get {
            return _inputState.controllerNum;
        }
        set {
            _inputState.controllerNum = value;
        }
    }

    // these are only used by the first player to control the selections of ai players
    public List<CharacterSelector> aiList = new List<CharacterSelector>();
    CharacterSelector parentSelector;
    static int aiIndex = 0;
    bool frameskip = false;

    PlayerManager _playerManager;

    // Use this for initialization
    void Start () {
        _playerManager = FindObjectOfType<PlayerManager>();
        readySprite.SetActive(false);
    }

    public void Activate(int conNum, bool ai) {
        ControllerNum = conNum;
        if(ai) {
            isAI = true;
            takeInput = false;
        }
        gameObject.SetActive(true);
        characterAnimator.gameObject.SetActive(true);
    }

    public void Deactivate() {
        gameObject.SetActive(false);
        characterAnimator.gameObject.SetActive(false);
        readySprite.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        // This is an attempt to prevent input from overflowing to ai selectors when activating them
        if (frameskip) {
            frameskip = false;
            return;
        }

        _inputState = InputState.GetInput(_inputState);

        if (takeInput) {
            if (!lockedIn) {
                // Right
                if (_inputState.right.isJustPressed) {
                    if (curCharacterIcon.adjOptions[0] != null && curCharacterIcon.adjOptions[0].isReady) {
                        // move selector to adjOptions[0]
                        HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[0]);
                    }
                }
                // Left
                if (_inputState.left.isJustPressed) {
                    if (curCharacterIcon.adjOptions[2] != null && curCharacterIcon.adjOptions[2].isReady) {
                        // move selector to adjOptions[2]
                        HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[2]);
                    }
                }
                // Up
                if (_inputState.up.isJustPressed) {
                    if (curCharacterIcon.adjOptions[3] != null && curCharacterIcon.adjOptions[3].isReady) {
                        // move selector to adjOptions[3]
                        HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[3]);
                    }
                }
                // Down
                if (_inputState.down.isJustPressed) {
                    if (curCharacterIcon.adjOptions[1] != null && curCharacterIcon.adjOptions[1].isReady) {
                        // move selector to adjOptions[1]
                        HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[1]);
                    }
                }
            }
            if (_inputState.swing.isJustPressed && !lockedIn && !curCharacterIcon.isLocked) {
                // Lock in
                LockIn();

                // If first player or an ai
                if ((playerNum == 0 || isAI) && aiList.Count > 0 && aiIndex < aiList.Count) {
                    // Gain control of next AI player
                    takeInput = false;
                    aiList[aiIndex].takeInput = true;
                    aiList[aiIndex].frameskip = true;
                    aiList[aiIndex].aiList = aiList;
                    aiList[aiIndex].parentSelector = this;
                    aiIndex++;
                }
            }
            if (_inputState.attack.isJustPressed) {
                if (lockedIn) {
                    Unlock();
                } else if (isAI) {
                    takeInput = false;
                    parentSelector.takeInput = true;
                    parentSelector.frameskip = true;
                    parentSelector.Unlock();
                    aiIndex--;
                }
            }
        }
    }

    void LockIn() {
        lockedIn = true;
        readySprite.SetActive(true);
        curCharacterIcon.Lock();
        _playerManager.AddPlayer(playerNum, ControllerNum, curCharacterIcon.characterName);
    }
    void Unlock() {
        lockedIn = false;
        readySprite.SetActive(false);
        curCharacterIcon.Unlock();
        _playerManager.RemovePlayerByNum(playerNum);
    }

    void HighlightIcon(CharacterIcon charaIcon) {
        // Get new icon
        curCharacterIcon = charaIcon;

        // Move to that icon
        transform.position = new Vector3(charaIcon.transform.position.x, charaIcon.transform.position.y, transform.position.z);

        // Change animator to correct character
        switch(charaIcon.characterName) {
            case CHARACTERNAMES.BOY1:
                characterAnimator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy1") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.BOY2:
                characterAnimator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy2") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.BOY3:
                characterAnimator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy3") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.BOY4:
                characterAnimator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy4") as RuntimeAnimatorController;
                break;
        }

        // Play idle animation
        characterAnimator.SetInteger("PlayerState", 0);
        characterAnimator.speed = 1;

        // Highlight the icon
        charaIcon.Highlight();
    }
}
