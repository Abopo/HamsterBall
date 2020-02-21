using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedPlayer : Photon.MonoBehaviour {
    public Hamster tryingToCatchHamster;

    PlayerController _playerController;
    InputState _serializedInput;

    int _correctState;

    float _bufferTime = 1f;
    float _bufferTimer = 0f;

    // Stupid input tracking stuff
    bool jumpPressed = false;
    bool swingPressed = false;
    bool attackPressed = false;

    private void Awake() {
        _playerController = GetComponent<PlayerController>();
        _serializedInput = new InputState();
    }

    public void Start() {
        _serializedInput.SetPlayerID(0);

        // Get instantiation data
        PhotonView photonView = GetComponent<PhotonView>();
        // If we have instantiation data
        if (photonView.instantiationData != null) {
            // Then we were spawned by the Player Spawner and need to initialize stuff

            if (photonView.owner == PhotonNetwork.player) {
                _playerController.SetInputID(0);
            }
            _playerController.SetPlayerNum((int)photonView.instantiationData[0]);
            _playerController.team = (int)photonView.instantiationData[1];

            CharaInfo tempInfo = new CharaInfo();
            tempInfo.name = (CHARACTERS)photonView.instantiationData[2];
            tempInfo.color = (int)photonView.instantiationData[3];
            _playerController.SetCharacterInfo(tempInfo);

            // Make sure our player spawner has us in its list
            NetworkedPlayerSpawner playerSpawner = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<NetworkedPlayerSpawner>();
            playerSpawner.AddPlayer(_playerController);
            if (!PhotonNetwork.isMasterClient) {
                playerSpawner.SetupSwitchMeter(_playerController);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            Vector3 pos = transform.localPosition;
            Quaternion rot = transform.localRotation;
            int state = (int)_playerController.CurState;

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
            stream.Serialize(ref _serializedInput.swing.isDown);
            stream.Serialize(ref _serializedInput.swing.isJustPressed);
            //stream.Serialize(ref _serializedInput.shift.isDown);
            //stream.Serialize(ref _serializedInput.shift.isJustPressed);
            //stream.Serialize(ref _serializedInput.shift.isJustReleased);
            stream.Serialize(ref _serializedInput.attack.isDown);
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
            stream.Serialize(ref _serializedInput.swing.isDown);
            stream.Serialize(ref _serializedInput.swing.isJustPressed);
            //stream.Serialize(ref _serializedInput.shift.isDown);
            //stream.Serialize(ref _serializedInput.shift.isJustPressed);
            //stream.Serialize(ref _serializedInput.shift.isJustReleased);
            stream.Serialize(ref _serializedInput.attack.isDown);
            stream.Serialize(ref _serializedInput.attack.isJustPressed);
            //stream.Serialize(ref _serializedInput.attack.isJustReleased);

            // Double check for "pressed" and "released" inputs
            ParseInput();

            if (_serializedInput.jump.isJustPressed) {
                Debug.Log("Jump button pressed");
            }
            if (_serializedInput.jump.isDown) {
                Debug.Log("Jump button down");
            }

            // Take all the input built up between updates
            _playerController.TakeInput(_serializedInput);

            ResetInput();
        }
    }

    void ParseInput() {
        // Jump seems to be working great
        if(_serializedInput.jump.isDown) {
            if(!jumpPressed) {
                _serializedInput.jump.isJustPressed = true;
                jumpPressed = true;
            }
        } else {
            if(jumpPressed) {
                _serializedInput.jump.isJustReleased = true;
                jumpPressed = false;
            }
        }

        // But swing and attack are still not consistent
        if(_serializedInput.swing.isDown) {
            if(!swingPressed) {
                _serializedInput.swing.isJustPressed = true;
                swingPressed = true;
            }
        } else {
            swingPressed = false;
        }

        if(_serializedInput.attack.isDown) {
            if(!attackPressed) {
                _serializedInput.attack.isJustPressed = true;
                attackPressed = true;
            }
        } else {
            attackPressed = false;
        }
    }

    public void FixedUpdate() {
        if (PhotonNetwork.connectedAndReady) {
            if (!photonView.isMine && _correctState != (int)_playerController.CurState) {
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

    [PunRPC]
    void ChangePlayerState(int state) {
        if (_playerController.CurState != (PLAYER_STATE)state) {
            _playerController.ChangeState((PLAYER_STATE)state);
        }
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
                photonView.RPC("HamsterCaught", PhotonTargets.Others, hamster.hamsterNum);
                OtherCatchHamster(hamster.hamsterNum); 
            } else {
                // Tell original player that they can't catch that hamster
                photonView.RPC("HamsterMissed", PhotonTargets.Others);
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
            _playerController.swingObj.GetComponent<CatchHitbox>().CatchHamster(tryingToCatchHamster);

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
        throwState.StartThrow();
    }

    [PunRPC]
    void TryThrowBubble(Quaternion arrowRot) {
        // Only the master client can check
        if (PhotonNetwork.isMasterClient) {
            // Check if the player has been punched or not
            // If we're still in the throw state we're good
            if (_playerController.CurState == PLAYER_STATE.THROW) {
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

    [PunRPC]
    void ShiftPlayer() {
        _playerController.StartShift();
    }
}
