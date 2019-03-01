using UnityEngine;
using Rewired;
using System.Collections.Generic;

[System.Serializable]
public class CharaInfo {
    public CHARACTERS name;
    public int color = 1;
}

public enum CHARACTERS { BOY = 0, GIRL, ROOSTER, NUM_CHARACTER };
//public enum CHARACTERNAMES { BOY1 = 0, BOY2, BOY3, BOY4, GIRL1, GIRL2, ROOSTER1, ROOSTER2, NUM_CHARACTERS};
public class Character : MonoBehaviour {
    public Team teamLeft;
    public Team teamRight;
    public SpriteRenderer readySprite;
    public bool lockedIn = false;
    public bool isAI;
    public bool takeInput;

    int _playerNum;
    //CHARACTERNAMES _characterName;
    CharaInfo _charaInfo = new CharaInfo();
    public int _team; // -1 = no team, 0 = left team, 1 = right team
    bool _active;

    Vector2 _initialPos;

    bool _justMoved;

    Player _player;

    // these are only used by the first player to control the selections of ai players
    public List<Character> aiList = new List<Character>();
    Character parentCharacter;
    static int aiIndex = 0;
    bool frameskip = false;

    Animator _animator;

    PlayerManager _playerManager;
    SpriteRenderer _spriteRenderer;
    AudioSource _audioSource;

    AudioClip _moveClip;

    TeamSelectArrow _leftArrow;
    TeamSelectArrow _rightArrow;

    // Networking stuff
    PhotonView _photonView;
    public PhotonView PhotonView {
        get { return _photonView; }
    }
    public bool isLocal;

    public int PlayerNum {
        get { return _playerNum; }
    }
    //public CHARACTERNAMES CharacterName {
    //    get { return _characterName; }
    //}
    public CHARACTERS CharacterName {
        get { return _charaInfo.name; }
    }
    public bool Active {
        get { return _active; }
    }
    public int Team {
        get { return _team; }
    }

    private void Awake() {
        _initialPos = transform.position;

        if (_playerManager == null) {
            _playerManager = FindObjectOfType<PlayerManager>();
        }
        if (_animator == null) {
            _animator = GetComponentInChildren<Animator>();
        }

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _audioSource = GetComponent<AudioSource>();
        _moveClip = Resources.Load<AudioClip>("Audio/SFX/Blip_Select");

        _photonView = GetComponent<PhotonView>();
        isLocal = true;

        GetArrows();
    }

    // Use this for initialization
    void Start() {
        _active = true;
        _justMoved = false;

        // If we're first player
        if (_playerNum == 0) {
            // Find the ai characters
            Character[] characters = FindObjectsOfType<Character>();
            foreach(Character chara in characters) {
                if(chara.isAI) {
                    aiList.Add(chara);
                }
            }
        }
    }

    public void Initialize(PlayerInfo pInfo) {
        _team = -1;
        _playerManager = FindObjectOfType<PlayerManager>();

        _playerNum = pInfo.playerNum;

        _animator = GetComponentInChildren<Animator>();
        SetCharacter(pInfo.charaInfo);

        if(pInfo.isAI) {
            isAI = true;
            takeInput = false;
            _player = ReInput.players.GetPlayer(0);
        } else {
            takeInput = true;
            _player = ReInput.players.GetPlayer(_playerNum);
        }
        /*
        if (pInfo.team == 0) {
            MoveLeft();
        } else {
            MoveRight();
        }
        */
    }

    void GetArrows() {
        // Get team select arrows
        TeamSelectArrow[] sideArrows = transform.GetComponentsInChildren<TeamSelectArrow>();
        if (sideArrows[0].side == 0) {
            _leftArrow = sideArrows[0];
            _rightArrow = sideArrows[1];
        } else {
            _rightArrow = sideArrows[0];
            _leftArrow = sideArrows[1];
        }

        //_leftArrow.transform.parent = null;
        //_rightArrow.transform.parent = null;
    }

    // Update is called once per frame
    void Update() {
        if (isLocal && _active && takeInput) {
            CheckInput();
            UpdateArrows();
        }
    }

    void CheckInput() {
        if (!_justMoved && !lockedIn) {
            // Changing teams
            if (_player.GetButtonDown("Left")) {
                MoveLeft();
                _justMoved = true;
            }
            if (_player.GetButtonDown("Right")) {
                MoveRight();
                _justMoved = true;
            }
        } else {
            if (!_player.GetButton("Left") && !_player.GetButton("Right")) {
                _justMoved = false;
            }
        }

        if (_player.GetButtonDown("Submit") && _team != -1 && !lockedIn) {
            // Lock in
            LockIn();

            // If first player or an ai
            if ((_playerNum == 0 || isAI) && aiList.Count > 0 && aiIndex < aiList.Count) {
                // Gain control of next AI player
                takeInput = false;
                aiList[aiIndex].takeInput = true;
                aiList[aiIndex].frameskip = true;
                aiList[aiIndex].aiList = aiList;
                aiList[aiIndex].parentCharacter = this;
                aiIndex++;
            }
        }
        if (_player.GetButtonDown("Cancel")) {
            if (lockedIn) {
                Unlock();
            } else if (isAI) {
                takeInput = false;
                parentCharacter.takeInput = true;
                parentCharacter.frameskip = true;
                parentCharacter.Unlock();
                aiIndex--;
            }
        }
    }

    public void LockIn() {
        lockedIn = true;
        readySprite.enabled = true;
    }
    public void Unlock() {
        lockedIn = false;
        readySprite.enabled = false;
    }

    void PlayMoveClip() {
        _audioSource.clip = _moveClip;
        _audioSource.Play();
    }

    void UpdateArrows() {
        if(_team == -1) {
            // Only activate arrows if that side has space
            if(teamLeft.HasSpace()) {
                _leftArrow.Activate();
            } else {
                _leftArrow.Deactivate();
            }
            if(teamRight.HasSpace()) {
                _rightArrow.Activate();
            } else {
                _rightArrow.Deactivate();
            }
        } else {
            _leftArrow.Deactivate();
            _rightArrow.Activate();
        }
    }

    public void MoveLeft() {
        // If not assigned to a team yet, 
        if (_team == -1) {
            // check if the left team has space
            if(teamLeft.HasSpace()) {
                // if it has space, move to that team
                // in the next open slot.
                teamLeft.TakeCharacter(this);
                _team = 0;
            }
        // If already assigned to right team,
        } else if (_team == 1) {
            // move to center, with no team.
            transform.position = _initialPos;
            teamRight.LoseCharacter(this);
            _team = -1;
        }
        // Set facing to normal
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        readySprite.transform.localScale = new Vector3(Mathf.Abs(readySprite.transform.localScale.x * -1f), readySprite.transform.localScale.y, readySprite.transform.localScale.z);

        _playerManager.SetTeam(_playerNum, _team);
        UpdateArrows();
        PlayMoveClip();
    }

    public void MoveRight() {
        // If not assigned to a team yet, 
        if (_team == -1) {
            // check if the right team has space
            if (teamRight.HasSpace()) {
                // if it has space, move to that team.
                // in the next open slot.
                teamRight.TakeCharacter(this);
                // Turn character around to face center
                transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
                readySprite.transform.localScale = new Vector3(readySprite.transform.localScale.x * -1f, readySprite.transform.localScale.y, readySprite.transform.localScale.z);
                _team = 1;
            }
            // If already assigned to left team,
        } else if (_team == 0) {
            // move to center, with no team.
            transform.position = _initialPos;
            teamLeft.LoseCharacter(this);
            _team = -1;
        }
        _playerManager.SetTeam(_playerNum, _team);
        UpdateArrows();
        PlayMoveClip();
    }

    public void SetCharacter(CharaInfo charaInfo) {
        //_characterName = charaName;
        _charaInfo = charaInfo;
        SetAnimator();
        
        // Update player info in player manager
        _playerManager.GetPlayerByNum(_playerNum).charaInfo = _charaInfo;
    }

    // Sets the sprite based on the current character
    void SetAnimator() {
        string path = "Art/Animations/Player/";
        switch (_charaInfo.name) {
            case CHARACTERS.BOY:
                path += "Boy/Animation Objects/Boy" + _charaInfo.color;
                break;
            case CHARACTERS.GIRL:
                path += "Girl/Animation Objects/Girl" + _charaInfo.color;
                break;
            case CHARACTERS.ROOSTER:
                path += "Rooster/Animation Objects/Rooster" + _charaInfo.color;
                break;
        }

        _animator.runtimeAnimatorController = Resources.Load(path) as RuntimeAnimatorController;
        /*
        switch (_characterName) {
            case CHARACTERNAMES.BOY1:
                _animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy1") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.BOY2:
                _animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy2") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.BOY3:
                _animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy3") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.BOY4:
                _animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy4") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.GIRL1:
                _animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Girl/Animation Objects/Girl1") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.GIRL2:
                _animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Girl/Animation Objects/Girl2") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.ROOSTER1:
                _animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Rooster/Animation Objects/Rooster1") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.ROOSTER2:
                _animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Rooster/Animation Objects/Rooster2") as RuntimeAnimatorController;
                break;
        }
        */

        // Play idle animation
        _animator.SetInteger("PlayerState", 0);
        _animator.speed = 1;
    }

    // Only for networking use
    public string GetNickname() {
        if(PhotonNetwork.connectedAndReady) {
            if(_team == 0) {
                return teamLeft.GetCharacterName(this);
            } else if(_team == 1) {
                return teamRight.GetCharacterName(this);
            }
        }

        return "";
    }

    public void Activate(int i) {

    }

    public void ActivateAI() {

    }

    public void Deactivate() {
        gameObject.SetActive(false);
    }
}
