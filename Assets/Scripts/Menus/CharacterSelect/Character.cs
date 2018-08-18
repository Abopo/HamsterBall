using UnityEngine;
using System.Collections.Generic;

public enum CHARACTERNAMES { BOY1 = 0, BOY2, BOY3, BOY4, NUM_CHARACTERS};
public class Character : MonoBehaviour {
    public Team teamLeft;
    public Team teamRight;
    public bool isAI;
    public bool takeInput;
    //public Character aiChild;
    public Stack<Character> aiChildren = new Stack<Character>();
    Character humanParent; // the player controlling this ai character

    int _playerNum;
    CHARACTERNAMES _characterName;
    int _team; // -1 = no team, 0 = left team, 1 = right team
    bool _active;

    Vector2 _initialPos;

    InputState _inputState;
    public int ControllerNum {
        get {
            return _inputState.controllerNum;
        }
    }

    Animator _animator;

    PlayerManager _playerManager;
    SpriteRenderer _spriteRenderer;
    AudioSource _audioSource;

    AudioClip _activateClip;
    AudioClip _deactivateClip;
    AudioClip _moveClip;

    TeamSelectArrow _leftArrow;
    TeamSelectArrow _rightArrow;
    CharacterChangeArrow _upArrow;
    CharacterChangeArrow _downArrow;

    // Networking stuff
    PhotonView _photonView;
    public PhotonView PhotonView {
        get { return _photonView; }
    }
    public bool isLocal;

    public int PlayerNum {
        get { return _playerNum; }
    }
    public int JoystickNum {
        get { return _inputState.controllerNum; }
    }
    public CHARACTERNAMES CharacterName {
        get { return _characterName; }
    }
    public bool Active {
        get { return _active; }
    }
    public int Team {
        get { return _team; }
    }
    public InputState InputState {
        get { return _inputState; }
    }

    public Character TopAIChild {
        get {
            if (aiChildren.Count > 0) {
                return aiChildren.Peek();
            } else {
                return null;
            }
        }    
    }

    // Use this for initialization
    void Start () {
        isAI = false;
        takeInput = true;

        _team = -1;
        _active = false;

        _initialPos = transform.position;

        _playerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();
        _inputState = new InputState();

        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _audioSource = GetComponent<AudioSource>();
        _activateClip = Resources.Load<AudioClip>("Audio/SFX/Activate_Character");
        _deactivateClip = Resources.Load<AudioClip>("Audio/SFX/Deactivate_Character");
        _moveClip = Resources.Load<AudioClip>("Audio/SFX/Blip_Select");

        _photonView = GetComponent<PhotonView>();
        isLocal = true;

        GetArrows();
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

        _leftArrow.transform.parent = null;
        _rightArrow.transform.parent = null;

        // Get character change arrows
        CharacterChangeArrow[] charaArrows = transform.GetComponentsInChildren<CharacterChangeArrow>();
        if (charaArrows[0].side == 0) {
            _upArrow = charaArrows[0];
            _downArrow = charaArrows[1];
        } else {
            _downArrow = charaArrows[0];
            _upArrow = charaArrows[1];
        }

        _upArrow.transform.parent = null;
        _downArrow.transform.parent = null;
    }

    // Update is called once per frame
    void Update () {
        if (_active && takeInput) {
            CheckInput();
        }
	}

    void CheckInput() {
        if (isLocal) {
            _inputState = InputState.GetInput(_inputState);
        }
        // Changing teams
        if (_inputState.left.isJustPressed && !isAI) {
            if (TopAIChild != null) {
                TopAIChild.MoveLeft();
            } else {
                MoveLeft();
            }
        }
        if(_inputState.right.isJustPressed && !isAI) {
            if (TopAIChild != null) {
                TopAIChild.MoveRight();
            } else {
                MoveRight();
            }
        }

        // Changing Characters
        if (_inputState.up.isJustPressed && !isAI) {
            if (TopAIChild != null) {
                TopAIChild.ChangeCharacterUp();
            } else {
                ChangeCharacterUp();
            }
        } else if (_inputState.down.isJustPressed && !isAI) {
            if (TopAIChild != null) {
                TopAIChild.ChangeCharacterDown();
            } else {
                ChangeCharacterDown();
            }
        }

        // Reset the input for networked instances
        if (_photonView != null && _photonView.owner != PhotonNetwork.player) {
            _inputState = InputState.ResetInput(_inputState);
        }
    }

    void PlayActivateClip() {
        _audioSource.clip = _activateClip;
        _audioSource.Play();
    }

    void PlayDeactivateClip() {
        _audioSource.clip = _deactivateClip;
        _audioSource.Play();
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
        } else if(_team == 0) {
            _leftArrow.Deactivate();
            _rightArrow.Activate();
        } else if(_team == 1) {
            _leftArrow.Activate();
            _rightArrow.Deactivate();
        }

        _leftArrow.transform.position = new Vector3(transform.position.x - 1f, transform.position.y, 0);
        _rightArrow.transform.position = new Vector3(transform.position.x + 1f, transform.position.y, 0);

        _upArrow.Activate();
        _downArrow.Activate();
        _upArrow.transform.position = new Vector3(transform.position.x, transform.position.y+ 1.14f, 0);
        _downArrow.transform.position = new Vector3(transform.position.x, transform.position.y-1.3f, 0);
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
                transform.localScale = new Vector3(-1f, 1f, 1f);
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

    public void Activate(int playerNum) {
        _active = true;
        _playerNum = playerNum;
        _inputState.controllerNum = _playerManager.GetControllerNum(playerNum);
        _spriteRenderer.enabled = true;
        UpdateArrows();
        isAI = false;

        // Get character from player manager
        _characterName = _playerManager.GetPlayerByNum(_playerNum).characterName;
        SetAnimator();

        // If we are networked,
        // Send event that we've been activated
        if (_photonView != null) {
            if (isLocal) {
                // Take ownership of this character
                _photonView.TransferOwnership(PhotonNetwork.player);
                 
                // Add the player id to controllerNum in order to differentiate a networked character
                _photonView.RPC("ActivateCharacter", PhotonTargets.Others, _inputState.controllerNum + 4 + _photonView.ownerId, _photonView.ownerId);
            } 
            if (PhotonNetwork.isMasterClient) {
                // Set owner id of playerinfo
                _playerManager.GetPlayerByNum(playerNum).ownerID = _photonView.ownerId;
            }
        }

        PlayActivateClip();
    }

    public void ActivateAI(int playerNum, Character parent) {
        isAI = true;
        _active = true;
        _playerNum = playerNum;
        _inputState.controllerNum = -parent.ControllerNum;
        _spriteRenderer.enabled = true;
        UpdateArrows();
        humanParent = parent;
        parent.aiChildren.Push(this);

        // Get character from player manager
        _characterName = _playerManager.GetPlayerByNum(_playerNum).characterName;
        SetAnimator();

        PlayActivateClip();
    }

    public void Deactivate() {
        if(aiChildren.Count > 0) {
            aiChildren.Pop().Deactivate();
            return;
        }

        // If we are networked,
        // Send event that we've been deactivated
        if (_photonView != null && _photonView.owner == PhotonNetwork.player) {
            _photonView.RPC("DeactivateCharacter", PhotonTargets.Others, _inputState.controllerNum, _photonView.ownerId);
        }

        _active = false;
        _inputState.controllerNum = -1;
        _spriteRenderer.enabled = false;
        if (_team == 0) {
            MoveRight();
        } else if(_team == 1) {
            MoveLeft();
        }

        //_leftArrow.GetComponent<SpriteRenderer>().enabled = false;
        //_rightArrow.GetComponent<SpriteRenderer>().enabled = false;
        _leftArrow.Deactivate();
        _rightArrow.Deactivate();
        _upArrow.Deactivate();
        _downArrow.Deactivate();

        if (isAI && humanParent != null) {
            humanParent = null;
        }


        PlayDeactivateClip();
    }

    public void SetCharacter(CHARACTERNAMES charaName) {
        _characterName = charaName;
        SetAnimator();
        
        // Update player info in player manager
        _playerManager.GetPlayerByNum(_playerNum).characterName = _characterName;
    }

    // Sets the sprite based on the current character
    void SetAnimator() {
        switch(_characterName) {
            /*
            case CHARACTERNAMES.BUB:
                _animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Bub") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.NEGABUB:
                _animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Bub2") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.BOB:
                _animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Bub3") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.NEGABOB:
                _animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Bub4") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.PEPSIMAN:
                _animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/PepsiMan/PepsiMan") as RuntimeAnimatorController;
                break;
            */
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
        }

        // Play idle animation
        _animator.SetInteger("PlayerState", 0);
        _animator.speed = 1;
    }

    public bool AllAIAssignedToTeam() {
        for(int i = 0; i < aiChildren.Count; ++i) {
            if (aiChildren.ToArray()[i]._team == -1) {
                return false;
            }
        }

        return true;
    }

    public void TakeInput(InputState input) {
        int conNum = _inputState.controllerNum;
        _inputState = input;
        _inputState.controllerNum = conNum;
    }

    public void ChangeCharacterUp() {
        // Get the next available character
        _characterName = _playerManager.GetNextAvailableCharacterUp(_characterName);
        // Update player info in player manager
        _playerManager.GetPlayerByNum(_playerNum).characterName = _characterName;

        // Change sprite new character
        SetAnimator();
    }
    public void ChangeCharacterDown() {
        // Get the next available character
        _characterName = _playerManager.GetNextAvailableCharacterDown(_characterName);
        // Update player info in player manager
        _playerManager.GetPlayerByNum(_playerNum).characterName = _characterName;

        // Change sprite new character
        SetAnimator();
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
}
