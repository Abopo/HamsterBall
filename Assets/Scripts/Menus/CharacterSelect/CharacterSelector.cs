using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class CharacterSelector : MonoBehaviour {
    public Animator characterAnimator;
    public CharacterIcon curCharacterIcon;
    public GameObject readySprite;

    public int playerNum = -1;
    public bool lockedIn = false;
    public bool takeInput = true;
    public bool isAI = false;
    public int charaColor = 1;

    Player _player;

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
        if (playerNum == -1 || characterAnimator == null || readySprite == null) { // Should probably only happen when networking
            // Find correct stuff
            //Initialize();
        }

        if (!isAI) {
            _player = ReInput.players.GetPlayer(playerNum);
        } else {
            _player = ReInput.players.GetPlayer(0);
        }
    }

    public void Initialize() {
        // Get player number
        NewCharacterSelect characterSelect = FindObjectOfType<NewCharacterSelect>();
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
        readySprite = networkedCharaSelect.readySprites[playerNum];
        readySprite.SetActive(false);

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
    }

    public void Activate(bool ai, bool local) {
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
            CheckInput();
        }

        _moveTimer += Time.deltaTime;
    }

    public void CheckInput() {
        if (!lockedIn && CanMove()) {
            // Right
            if (_player.GetButtonDown("Right")) {
                if (curCharacterIcon.adjOptions[0] != null && curCharacterIcon.adjOptions[0].isReady) {
                    // move selector to adjOptions[0]
                    HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[0]);
                    _moveTimer = 0f;
                }
            }
            // Left
            if (_player.GetButtonDown("Left")) {
                if (curCharacterIcon.adjOptions[2] != null && curCharacterIcon.adjOptions[2].isReady) {
                    // move selector to adjOptions[2]
                    HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[2]);
                    _moveTimer = 0f;
                }
            }
            // Up
            if (_player.GetButtonDown("Up")) {
                if (curCharacterIcon.adjOptions[3] != null && curCharacterIcon.adjOptions[3].isReady) {
                    // move selector to adjOptions[3]
                    HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[3]);
                    _moveTimer = 0f;
                }
            }
            // Down
            if (_player.GetButtonDown("Down")) {
                if (curCharacterIcon.adjOptions[1] != null && curCharacterIcon.adjOptions[1].isReady) {
                    // move selector to adjOptions[1]
                    HighlightIcon((CharacterIcon)curCharacterIcon.adjOptions[1]);
                    _moveTimer = 0f;
                }
            }
        } else if(lockedIn && CanMove()) {
            if(_player.GetButtonDown("Right")) {
                // Change Color to the right
                charaColor += 1;
                if(charaColor > _resources.CharaAnimators[(int)curCharacterIcon.charaName].Count) {
                    charaColor = 1;
                }

                // Change animator to correct character
                characterAnimator.runtimeAnimatorController = _resources.CharaAnimators[(int)curCharacterIcon.charaName][charaColor - 1];

                _audioSource.Play();
            }
            if (_player.GetButtonDown("Left")) {
                // Change color to the left
                charaColor -= 1;
                if(charaColor < 1) {
                    charaColor = _resources.CharaAnimators[(int)curCharacterIcon.charaName].Count;
                }

                // Change animator to correct character
                characterAnimator.runtimeAnimatorController = _resources.CharaAnimators[(int)curCharacterIcon.charaName][charaColor - 1];

                _audioSource.Play();
            }
        }

        if (!_player.GetButton("Right") && !_player.GetButton("Left") && !_player.GetButton("Up") && !_player.GetButton("Down")) {
            _moveTimer = _moveTime + 1f;
        }

        if (_player.GetButtonDown("Submit") && !lockedIn && !curCharacterIcon.isLocked) {
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
        if (_player.GetButtonDown("Cancel")) {
            if (lockedIn) {
                Unlock();
            } else if (isAI) {
                takeInput = false;
                parentSelector.takeInput = true;
                parentSelector.frameskip = true;
                parentSelector.Unlock();
                aiIndex--;
            } else {
                // Back out to local play menu
                FindObjectOfType<GameManager>().LocalPlayButton();
            }
        }
    }

    public void LockIn() {
        lockedIn = true;
        readySprite.SetActive(true);
        curCharacterIcon.Lock();

        // Play a sound
        _audioSource.Play();
    }
    public void Unlock() {
        lockedIn = false;
        readySprite.SetActive(false);
        curCharacterIcon.Unlock();
        //_playerManager.RemovePlayerByNum(playerNum);

        // Reset color to default
        charaColor = 1;
    }

    void HighlightIcon(CharacterIcon charaIcon) {
        // Get new icon
        curCharacterIcon = charaIcon;

        // Move to that icon
        transform.position = new Vector3(charaIcon.transform.position.x, charaIcon.transform.position.y, charaIcon.transform.position.z - (2f + 0.1f * playerNum));

        // Change animator to correct character
        characterAnimator.runtimeAnimatorController = _resources.CharaAnimators[(int)charaIcon.charaName][charaColor-1];

        // Play idle animation
        characterAnimator.SetInteger("PlayerState", 0);
        characterAnimator.speed = 1;

        // Highlight the icon
        charaIcon.Highlight();

        // Play a sound
        _audioSource.Play();
    }

    bool CanMove() {
        if(_moveTimer >= _moveTime) {
            return true;
        }

        return false;
    }

    public void LoadCharacter() {
        CharaInfo tempInfo = new CharaInfo();
        tempInfo.name = curCharacterIcon.charaName;
        tempInfo.color = charaColor;
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

}
