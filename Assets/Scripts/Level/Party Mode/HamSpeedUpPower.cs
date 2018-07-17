using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamSpeedUpPower : PowerUp {

    HamsterScan _hamScan;

    // Use this for initialization
    protected override void Start() {
        base.Start();
        _hamScan = FindObjectOfType<HamsterScan>();

        _spriteRenderer.sprite = Resources.Load<Sprite>("Art/Effects/PowerUps/HAM_SPEED_UP");
        _powerText.SetText("HAM SPD UP");

        _activateTime = 10.0f;

        exitedPipe = false;
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if (_isActive) {

            // TODO: This is kinda inefficient doing this every frame
            // Find opposing team's hamsters and increase their speed
            if (_caughtPlayer.team == 0) {
                IncreaseHamsterSpeed(_hamScan.AllRightHamsters);
            } else if (_caughtPlayer.team == 1) {
                IncreaseHamsterSpeed(_hamScan.AllLeftHamsters);
            }

            _activateTimer += Time.deltaTime;
            if (_activateTimer >= _activateTime) {
                Deactivate();
            }
        }
    }

    protected override void Activate() {
        base.Activate();

        // Find opposing team's hamsters and increase their speed
        HamsterScan hamScan = FindObjectOfType<HamsterScan>();
        if (_caughtPlayer.team == 0) {
            IncreaseHamsterSpeed(_hamScan.AllRightHamsters);
        } else if(_caughtPlayer.team == 1) {
            IncreaseHamsterSpeed(_hamScan.AllLeftHamsters);
        }
    }

    protected override void Deactivate() {
        //base.Deactivate();

        // Find opposing team's hamsters and return their speed to normal
        HamsterScan hamScan = FindObjectOfType<HamsterScan>();
        if (_caughtPlayer.team == 0) {
            DefaultHamsterSpeed(_hamScan.AllRightHamsters);
        } else if (_caughtPlayer.team == 1) {
            DefaultHamsterSpeed(_hamScan.AllLeftHamsters);
        }

        DestroyObject(this.gameObject);
    }

    void IncreaseHamsterSpeed(List<Hamster> hamsters) {
        foreach(Hamster ham in hamsters) {
            ham.moveSpeedModifier = 2;
        }
    }

    void DefaultHamsterSpeed(List<Hamster> hamsters) {
        foreach (Hamster ham in hamsters) {
            ham.moveSpeedModifier = 0;
        }
    }
}
