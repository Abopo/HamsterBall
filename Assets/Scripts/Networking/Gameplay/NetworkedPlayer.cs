using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedPlayer : Photon.MonoBehaviour {
    public Hamster tryingToCatchHamster;

    PlayerController _playerController;
    InputState _serializedInput;
    //List<InputState> _writingInputList = new List<InputState>();
    //List<InputState> _readingInputList = new List<InputState>();

    int _correctState;

    float _bufferTime = 0.2f;
    float _bufferTimer = 0f;

    private void Awake() {
        _playerController = GetComponent<PlayerController>();
        _serializedInput = new InputState();
    }

    public void Start() {
        // Get instantiation data
        PhotonView photonView = GetComponent<PhotonView>();
        _playerController.playerNum = (int)photonView.instantiationData[0];
        _playerController.team = (int)photonView.instantiationData[1];

        CharaInfo tempInfo = new CharaInfo();
        tempInfo.name = (CHARACTERS)photonView.instantiationData[2];
        tempInfo.color = (int)photonView.instantiationData[3];
        SetAnimatorController(tempInfo);
        //SetAnimatorController((CHARACTERNAMES)photonView.instantiationData[2]);

        // Make sure our player spawner has us in its list
        NetworkedPlayerSpawner playerSpawner = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<NetworkedPlayerSpawner>();
        playerSpawner.AddPlayer(_playerController);
        if (!PhotonNetwork.isMasterClient) {
            playerSpawner.SetupSwitchMeter(_playerController);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            Vector3 pos = transform.localPosition;
            Quaternion rot = transform.localRotation;
            int state = (int)_playerController.curState;

            //stream.Serialize(ref pos);
            //stream.Serialize(ref rot);
            stream.Serialize(ref state);

            stream.Serialize(ref _serializedInput.jump.isDown);
            stream.Serialize(ref _serializedInput.jump.isJustReleased);
            stream.Serialize(ref _serializedInput.jump.isJustPressed);
            stream.Serialize(ref _serializedInput.left.isDown);
            stream.Serialize(ref _serializedInput.left.isJustPressed);
            stream.Serialize(ref _serializedInput.left.isJustReleased);
            stream.Serialize(ref _serializedInput.right.isDown);
            stream.Serialize(ref _serializedInput.right.isJustPressed);
            stream.Serialize(ref _serializedInput.right.isJustReleased);
            stream.Serialize(ref _serializedInput.swing.isJustPressed);
            //stream.Serialize(ref _serializedInput.shift.isDown);
            stream.Serialize(ref _serializedInput.shift.isJustPressed);
            //stream.Serialize(ref _serializedInput.shift.isJustReleased);
            //stream.Serialize(ref _serializedInput.attack.isDown);
            stream.Serialize(ref _serializedInput.attack.isJustPressed);
            //stream.Serialize(ref _serializedInput.attack.isJustReleased);

            ResetInput();
        } else {
            // Receive latest state information

            stream.Serialize(ref _correctState);

            stream.Serialize(ref _serializedInput.jump.isDown);
            stream.Serialize(ref _serializedInput.jump.isJustReleased);
            stream.Serialize(ref _serializedInput.jump.isJustPressed);
            stream.Serialize(ref _serializedInput.left.isDown);
            stream.Serialize(ref _serializedInput.left.isJustPressed);
            stream.Serialize(ref _serializedInput.left.isJustReleased);
            stream.Serialize(ref _serializedInput.right.isDown);
            stream.Serialize(ref _serializedInput.right.isJustPressed);
            stream.Serialize(ref _serializedInput.right.isJustReleased);
            stream.Serialize(ref _serializedInput.swing.isJustPressed);
            //stream.Serialize(ref _serializedInput.shift.isDown);
            stream.Serialize(ref _serializedInput.shift.isJustPressed);
            //stream.Serialize(ref _serializedInput.shift.isJustReleased);
            //stream.Serialize(ref _serializedInput.attack.isDown);
            stream.Serialize(ref _serializedInput.attack.isJustPressed);
            //stream.Serialize(ref _serializedInput.attack.isJustReleased);

            // Take all the input built up between updates
            _playerController.TakeInput(_serializedInput);

            ResetInput();
        }
    }

    public void FixedUpdate() {
        if (!photonView.isMine && _correctState != (int)_playerController.curState) {
            _bufferTimer += Time.deltaTime;
            if (_bufferTimer >= _bufferTime) {
                _playerController.ChangeState((PLAYER_STATE)_correctState);
            }
        } else {
            _bufferTimer = 0f;
        }

        if (_playerController.inputState != null) {
            GetOwnerInput();
        }
    }

    void GetOwnerInput() {
        if (_playerController.inputState.jump.isDown) {
            _serializedInput.jump.isDown = true;
        }
        if (_playerController.inputState.jump.isJustReleased) {
            _serializedInput.jump.isJustReleased = true;
        }
        if (_playerController.inputState.jump.isJustPressed) {
            _serializedInput.jump.isJustPressed = true;
        }
        if (_playerController.inputState.left.isDown) {
            _serializedInput.left.isDown = true;
        }
        if (_playerController.inputState.left.isJustPressed) {
            _serializedInput.left.isJustPressed = true;
        }
        if (_playerController.inputState.left.isJustReleased) {
            _serializedInput.left.isJustReleased = true;
        }
        if (_playerController.inputState.right.isDown) {
            _serializedInput.right.isDown = true;
        }
        if (_playerController.inputState.right.isJustPressed) {
            _serializedInput.right.isJustPressed = true;
        }
        if (_playerController.inputState.right.isJustReleased) {
            _serializedInput.right.isJustReleased = true;
        }
        if (_playerController.inputState.swing.isJustPressed) {
            _serializedInput.swing.isJustPressed = true;
        }
        //stream.Serialize(ref _serializedInput.shift.isDown);
        if (_playerController.inputState.shift.isJustPressed) {
            _serializedInput.shift.isJustPressed = true;
        }
        //stream.Serialize(ref _serializedInput.shift.isJustReleased);
        //stream.Serialize(ref _serializedInput.attack.isDown);
        if (_playerController.inputState.attack.isJustPressed) {
            _serializedInput.attack.isJustPressed = true;
        }

        //stream.Serialize(ref _serializedInput.attack.isJustReleased);
    }

    void ResetInput() {
        _serializedInput = new InputState();
        //_writingInputList.Clear();
    }

    void SetAnimatorController(CharaInfo charaInfo) {
        string path = "Art/Animations/Player/";
        switch (charaInfo.name) {
            case CHARACTERS.BOY:
                path += "Boy/Animation Objects/Boy" + charaInfo.color;
                break;
            case CHARACTERS.GIRL:
                path += "Girl/Animation Objects/Girl" + charaInfo.color;
                break;
            case CHARACTERS.ROOSTER:
                path += "Rooster/Animation Objects/Rooster" + charaInfo.color;
                break;
        }

        _playerController.Animator.runtimeAnimatorController = Resources.Load(path) as RuntimeAnimatorController;

        /*
        switch (charaName) {
            case CHARACTERNAMES.BOY1:
                _playerController.Animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy1") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.BOY2:
                _playerController.Animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy2") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.BOY3:
                _playerController.Animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy3") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.BOY4:
                _playerController.Animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy4") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.GIRL1:
                _playerController.Animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Girl/Animation Objects/Girl1") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.GIRL2:
                _playerController.Animator.runtimeAnimatorController = Resources.Load("Art/Animations/Player/Girl/Animation Objects/Girl2") as RuntimeAnimatorController;
                break;
        }
        */
    }

    [PunRPC]
    void CheckHamster(int hamsterNum) {
        // Only the master client can check a hamster's state
        if (PhotonNetwork.isMasterClient) {
            // find the hamster with the same number
            HamsterScan hamsterScan = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<HamsterScan>();
            Hamster hamster = hamsterScan.GetHamster(hamsterNum);

            if (hamster != null && !hamster.wasCaught) {
                // Tell rest of players that a hamster was caught
                photonView.RPC("HamsterCaught", PhotonTargets.All, hamster.hamsterNum);
            } else {
                // Tell original player that they can't catch that hamster
                photonView.RPC("HamsterMissed", PhotonTargets.All);
            }
        }
    }

    [PunRPC]
    void HamsterCaught(int hamsterNum) {
        if(tryingToCatchHamster != null && tryingToCatchHamster.hamsterNum == hamsterNum) {
            MeCatchHamster();
        } else {
            OtherCatchHamster(hamsterNum);
        }


        _playerController.aimCooldownTimer = 0.0f;

        // For now this is only for the AI
        _playerController.significantEvent.Invoke();
    }

    // When this player catches a hamster
    void MeCatchHamster() {
        if (_playerController.heldBall == null) {
            //_playerController.attackBubble.GetComponent<AttackBubble>().CatchHamster(tryingToCatchHamster);

            // For some reason, the client side player will sometimes go straight into the throw state after catching a hamster
            // Trying to prevent that
            _playerController.inputState.swing.isJustPressed = false;
        }
    }

    // When another player catches a hamster
    void OtherCatchHamster(int hamsterNum) {
        // find the hamster with the same number
        HamsterScan hamsterScan = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<HamsterScan>();
        Hamster hamster = hamsterScan.GetHamster(hamsterNum);

        if (hamster != null) {
            // For some reason, the client side player will sometimes go straight into the throw state after catching a hamster
            // Trying to prevent that
            _playerController.inputState.swing.isJustPressed = false;

            // The hamster was caught.
            hamster.Caught();
        }
    }

    [PunRPC]
    void HamsterMissed() {
        // If we were the player to try and catch this bubble
        if (_playerController.heldBall.GetComponent<PhotonView>().owner == PhotonNetwork.player) { 
            // Destroy the held bubble
            PhotonNetwork.Destroy(_playerController.heldBall.gameObject);
        }
    }

    [PunRPC]
    void ThrowBubble(Quaternion arrowRot) {
        ThrowState throwState = (ThrowState)_playerController.GetPlayerState(PLAYER_STATE.THROW);
        throwState.aimingArrow.localRotation = arrowRot;
        throwState.Throw();
    }

    [PunRPC]
    void TryThrowBubble(Quaternion arrowRot) {
        // Only the master client can check
        if (PhotonNetwork.isMasterClient) {
            // Check if the player has been punched or not
            // If we're still in the throw state we're good
            if (_playerController.curState == PLAYER_STATE.THROW) {
                // Throw the bubble!
                ThrowBubble(arrowRot);

                // Send back the confirmation
                GetComponent<PhotonView>().RPC("ThrowBubble", PhotonTargets.Others, arrowRot);

            // If we've been knocked out of the throw state
            } else {
                // TODO: send a response?
            }
        }
    }
}
