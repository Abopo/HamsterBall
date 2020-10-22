using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedPlayer : Photon.MonoBehaviour {
    public int photonID;
    public Hamster tryingToCatchHamster;
    public Bubble thrownBubble;
    public SuperTextMesh playerName;

    PlayerController _playerController;
    InputState _serializedInput;

    int _correctState;

    float _bufferTime = 1f;
    float _bufferTimer = 0f;

    // Stupid input tracking stuff
    bool jumpPressed = false;
    bool swingPressed = false;
    bool attackPressed = false;

    Vector3 _arrowAngle;
    bool _facingRight;

    PhotonTransformView _photonTransformView;
    GameManager _gameManager;

    private void Awake() {
        _playerController = GetComponent<PlayerController>();
        _serializedInput = new InputState();
        _photonTransformView = GetComponent<PhotonTransformView>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    public void Start() {
        _serializedInput.SetPlayerID(0);

        // Get instantiation data
        PhotonView photonView = GetComponent<PhotonView>();
        // If we have instantiation data
        if (photonView.instantiationData != null) {
            // Then we were spawned by the Player Spawner and need to initialize stuff
            photonID = photonView.ownerId;

            if (photonView.owner == PhotonNetwork.player) {
                _playerController.SetInputID(0);
            }
            _playerController.SetPlayerNum((int)photonView.instantiationData[0]);
            _playerController.team = (int)photonView.instantiationData[1];

            CharaInfo tempInfo = new CharaInfo();
            tempInfo.name = (CHARACTERS)photonView.instantiationData[2];
            tempInfo.color = (int)photonView.instantiationData[3];
            _playerController.SetCharacterInfo(tempInfo);

            _playerController.FindHomeBubbleManager();

            // Make sure our player spawner has us in its list
            NetworkedPlayerSpawner playerSpawner = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<NetworkedPlayerSpawner>();
            playerSpawner.AddPlayer(_playerController);
            if (!PhotonNetwork.isMasterClient) {
                playerSpawner.SetupSwitchMeter(_playerController);
            }
        }

        if (photonView.owner != null) {
            playerName.text = photonView.owner.NickName;

            // Send name over?
            photonView.RPC("SendName", PhotonTargets.OthersBuffered, photonView.owner.NickName);
        }

        FindObjectOfType<GameManager>().gameOverEvent.AddListener(OnGameEnd);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            //Vector3 pos = transform.localPosition;
            //Quaternion rot = transform.localRotation;
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

            // If we're in the throw state
            if (_playerController.CurState == PLAYER_STATE.THROW) {
                // Serialize the aiming angle
                _arrowAngle = ((ThrowState)_playerController.currentState).aimingArrow.localEulerAngles;
                stream.Serialize(ref _arrowAngle);

                // And the player's facing?
                _facingRight = _playerController.FacingRight;
                stream.Serialize(ref _facingRight);
            }
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

            // Take all the input built up between updates
            _playerController.TakeInput(_serializedInput);

            ResetInput();

            if (_playerController.CurState == PLAYER_STATE.THROW) {
                stream.Serialize(ref _arrowAngle);
                stream.Serialize(ref _facingRight);
            }
        }
    }

    void ParseInput() {
        // Jump seems to be working great
        if (_serializedInput.jump.isDown) {
            if (!jumpPressed) {
                _serializedInput.jump.isJustPressed = true;
                jumpPressed = true;
            }
        } else {
            if (jumpPressed) {
                _serializedInput.jump.isJustReleased = true;
                jumpPressed = false;
            }
        }

        // But swing and attack are still not consistent
        if (_serializedInput.swing.isDown) {
            if (!swingPressed) {
                _serializedInput.swing.isJustPressed = true;
                swingPressed = true;
            }
        } else {
            swingPressed = false;
        }

        if (_serializedInput.attack.isDown) {
            if (!attackPressed) {
                _serializedInput.attack.isJustPressed = true;
                attackPressed = true;
            }
        } else {
            attackPressed = false;
        }
    }

    public void FixedUpdate() {
        if (PhotonNetwork.connectedAndReady && !_gameManager.gameIsOver) {
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

            if (photonView.isMine) {
                _photonTransformView.SetSynchronizedValues(_playerController.velocity, 0f);
            }

            // If we are aiming
            if (!photonView.isMine && _playerController.CurState == PLAYER_STATE.THROW) {
                // Update the arrow to be in the right direction
                ((ThrowState)_playerController.currentState).aimingArrow.localEulerAngles = _arrowAngle;
                
                // Make sure player is facing the correct direction
                if(_playerController.FacingRight != _facingRight) {
                    _playerController.Flip();
                }
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
        if (tryingToCatchHamster != null && tryingToCatchHamster.hamsterNum == hamsterNum) {
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
    void ThrowBubble(Vector3 playerPos, Quaternion arrowRot) {
        // We need to make sure the player is in exactly the right position when they throw
        _playerController.transform.position = playerPos;
        // And that they are facing the correct direction
        if (_playerController.FacingRight != _facingRight) {
            _playerController.Flip();
        }

        // Then throw the bubble
        ThrowState throwState = (ThrowState)_playerController.GetPlayerState(PLAYER_STATE.THROW);
        throwState.aimingArrow.localRotation = arrowRot;
        throwState.StartThrow();
    }

    [PunRPC]
    void TryThrowBubble(Vector3 playerPos, Quaternion arrowRot) {
        // Only the master client can check
        if (PhotonNetwork.isMasterClient) {
            // Check if the player has been punched or not
            // If we're still in the throw state we're good
            if (_playerController.CurState == PLAYER_STATE.THROW) {
                // Send back the confirmation
                GetComponent<PhotonView>().RPC("ThrowBubble", PhotonTargets.All, playerPos, arrowRot);

                // If we've been knocked out of the throw state
            } else {
                // TODO: send a response?
                photonView.RPC("ThrowFailed", PhotonTargets.Others);
            }
        }
    }

    [PunRPC]
    void ThrowFailed() {
        // Ok so we tried throwing, but on the master client we had been hit

        // So destroy our thrown bubble
        if (thrownBubble != null) {
            PhotonNetwork.Destroy(thrownBubble.GetComponent<PhotonView>());
        } else {
            // If we got here abut don't have a thrown bubble, something's big time wrong
            Debug.LogError("Failed throw, but found no thrown bubble. Uh ohs...");
        }
    }

    [PunRPC]
    void ShiftPlayer() {
        _playerController.StartShift();
    }

    [PunRPC]
    void SendName(string nickname) {
        playerName.text = nickname;
    }

    void OnGameEnd() {
        // stop synching position
        _photonTransformView.enabled = false;
    }

    private void OnDestroy() {
        if (PhotonNetwork.connectedAndReady) {
            // TODO: this is the quick fix for now, maybe update to be better
            if (GetComponent<CSPlayerController>() != null) {
                return;
            }

            // Only the master client should try and destroy things
            if (PhotonNetwork.isMasterClient) {
                if (PhotonNetwork.player != photonView.owner) {
                    photonView.TransferOwnership(PhotonNetwork.masterClient);
                }
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
