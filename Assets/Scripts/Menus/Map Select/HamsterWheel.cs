using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Used to select the level in multiplayer
public class HamsterWheel : MonoBehaviour {
    public float baseRotSpeed;
    public Animator hamster;

    float _curRotSpeed = 0;
    public float _desiredRotation;
    int[] _possibleRotations = new int[8];
    bool _rotatingRight = false;
    bool _rotatingLeft = false;
    public bool Rotating {
        get { return _rotatingRight || _rotatingLeft; }
    }

    int _index = 0;
    BOARDS[] _stages = new BOARDS[8];
    StageIcon[] _stageIcons;

    float _longIdleTimer = 0f;
    float _longIdleTime = 5f;

    GameManager _gameManager;
    AudioSource _audioSource;

    PhotonView _photonView;

    public int Index {
        get { return _index; }
        set { _index = value; }
    }

    private void Awake() {
        int type = Random.Range(0, 7);
        hamster.SetInteger("Type", type);
        hamster.SetInteger("State", 1);
    }
    // Use this for initialization
    void Start() {
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
        _stages[6] = BOARDS.CORPORATION;
        _stages[7] = BOARDS.AIRSHIP;

        _curRotSpeed = 0;
        _desiredRotation = transform.rotation.eulerAngles.z;
        _rotatingRight = true;

        _gameManager = FindObjectOfType<GameManager>();
        _audioSource = GetComponent<AudioSource>();

        _photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update() {
        CheckInput();

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
        if (_gameManager.playerInput.GetAxis("Horizontal0") < -0.3f || _gameManager.playerInput.GetButtonDown("Left")) {
            RotateLeft();
        } else if (_gameManager.playerInput.GetAxis("Horizontal0") > 0.3f || _gameManager.playerInput.GetButtonDown("Right")) {
            RotateRight();
        }

        if (_gameManager.playerInput.GetButtonDown("Cancel")) {
            _audioSource.Play();
            LoadCharacterSelect();
        }
    }

    public void RotateRight() {
        if (_rotatingRight || _stageIcons[NextIndex()].isLocked) {
            return;
        }
        _audioSource.Play();

        // Reduce size of current stage icon
        //_stageIcons[_index].ScaleDown();

        _index++;
        if (_index > 7) {
            _index = 0;
        }

        _curRotSpeed = -baseRotSpeed;
        _desiredRotation = _possibleRotations[_index];
        //_desiredRotation = transform.eulerAngles.z - 45f;
        //if (_desiredRotation < -1) {
        //   _desiredRotation = 315f;
        //}

        hamster.SetInteger("State", 1);
        FlipHamster(true);

        if (_photonView != null && PhotonNetwork.connectedAndReady) {
            _photonView.RPC("RotateWheel", PhotonTargets.OthersBuffered, false);
        }

        _rotatingRight = true;
        _rotatingLeft = false;
    }

    public void RotateLeft() {
        if (_rotatingLeft || _stageIcons[PrevIndex()].isLocked) {
            return;
        }
        _audioSource.Play();

        // Reduce size of current stage icon
        //_stageIcons[_index].ScaleDown();

        _index--;
        if (_index < 0) {
            _index = 7;
        }

        _curRotSpeed = baseRotSpeed;
        _desiredRotation = _possibleRotations[_index];
        //_desiredRotation = transform.eulerAngles.z + 45f;
        //if (_desiredRotation > 361) {
        //    _desiredRotation = 45f;
        //}

        hamster.SetInteger("State", 1);
        FlipHamster(false);

        if (_photonView != null && PhotonNetwork.connectedAndReady) {
            _photonView.RPC("RotateWheel", PhotonTargets.OthersBuffered, true);
        }

        _rotatingLeft = true;
        _rotatingRight = false;
    }

    void EndRotation() {
        // End the rotation
        _rotatingRight = false;
        _rotatingLeft = false;
        transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, _desiredRotation);

        // Increase size of current stage icon
        _stageIcons[_index].ScaleUp();

        // But check if we want to keep running past this map
        CheckInput();

        // If we didn't keep rotating
        if (!Rotating) {
            // Fully stop
            //hamster.SetInteger("State", 0);
        }

        // Reset long idle timer
        _longIdleTimer = -3f - Random.Range(2f, 7f);
    }

    public void LoadSelectedMap() {
        _gameManager.stage = "";
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
            PhotonNetwork.LoadLevel(levelName);
        } else {
            //LoadingScreen.sceneToLoad = levelName;
            //SceneManager.LoadScene("LoadingScreen");
            SceneManager.LoadScene(levelName);
        }
		FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.MainMenuGameStart);
    }

    public void LoadCharacterSelect() {
        _gameManager.CharacterSelectButton();
    }

    public void FlipHamster(bool right) {
        hamster.SetBool("FacingLeft", !right);

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
        if (left) {
            RotateLeft();
        } else {
            RotateRight();
        }
    }
}
