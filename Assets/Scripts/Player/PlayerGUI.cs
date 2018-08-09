using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerGUI : MonoBehaviour {
	PlayerController _playerController;

    RectTransform _shiftMeter;
    Material _meterMaterial;
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

        _meterMaterial = _shiftMeter.GetComponent<Image>().material;
	}
	
	// Update is called once per frame
	void Update () {
        if (_shiftMeter != null) {
            //_shiftMeter.sizeDelta = new Vector2(_shiftMeterWidth * (_playerController.ShiftCooldownTimer / _playerController.ShiftCooldownTime), _shiftMeter.sizeDelta.y);
            _meterMaterial.SetFloat("_Cutoff", (_playerController.ShiftCooldownTimer / _playerController.ShiftCooldownTime));
        }
	}
	
    public void SetMeterPosition(RectTransform meter) {
        if (meter != null) {
            _shiftMeter = meter;
            _shiftMeterWidth = _shiftMeter.sizeDelta.x;
        }
    }
}
