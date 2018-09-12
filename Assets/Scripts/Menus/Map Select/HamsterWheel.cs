using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Used to select the level in multiplayer
public class HamsterWheel : MonoBehaviour {
    public float baseRotSpeed;
    public Text curMapText;
    public Animator hamster;

    float _curRotSpeed;
    public float _desiredRotation;
    bool _rotating = false;
    public bool Rotating {
        get { return _rotating; }
    }
    float _failsafeTime = 1.75f;
    float _failsafeTimer = 0f;

    int _index = 0;
    string[] _mapNames = new string[8];
    StageIcon[] _stageIcons;

    GameManager _gameManager;
    AudioSource _audioSource;

    PhotonView _photonView;

    public int Index {
        get { return _index; }
        set { _index = value; }
    }

    // Use this for initialization
    void Start() {
        _stageIcons = GetComponentsInChildren<StageIcon>();

        _mapNames[0] = "Forest";
        _mapNames[1] = "Mountain";
        _mapNames[2] = "Beach";
        _mapNames[3] = "City2";
        _mapNames[4] = "Sewers";
        _mapNames[5] = "Laboratory";
        _mapNames[6] = "DarkForest";
        _mapNames[7] = "Space";

        curMapText.text = _mapNames[_index];
        SetTextColor();

        // TODO: set random hamster
        int type = Random.Range(0, 7);
        hamster.SetInteger("Type", type);
        hamster.SetInteger("State", 0);

        _gameManager = FindObjectOfType<GameManager>();
        _audioSource = GetComponent<AudioSource>();

        _photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update() {
        CheckInput();

        if (_rotating) {
            transform.Rotate(0f, 0f, _curRotSpeed * Time.deltaTime);
            _failsafeTimer += Time.deltaTime;

            // If we get close enough to the desired rotation angle or we've past the failsafe timer
            if (Mathf.Abs(transform.rotation.eulerAngles.z - _desiredRotation) < 1f) {
                EndRotation();
                _failsafeTimer = 0f;
            }
        }
    }

    void CheckInput() {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) ||
            (Input.GetAxis("Horizontal") < -0.3f) || Input.GetAxis("Horizontal DPad") < -0.3f) {
            RotateLeft();
        } else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) ||
            (Input.GetAxis("Horizontal") > 0.3f) || Input.GetAxis("Horizontal DPad") > 0.3f) {
            RotateRight();
        }

        if (Input.GetButtonDown("Cancel")) {
            _audioSource.Play();
            LoadCharacterSelect();
        }
    }

    public void RotateRight() {
        if (_rotating || _stageIcons[NextIndex()].isLocked) {
            return;
        }
        _audioSource.Play();

        _curRotSpeed = -baseRotSpeed;
        _desiredRotation = transform.eulerAngles.z - 45f;
        if (_desiredRotation < -1) {
            _desiredRotation = 315f;
        }

        _index++;
        if (_index > 7) {
            _index = 0;
        }

        hamster.SetInteger("State", 1);
        FlipHamster(true);

        if (_photonView != null && PhotonNetwork.connectedAndReady) {
            _photonView.RPC("RotateWheel", PhotonTargets.OthersBuffered, false);
        }

        _rotating = true;
    }

    public void RotateLeft() {
        if (_rotating || _stageIcons[PrevIndex()].isLocked) {
            return;
        }
        _audioSource.Play();

        _curRotSpeed = baseRotSpeed;
        _desiredRotation = transform.eulerAngles.z + 45f;
        if (_desiredRotation > 361) {
            _desiredRotation = 45f;
        }

        _index--;
        if (_index < 0) {
            _index = 7;
        }

        hamster.SetInteger("State", 1);
        FlipHamster(false);

        if (_photonView != null && PhotonNetwork.connectedAndReady) {
            _photonView.RPC("RotateWheel", PhotonTargets.OthersBuffered, true);
        }

        _rotating = true;
    }

    void EndRotation() {
        // End the rotation
        _rotating = false;
        transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, _desiredRotation);

        // But check if we want to keep running past this map
        CheckInput();

        // If we didn't keep rotating
        if (!_rotating) {
            // Fully stop
            curMapText.text = _mapNames[_index];
            SetTextColor();
            hamster.SetInteger("State", 0);
        }
    }

    public void LoadSelectedMap() {
        _gameManager.stage = "";
        string levelName = "";
        if (_gameManager.isOnline) {
            levelName = "Networked ";
        }
        levelName += _mapNames[_index];

        if (_gameManager.isSinglePlayer) {
            levelName += " - SinglePlayer";
        } else if (_gameManager.gameMode == GAME_MODE.TEAMSURVIVAL) {
            levelName += " - Team Survival";
        }

        if (_gameManager.isOnline && PhotonNetwork.connectedAndReady) {
            PhotonNetwork.LoadLevel(levelName);
        } else {
            SceneManager.LoadScene(levelName);
        }
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

    void SetTextColor() {
        Color newColor = new Color();
        switch (curMapText.text) {
            case "Forest":
                ColorUtility.TryParseHtmlString("#00FF4CFF", out newColor);
                break;
            case "Mountain":
                ColorUtility.TryParseHtmlString("#11FFEBFF", out newColor);
                break;
            case "Beach":
                ColorUtility.TryParseHtmlString("#FBEC99FF", out newColor);
                break;
            default:
                ColorUtility.TryParseHtmlString("#FFFFFFFF", out newColor);
                break;
        }

        curMapText.color = newColor;
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
