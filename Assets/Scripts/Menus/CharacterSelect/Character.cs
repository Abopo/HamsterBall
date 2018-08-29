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

    private void Awake() {
        _initialPos = transform.position;

        _playerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();
        _inputState = new InputState();

        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _audioSource = GetComponent<AudioSource>();
        _moveClip = Resources.Load<AudioClip>("Audio/SFX/Blip_Select");

        GetArrows();
    }

    // Use this for initialization
    void Start () {
        isAI = false;
        takeInput = true;

        _active = true;

        _photonView = GetComponent<PhotonView>();
        isLocal = true;
    }

    public void Initialize(PlayerInfo pInfo) {
        _team = -1;

        _inputState.controllerNum = pInfo.controllerNum;
        _playerNum = pInfo.playerNum;
        SetCharacter(pInfo.characterName);
        if (pInfo.team == 0) {
            MoveLeft();
        } else {
            MoveRight();
        }
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

        // Reset the input for networked instances
        if (_photonView != null && _photonView.owner != PhotonNetwork.player) {
            _inputState = InputState.ResetInput(_inputState);
        }
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
        } /*else if (_team == 0) {
            _leftArrow.Deactivate();
            _rightArrow.Activate();
        } else if(_team == 1) {
            _leftArrow.Activate();
            _rightArrow.Deactivate();
        }*/

        //_leftArrow.transform.position = new Vector3(transform.position.x - 1f, transform.position.y, 0);
        //_rightArrow.transform.position = new Vector3(transform.position.x + 1f, transform.position.y, 0);
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
                transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
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

    public void SetCharacter(CHARACTERNAMES charaName) {
        _characterName = charaName;
        SetAnimator();
        
        // Update player info in player manager
        _playerManager.GetPlayerByNum(_playerNum).characterName = _characterName;
    }

    // Sets the sprite based on the current character
    void SetAnimator() {
        switch(_characterName) {
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
