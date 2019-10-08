using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerGUI : MonoBehaviour {
	PlayerController _playerController;

    ShiftMeter _shiftMeter;
    Material _meterMaterial;
    Image _shiftMeterEnd;

    Sprite _fullSprite;
    Sprite _emptySprite;

    static int teamLeftPlayers;
    static int teamRightPlayers;

    float tempFloat;

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

        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/UI/Level UI/New-Shift-assets");
        _fullSprite = sprites[5];
        _emptySprite = sprites[8];

        if (_shiftMeter != null) {
            _meterMaterial = _shiftMeter.GetMeterFront().GetComponent<Image>().material;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (_shiftMeter != null) {
            //_meterMaterial.SetFloat("_Cutoff", (_playerController.ShiftCooldownTimer / _playerController.ShiftCooldownTime));

            if (_shiftMeter.meterBar != null) {
                tempFloat = (_playerController.ShiftCooldownTimer / _playerController.ShiftCooldownTime);
                // Avoid dividing by zero
                if (tempFloat == 0) {
                    tempFloat = 0.001f;
                }
                _shiftMeter.meterBar.rectTransform.sizeDelta = new Vector2(_shiftMeter.meterBar.rectTransform.rect.width, 770 - 770 * tempFloat);
            }

            // TODO: Shouldn't be doing this every frame
            if(_playerController.ShiftCooldownTimer >= _playerController.ShiftCooldownTime) {
                _shiftMeterEnd.sprite = _fullSprite;
                _shiftMeter.Full();
            } else {
                _shiftMeterEnd.sprite = _emptySprite;
                _shiftMeter.Empty();
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
