using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonochromePower : PowerUp {

    HamsterScan _hamScan;

    // Use this for initialization
    protected override void Start() {
        base.Start();
        _hamScan = FindObjectOfType<HamsterScan>();

        _spriteRenderer.sprite = Resources.Load<Sprite>("Art/Effects/PowerUps/MONOCHROME");
        _powerText.SetText("MONOCHROME");

        _activateTime = 20.0f;

        exitedPipe = false;
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if (_isActive) {
            _activateTimer += Time.deltaTime;
            if (_activateTimer >= _activateTime) {
                Deactivate();
            }
        }
    }

    protected override void Activate() {
        base.Activate();

        // Find opposing team's hamsters and increase their speed
        if (_caughtPlayer.team == 0) {
            Monochrome(_hamScan.AllRightHamsters);
        } else if (_caughtPlayer.team == 1) {
            Monochrome(_hamScan.AllLeftHamsters);
        }
    }

    protected override void Deactivate() {
        //base.Deactivate();

        DestroyObject(this.gameObject);
    }

    void Monochrome(List<Hamster> hamsters) {
        int rType = Random.Range(0, (int)HAMSTER_TYPES.NUM_NORM_TYPES);
        foreach(Hamster ham in hamsters) {
            if (ham.isGravity) {
                ham.SetType(11, (HAMSTER_TYPES)rType);
            } else {
                ham.SetType(rType);
            }
        }
    }
}
