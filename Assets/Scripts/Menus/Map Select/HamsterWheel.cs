using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;

// Used to select the level in multiplayer
public class HamsterWheel : MonoBehaviour {
    public float baseRotSpeed;
    public Animator hamster;
    public SuperTextMesh stageDescription;

    float _curRotSpeed = 0;
    public float _desiredRotation;
    int[] _possibleRotations = new int[8];
    bool _rotatingRight = false;
    bool _rotatingLeft = false;
    public bool Rotating {
        get { return _rotatingRight || _rotatingLeft; }
    }

    bool _keepRotating; // generally only used for networking

    int _index = 0;
    BOARDS[] _stages = new BOARDS[8];
    StageIcon[] _stageIcons;

    float _longIdleTimer = 0f;
    float _longIdleTime = 5f;

    GameManager _gameManager;

    // Networking
    PhotonView _photonView;
    NetworkedMapSelect _netMapSelect;

    public int Index {
        get { return _index; }
        set { _index = value; }
    }

    private void Awake() {
        int type = Random.Range(0, 7);
        hamster.SetInteger("Type", type);
        hamster.SetInteger("State", 1);

        _photonView = GetComponent<PhotonView>();
        _netMapSelect = GetComponent<NetworkedMapSelect>();
    }

    // Use this for initialization
    void Start() {
        _gameManager = GameManager.instance;

        _stageIcons = GetComponentsInChildren<StageIcon>();
        // Scale down all the stage icons
        for (int i = 0; i < _stageIcons.Length; ++i) {
            _stageIcons[i].ScaleDown();
        }

        _possibleRotations[0] = 0;
        _possibleRotations[1] = 315;
        _possibleRotations[2] = 270;
        _possibleRotations[3] = 225;
        _possibleRotations[4] = 180;
        _possibleRotations[5] = 135;
        _possibleRotations[6] = 90;
        _possibleRotations[7] = 45;

        _stages[0] = BOARDS.FOREST;
        _stages[1] = BOARDS.MOUNTAIN;
        _stages[2] = BOARDS.BEACH;
        _stages[3] = BOARDS.CITY;
        _stages[4] = BOARDS.CORPORATION;
        _stages[5] = BOARDS.LABORATORY;
        _stages[6] = BOARDS.AIRSHIP;
        _stages[7] = BOARDS.CRASH;

        _curRotSpeed = 0;
        _desiredRotation = transform.rotation.eulerAngles.z;
        _rotatingRight = true;
    }

    // Update is called once per frame
    void Update() {
        if(!_gameManager.isOnline || (_netMapSelect != null && _netMapSelect.allPlayersLoaded)) {
            CheckInput();
        }

        if (Rotating) {
            transform.Rotate(0f, 0f, _curRotSpeed * Time.deltaTime);

            // If we get close enough to the desired rotation angle or we've past the failsafe timer
            if (Mathf.Abs(transform.rotation.eulerAngles.z - _desiredRotation) < 1f) {
                EndRotation();
            }
        } else {
            // Make sure the hamster is in idle
            hamster.SetInteger("State", 0);

            // Update long idle timer
            _longIdleTimer += Time.deltaTime;
            if (_longIdleTimer >= _longIdleTime) {
                hamster.SetBool("LongIdle", true);
                _longIdleTimer = -3f - Random.Range(2f, 7f);
            }
        }
    }

    private void LateUpdate() {
        // Have to fix the rotation every frame because Unity animation scales are stupid and don't make any sense
        if (_rotatingRight) {
            FlipHamster(true);
        } else if (_rotatingLeft) {
            FlipHamster(false);
        }
    }

    void CheckInput() {
        if (InputState.GetButtonOnAnyController("Left")) {
            RotateLeft();
        } else if (InputState.GetButtonOnAnyController("Right")) {
            RotateRight();
        }

        if(InputState.GetButtonOnAnyControllerPressed("Submit")) {
            if (!_stageIcons[_index].isLocked) {
                // Load the selected stage
                LoadSelectedMap();
            } else {
                // Play a little error sound?
            }
        }
        if (InputState.GetButtonOnAnyControllerPressed("Cancel")) {
            FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.SubMenuSelect);
            LoadCharacterSelect();
        }
    }

    public void RotateRight() {
        if (_rotatingRight) {
            return;
        }
        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.MainMenuHighlight);

        _index++;
        if (_index > 7) {
            _index = 0;
        }

        _curRotSpeed = -baseRotSpeed;
        _desiredRotation = _possibleRotations[_index];

        hamster.SetInteger("State", 1);
        FlipHamster(true);

        stageDescription.text = "";

        if (_photonView != null && PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient) {
            _photonView.RPC("RotateWheel", PhotonTargets.OthersBuffered, false);
        }

        _rotatingRight = true;
        _rotatingLeft = false;
    }

    public void RotateLeft() {
        if (_rotatingLeft) {
            return;
        }
        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.MainMenuHighlight);

        _index--;
        if (_index < 0) {
            _index = 7;
        }

        _curRotSpeed = baseRotSpeed;
        _desiredRotation = _possibleRotations[_index];

        hamster.SetInteger("State", 1);
        FlipHamster(false);

        stageDescription.text = "";

        if (_photonView != null && PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient) {
            _photonView.RPC("RotateWheel", PhotonTargets.OthersBuffered, true);
        }

        _rotatingLeft = true;
        _rotatingRight = false;
    }

    void EndRotation() {
        transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, _desiredRotation);

        // If we'r enot going to keep rotating
        if (!_keepRotating) {
            // End the rotation
            _rotatingRight = false;
            _rotatingLeft = false;
        } else {
            if(_rotatingRight) {
                _rotatingRight = false;
                RotateRight();
            } else if(_rotatingLeft) {
                _rotatingLeft = false;
                RotateLeft();
            }

            // Don't carry this over to next rotation
            _keepRotating = false;
        }

        // Increase size of current stage icon
        _stageIcons[_index].ScaleUp();

        // But check if we want to keep running past this map
        CheckInput();

        // If we didn't keep rotating
        if (!Rotating) {
            if (!_stageIcons[_index].isLocked) {
                stageDescription.text = _stageIcons[_index].stageDescription;
            } else {
                stageDescription.text = "???????";
            }
        }

        // Reset long idle timer
        _longIdleTimer = -3f - Random.Range(2f, 7f);
    }

    public void LoadSelectedMap() {
        _gameManager.stage = new int[2] { 0, 0 }; // make sure stage is set to nothing
        string levelName = "";
        if (_gameManager.isOnline) {
            levelName = "NetworkedMultiplayer";
        } else if (_gameManager.isSinglePlayer) {
            levelName = "SinglePlayer";
        } else if (_gameManager.gameMode == GAME_MODE.TEAMSURVIVAL) {
            levelName = "TeamSurvival";
        } else {
            levelName = "VersusMultiplayer";
        }

        _gameManager.selectedBoard = _stages[_index];

        if (_gameManager.isOnline && PhotonNetwork.connectedAndReady) {
            PhotonNetwork.RPC(GetComponent<PhotonView>(), "SetStage", PhotonTargets.Others, false, (int)_stages[_index]);
            PhotonNetwork.LoadLevel(levelName);
        } else {
            //LoadingScreen.sceneToLoad = levelName;
            //SceneManager.LoadScene("LoadingScreen");
            //SceneManager.LoadScene(levelName);
            _gameManager.sceneTransition.StartTransition(levelName);
        }

        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.MainMenuGameStart);
    }

    public void LoadCharacterSelect() {
        _gameManager.CharacterSelectButton();
    }

    public void FlipHamster(bool right) {
        hamster.SetBool("FacingRight", right);

        if (right) {
            // Multiply the player's x local scale by -1.
            Vector3 theScale = hamster.transform.localScale;
            theScale.x = Mathf.Abs(theScale.x);
            hamster.transform.localScale = theScale;
        } else {
            Vector3 theScale = hamster.transform.localScale;
            theScale.x = Mathf.Abs(theScale.x) * -1;
            hamster.transform.localScale = theScale;
        }
    }

    int NextIndex() {
        if(_index+1 >= _stageIcons.Length) {
            return 0;
        } else {
            return _index + 1;
        }
    }
    int PrevIndex() {
        if (_index-1 < 0) {
            return _stageIcons.Length-1;
        } else {
            return _index - 1;
        }
    }

    // Networking
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    }

    [PunRPC]
    void RotateWheel(bool left) {
        // If we're rotating the same direction
        if ((left && _rotatingLeft) || (!left && _rotatingRight)) {
            // It means we need to continue forward
            _keepRotating = true;
        } else {
            _keepRotating = false;

            if (left) {
                RotateLeft();
            } else {
                RotateRight();
            }
        }
    }

    [PunRPC]
    void SetStage(int selectedBoard) {
        _gameManager.selectedBoard = (BOARDS)selectedBoard;
    }
}
