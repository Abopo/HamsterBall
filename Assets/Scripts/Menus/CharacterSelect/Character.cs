using UnityEngine;
using System.Collections.Generic;

public class Character : MonoBehaviour {
    public Team teamLeft;
    public Team teamRight;
    public bool isAI;
    public bool takeInput;
    //public Character aiChild;
    public Stack<Character> aiChildren = new Stack<Character>();
    Character humanParent;

    int _playerNum;
    int _team; // -1 = no team, 0 = left team, 1 = right team
    bool _active;

    Vector2 _initialPos;

    InputState _inputState;
    public int ControllerNum {
        get {
            return _inputState.controllerNum;
        }
    }

    PlayerManager _playerManager;
    SpriteRenderer _spriteRenderer;
    AudioSource _audioSource;

    AudioClip _activateClip;
    AudioClip _deactivateClip;
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

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();

        _activateClip = Resources.Load<AudioClip>("Audio/SFX/Activate_Character");
        _deactivateClip = Resources.Load<AudioClip>("Audio/SFX/Deactivate_Character");
        _moveClip = Resources.Load<AudioClip>("Audio/SFX/Blip_Select");

        _photonView = GetComponent<PhotonView>();
        isLocal = true;

        GetArrows();
    }

    void GetArrows() {
        // Get arrows
        TeamSelectArrow[] arrows = transform.GetComponentsInChildren<TeamSelectArrow>();
        if (arrows[0].side == 0) {
            _leftArrow = arrows[0];
            _rightArrow = arrows[1];
        } else {
            _rightArrow = arrows[0];
            _leftArrow = arrows[1];
        }

        _leftArrow.transform.parent = null;
        _rightArrow.transform.parent = null;
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
        if(_photonView != null && _photonView.owner != PhotonNetwork.player) {
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

        _leftArrow.GetComponent<SpriteRenderer>().enabled = false;
        _rightArrow.GetComponent<SpriteRenderer>().enabled = false;

        if (isAI && humanParent != null) {
            humanParent = null;
        }


        PlayDeactivateClip();
    }

    public bool AllAIAssigned() {
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
}
