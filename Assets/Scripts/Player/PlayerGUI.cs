using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerGUI : MonoBehaviour {
	PlayerController _playerController;

    ShiftMeter _shiftMeter;
    Material _meterMaterial;
    SpriteRenderer _shiftMeterEnd;

    Sprite _fullSprite;
    Sprite _emptySprite;

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
                teamLeftPlayers++;
            }
        } else if (_playerController.team == 1) {
            if(teamRightPlayers == 0) {
                teamRightPlayers++;
            }
        }

        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/UI/Level UI/Warp-Screen-Assets");
        _fullSprite = sprites[6];
        _emptySprite = sprites[5];

        if (_shiftMeter != null) {
            _meterMaterial = _shiftMeter.GetMeterFront().GetComponent<Image>().material;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (_shiftMeter != null) {
            _meterMaterial.SetFloat("_Cutoff", (_playerController.ShiftCooldownTimer / _playerController.ShiftCooldownTime));

            if(_playerController.ShiftCooldownTimer >= _playerController.ShiftCooldownTime) {
                _shiftMeterEnd.sprite = _fullSprite;
            } else {
                _shiftMeterEnd.sprite = _emptySprite;
            }
        }
	}
	
    public void SetMeter(ShiftMeter meter) {
        if (meter != null) {
            _shiftMeter = meter;
            _shiftMeterEnd = meter.GetMeterEnd();
        }
    }
}
