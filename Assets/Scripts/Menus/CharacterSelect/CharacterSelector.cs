using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour {
    public Animator characterAnimator;
    public CharacterIcon curCharacterIcon;
    public GameObject readySprite;

    public int playerNum = -1;
    public bool lockedIn = false;
    public bool takeInput = true;
    public bool isAI = false;

    InputState _inputState = new InputState();
    public InputState InputState {
        get { return _inputState; }
    }
    public int ControllerNum {
        get {
            return _inputState.controllerNum;
        }
        set {
            _inputState.controllerNum = value;
        }
    }

    public bool Active {
        get { return gameObject.activeSelf; }
    }

    float _moveTime = 0.3f;
    float _moveTimer = 0f;

    // these are only used by the first player to control the selections of ai players
    public List<CharacterSelector> aiList = new List<CharacterSelector>();
    CharacterSelector parentSelector;
    static int aiIndex = 0;
    bool frameskip = false;

    PlayerManager _playerManager;

    // Networking stuff
    PhotonView _photonView;
    public PhotonView PhotonView {
        get { return _photonView; }
    }
    public bool isLocal;
    public int ownerId;

    CharacterIcon[] _charaIcons;

    private void Awake() {
        _photonView = GetComponent<PhotonView>();
    }

    // Use this for initialization
    void Start () {
        _playerManager = FindObjectOfType<PlayerManager>();

        // If we haven't been set up properly
        if (playerNum == -1 || characterAnimator == null || readySprite == null) { // Should probably only happen when networking
            // Find correct stuff
            //Initialize();
        }
    }

    public void Initialize() {
        // Get player number
        NewCharacterSelect characterSelect = FindObjectOfType<NewCharacterSelect>();
        playerNum = characterSelect.NumPlayers;

        // With player number, get correct border sprite, character animator, and ready sprite
        Sprite[] selectorSprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/CharacterSelectors");
        GetComponent<SpriteRenderer>().sprite = selectorSprites[playerNum];
        Transform child = transform.GetChild(0);
        child.GetComponent<SpriteRenderer>().sprite = selectorSprites[playerNum + 4];
        child.transform.Translate(0.66f*playerNum, 0f, 0f);

        NetworkedCharacterSelect networkedCharaSelect = FindObjectOfType<NetworkedCharacterSelect>();
        characterAnimator = networkedCharaSelect.charaAnimators[playerNum];
        characterAnimator.gameObject.SetActive(true);
        readySprite = networkedCharaSelect.readySprites[playerNum];
        readySprite.SetActive(false);

        _charaIcons = FindObjectsOfType<CharacterIcon>();
        HighlightIcon(_charaIcons[playerNum]);
        //curCharacterIcon = _charaIcons[playerNum];
        //transform.position = new Vector3(_charaIcons[playerNum].transform.position.x,
        //                                            _charaIcons[playerNum].transform.position.y,
        //                                            _charaIcons[playerNum].transform.position.z - (2f+0.1f*playerNum));

        // Add ourself to the character select in case we were missed
        //FindObjectOfType<NewCharacterSelect>().AddSelector(this);
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

    public void Activate(int conNum, bool ai, bool local) {
        ControllerNum = conNum;
        if (ai) {
            isAI = true;
            takeInput = false;
        }
        gameObject.SetActive(true);
        characterAnimator.gameObject.SetActive(true);
        isLocal = local;
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

        if (isLocal && takeInput) {
            GetInput();
            CheckInput();
        }

        _moveTimer += Time.deltaTime;
    }

    public void CheckInput() {
        if (!lockedIn && CanMove()) {
            // Right
            if (_inputState.right.isDown) {
                if (curCharacterIcon.adjOptions[0] != null && curCharacterIcon.adjOptions[0].isReady) {
                    // move selector to adjOptions[0]
                    HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[0]);
                    _moveTimer = 0f;
                }
            }
            // Left
            if (_inputState.left.isDown) {
                if (curCharacterIcon.adjOptions[2] != null && curCharacterIcon.adjOptions[2].isReady) {
                    // move selector to adjOptions[2]
                    HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[2]);
                    _moveTimer = 0f;
                }
            }
            // Up
            if (_inputState.up.isDown) {
                if (curCharacterIcon.adjOptions[3] != null && curCharacterIcon.adjOptions[3].isReady) {
                    // move selector to adjOptions[3]
                    HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[3]);
                    _moveTimer = 0f;
                }
            }
            // Down
            if (_inputState.down.isDown) {
                if (curCharacterIcon.adjOptions[1] != null && curCharacterIcon.adjOptions[1].isReady) {
                    // move selector to adjOptions[1]
                    HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[1]);
                    _moveTimer = 0f;
                }
            }
        }
        if (!_inputState.right.isDown && !_inputState.left.isDown && !_inputState.up.isDown && !_inputState.down.isDown) {
            _moveTimer = _moveTime + 1f;
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

    public void LockIn() {
        lockedIn = true;
        readySprite.SetActive(true);
        curCharacterIcon.Lock();
        if (!isAI) {
            _playerManager.AddPlayer(playerNum, ControllerNum, curCharacterIcon.characterName);
        } else {
            _playerManager.AddPlayer(playerNum, -1, curCharacterIcon.characterName);
        }
    }
    public void Unlock() {
        lockedIn = false;
        readySprite.SetActive(false);
        curCharacterIcon.Unlock();
        _playerManager.RemovePlayerByNum(playerNum);
    }

    void HighlightIcon(CharacterIcon charaIcon) {
        // Get new icon
        curCharacterIcon = charaIcon;

        // Move to that icon
        transform.position = new Vector3(charaIcon.transform.position.x, charaIcon.transform.position.y, charaIcon.transform.position.z - (2f + 0.1f * playerNum));

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

    bool CanMove() {
        if(_moveTimer >= _moveTime) {
            return true;
        }

        return false;
    }

    public void GetInput() {
        _inputState = InputState.GetInput(_inputState);
    }
    public void TakeInput(InputState input) {
        int conNum = _inputState.controllerNum;
        _inputState = input;
        _inputState.controllerNum = conNum;
    }

    public void SetIcon(CHARACTERNAMES characterName) {
        if (_charaIcons != null) {
            foreach (CharacterIcon charaIcon in _charaIcons) {
                if (charaIcon.characterName == characterName) {
                    HighlightIcon(charaIcon);
                }
            }
        }
    }
}
