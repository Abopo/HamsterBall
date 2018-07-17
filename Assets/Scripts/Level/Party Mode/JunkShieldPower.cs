using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunkShieldPower : PowerUp {

    HamsterMeter _hamMeter;

    // Use this for initialization
    protected override void Start() {
        base.Start();

        _spriteRenderer.sprite = Resources.Load<Sprite>("Art/Effects/PowerUps/JUNK_SHIELD");
        _powerText.SetText("JUNK SHIELD");

        _activateTime = 15.0f;

        exitedPipe = false;
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if (_isActive) {
            _activateTimer += Time.deltaTime;
            if (_activateTimer >= _activateTime || _hamMeter.shields == 0) {
                Deactivate();
            }
        }
    }

    protected override void Activate() {
        base.Activate();

        // Find hamster meter
        _hamMeter = _caughtPlayer.HomeBubbleManager.hamsterMeter;

        // Give it some shields
        _hamMeter.GainShields(6);
    }

    protected override void Deactivate() {
        //base.Deactivate();

        _hamMeter.LoseAllShields();

        DestroyObject(this.gameObject);
    }
}
