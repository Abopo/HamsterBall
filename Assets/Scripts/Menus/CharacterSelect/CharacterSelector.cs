using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class CharacterSelector : MonoBehaviour {
    public CSPlayerController playerController;
    public Animator characterAnimator;
    public SpriteRenderer characterPortrait;
    public SpriteRenderer characterName;
    public CharacterIcon curCharacterIcon;
    public PullDownWindow pullDownWindow;
    public GameObject colorArrows;
    public GameObject comText;

    public int playerNum = -1;
    public bool lockedIn = false;
    public bool isReady = false;
    public bool takeInput = true;
    public bool isAI = false;
    public int charaColor = 1;

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

    PlayerManager _playerManager;
    AudioSource _audioSource;

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
    }

    // Use this for initialization
    void Start () {
        _playerManager = FindObjectOfType<PlayerManager>();
        _audioSource = GetComponent<AudioSource>();
        _resources = FindObjectOfType<CharacterSelectResources>();

        aiIndex = 0;
        HighlightIcon(curCharacterIcon);

        // If we haven't been set up properly
        if (playerNum == -1 || characterAnimator == null) { // Should probably only happen when networking
            // Find correct stuff
            //Initialize();
        }

        if (!isAI) {
            _player = ReInput.players.GetPlayer(playerNum);
        } else {
            _player = ReInput.players.GetPlayer(0);
            playerController.SetInputPlayer(0);
        }

        playerController.characterSelector = this;
    }

    public void Initialize() {
        // Get player number
        CharacterSelect characterSelect = FindObjectOfType<CharacterSelect>();
        playerNum = characterSelect.NumPlayers;
        _player = ReInput.players.GetPlayer(playerNum);

        // With player number, get correct border sprite, character animator, and ready sprite
        Sprite[] selectorSprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/CharacterSelectors");
        GetComponent<SpriteRenderer>().sprite = selectorSprites[playerNum];
        Transform child = transform.GetChild(0);
        child.GetComponent<SpriteRenderer>().sprite = selectorSprites[playerNum + 4];
        child.transform.Translate(0.66f*playerNum, 0f, 0f);

        NetworkedCharacterSelect networkedCharaSelect = FindObjectOfType<NetworkedCharacterSelect>();
        characterAnimator = networkedCharaSelect.charaAnimators[playerNum];
        characterAnimator.gameObject.SetActive(true);

        _charaIcons = FindObjectsOfType<CharacterIcon>();
        HighlightIcon(_charaIcons[playerNum]);
    }

    public void Activate(bool ai) {
        if(ai) {
            isAI = true;
            takeInput = false;
        }
        gameObject.SetActive(true);
        characterAnimator.gameObject.SetActive(true);
        characterPortrait.enabled = true;
    }

    public void Activate(bool ai, bool local) {
        if (ai) {
            isAI = true;
            takeInput = false;
        }
        gameObject.SetActive(true);
        characterAnimator.gameObject.SetActive(true);
        characterPortrait.enabled = true;
        isLocal = local;
    }

    public void Deactivate() {
        gameObject.SetActive(false);
        characterAnimator.gameObject.SetActive(false);
        characterPortrait.enabled = false;
    }

    // Update is called once per frame
    void Update () {
        // This is an attempt to prevent input from overflowing to ai selectors when activating them
        if (frameskip) {
            frameskip = false;
            return;
        }

        if (isLocal && takeInput) {
            CheckInput();
        }
    }

    public void CheckInput() {
        // Don't take any input if the player is being controlled
        if(playerController.underControl) {
            return;
        }

        if (!lockedIn) {
            // Right
            if (_player.GetButtonDown("Right")) {
                if (curCharacterIcon.adjOptions[0] != null && curCharacterIcon.adjOptions[0].isReady) {
                    // move selector to adjOptions[0]
                    HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[0]);
                }
            }
            // Left
            if (_player.GetButtonDown("Left")) {
                if (curCharacterIcon.adjOptions[2] != null && curCharacterIcon.adjOptions[2].isReady) {
                    // move selector to adjOptions[2]
                    HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[2]);
                }
            }
            // Up
            if (_player.GetButtonDown("Up")) {
                if (curCharacterIcon.adjOptions[3] != null && curCharacterIcon.adjOptions[3].isReady) {
                    // move selector to adjOptions[3]
                    HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[3]);
                }
            }
            // Down
            if (_player.GetButtonDown("Down")) {
                if (curCharacterIcon.adjOptions[1] != null && curCharacterIcon.adjOptions[1].isReady) {
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
                // If the selected color isn't taken
                if(!_resources.CharaAnimators[(int)curCharacterIcon.charaName][charaColor - 1].isTaken) {
                    // Take it
                    _resources.CharaAnimators[(int)curCharacterIcon.charaName][charaColor - 1].isTaken = true;

                    // Shift this player into the play area
                    playerController.ShiftIntoPlayArea();
                    isReady = true;
                }
            }
        }
        if (_player.GetButtonDown("Cancel")) {
            if (lockedIn) {
                Unlock();
            } else if (isAI) {
                // Revert input to parent
                takeInput = false;
                parentSelector.takeInput = true;
                parentSelector.playerController.RegainControl();
                aiIndex--;
            } else {
                // Back out to local play menu
                FindObjectOfType<GameManager>().LocalPlayButton();
            }
        }
    }

    public void LockIn() {
        lockedIn = true;
        //curCharacterIcon.Lock();
        colorArrows.SetActive(true);

        pullDownWindow.Show();

        // Play a sound
        _audioSource.Play();
    }
    public void Unlock() {
        lockedIn = false;
        colorArrows.SetActive(false);
        // curCharacterIcon.Unlock();
        //_playerManager.RemovePlayerByNum(playerNum);

        pullDownWindow.Hide();

        // Reset color to default
        charaColor = 1;
    }
    public void Unready() {
        isReady = false;
        colorArrows.SetActive(true);
        
        // Free up that color
        _resources.CharaAnimators[(int)curCharacterIcon.charaName][charaColor - 1].isTaken = false;
    }

    void HighlightIcon(CharacterIcon charaIcon) {
        // Get new icon
        curCharacterIcon = charaIcon;

        // Move to that icon
        transform.position = new Vector3(charaIcon.transform.position.x, charaIcon.transform.position.y, charaIcon.transform.position.z - (2f + 0.1f * playerNum));

        // Change portrait to correct character
        characterPortrait.sprite = _resources.CharaPortraits[(int)charaIcon.charaName][0];
        // Change animator to correct character
        characterAnimator.runtimeAnimatorController = _resources.CharaAnimators[(int)charaIcon.charaName][0].animator;
        // Change name to correct character
        characterName.sprite = _resources.CharaNames[(int)charaIcon.charaName];

        // Play idle animation
        characterAnimator.SetInteger("PlayerState", 0);
        characterAnimator.speed = 1;

        // Highlight the icon
        charaIcon.Highlight();

        // Play a sound
        _audioSource.Play();
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
        characterPortrait.sprite = _resources.CharaPortraits[(int)curCharacterIcon.charaName][charaColor - 1];
        // Change animator to correct character
        characterAnimator.runtimeAnimatorController = _resources.CharaAnimators[(int)curCharacterIcon.charaName][charaColor - 1].animator;

        _audioSource.Play();
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
        characterPortrait.sprite = _resources.CharaPortraits[(int)curCharacterIcon.charaName][charaColor - 1];
        // Change animator to correct character
        characterAnimator.runtimeAnimatorController = _resources.CharaAnimators[(int)curCharacterIcon.charaName][charaColor - 1].animator;

        _audioSource.Play();
    }

    public void ControlNextAI() {
        // If first player or an ai
        if ((playerNum == 0 || isAI) && aiList.Count > 0 && aiIndex < aiList.Count) {
            // Gain control of next AI player
            takeInput = false;
            aiList[aiIndex].takeInput = true;
            aiList[aiIndex].frameskip = true;
            aiList[aiIndex].aiList = aiList;
            aiList[aiIndex].parentSelector = this;
            aiList[aiIndex].HideCOMText();
            aiIndex++;
        }
    }

    public void LoadCharacter() {
        CharaInfo tempInfo = new CharaInfo();
        tempInfo.name = curCharacterIcon.charaName;
        tempInfo.color = charaColor;
        tempInfo.team = playerController.team;
        // TODO: Make it so they can't get a team at all in the character select
        if(FindObjectOfType<GameManager>().gameMode == GAME_MODE.SURVIVAL) {
            tempInfo.team = -1;
        }
        _playerManager.AddPlayer(playerNum, isAI, tempInfo);
    }

    public void SetIcon(CharaInfo charaInfo) {
        if (_charaIcons != null) {
            foreach (CharacterIcon charaIcon in _charaIcons) {
                if (charaIcon.charaName == charaInfo.name) {
                    HighlightIcon(charaIcon);
                }
            }
        }

        SetColor(charaInfo.color);
    }

    public void SetColor(int color) {

    }

    public void ShowCOMText() {
        comText.SetActive(true);
    }
    public void HideCOMText() {
        comText.SetActive(false);
    }
}
