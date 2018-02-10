using UnityEngine;
using System.Collections;

public class PlayerGUI : MonoBehaviour {
	public GUIStyle shiftBarStyle;
	public GUIStyle shiftBarBackStyle;
	
	PlayerController _playerController;

    RectTransform _shiftMeter;
    float _shiftMeterWidth;

    bool _topPlayer;
    static int teamLeftPlayers;
    static int teamRightPlayers;

    void Awake() {
        teamLeftPlayers = 0;
        teamRightPlayers = 0;
    }

	// Use this for initialization
	void Start () {
		_playerController = GetComponent<PlayerController> ();

        if (_playerController.team == 0) {
            if (teamLeftPlayers == 0) {
                _topPlayer = true;
                teamLeftPlayers++;
            } else {
                _topPlayer = false;
            }
        } else if (_playerController.team == 1) {
            if(teamRightPlayers == 0) {
                _topPlayer = true;
                teamRightPlayers++;
            } else {
                _topPlayer = false;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (_shiftMeter != null) {
            _shiftMeter.sizeDelta = new Vector2(_shiftMeterWidth * (_playerController.ShiftCooldownTimer / _playerController.ShiftCooldownTime), _shiftMeter.sizeDelta.y);
        }
	}
	
	void OnGUI() {
        if(_playerController.team == 0) {
            if(_topPlayer) {
                //GUI.Box(new Rect(meterIconPos.x + 5, Screen.height - meterIconPos.y - 2, (Screen.width / 65) * _playerController.ShiftCooldownTime, Screen.height / 50), "", shiftBarBackStyle);
                //GUI.Box(new Rect(meterIconPos.x + 5, Screen.height - meterIconPos.y - 2, (Screen.width / 65) * _playerController.ShiftCooldownTimer, Screen.height / 50), "", shiftBarStyle);
            } else {
                //GUI.Box(new Rect(meterIconPos.x + 5, Screen.height - meterIconPos.y - 2, (Screen.width / 65) * _playerController.ShiftCooldownTime, Screen.height / 50), "", shiftBarBackStyle);
                //GUI.Box(new Rect(meterIconPos.x + 5, Screen.height - meterIconPos.y - 2, (Screen.width / 65) * _playerController.ShiftCooldownTimer, Screen.height / 50), "", shiftBarStyle);
            }
        } else if(_playerController.team == 1) {
            if (_topPlayer) {
                //GUI.Box(new Rect(meterIconPos.x + 5, Screen.height - meterIconPos.y - 2, (Screen.width / 65) * _playerController.ShiftCooldownTime, Screen.height / 50), "", shiftBarBackStyle);
                //GUI.Box(new Rect(meterIconPos.x + 5, Screen.height - meterIconPos.y - 2, (Screen.width / 65) * _playerController.ShiftCooldownTimer, Screen.height / 50), "", shiftBarStyle);
            } else {
                //GUI.Box(new Rect(meterIconPos.x + 5, Screen.height - meterIconPos.y - 2, (Screen.width / 65) * _playerController.ShiftCooldownTime, Screen.height / 50), "", shiftBarBackStyle);
                //GUI.Box(new Rect(meterIconPos.x + 5, Screen.height - meterIconPos.y - 2, (Screen.width / 65) * _playerController.ShiftCooldownTimer, Screen.height / 50), "", shiftBarStyle);
            }
        }
	}

    public void SetMeterPosition(RectTransform meter) {
        if (meter != null) {
            _shiftMeter = meter;
            _shiftMeterWidth = _shiftMeter.sizeDelta.x;
        }
    }
}
