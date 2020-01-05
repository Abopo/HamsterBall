using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinishiftPower : PowerUp {

    // Use this for initialization
    protected override void Start() {
        base.Start();

        _spriteRenderer.sprite = Resources.Load<Sprite>("Art/Effects/PowerUps/INFINISHIFT");
        _powerText.SetText("INFINISHIFT");

        _activateTime = 10.0f;

        exitedPipe = false;
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if (_isActive) {
            _caughtPlayer.ShiftCooldownTimer = _caughtPlayer.ShiftCooldownTime;

            _activateTimer += Time.deltaTime;
            if (_activateTimer >= _activateTime) {
                Deactivate();
            }
        }
    }

    protected override void Activate() {
        base.Activate();

        _caughtPlayer.ShiftCooldownTimer = _caughtPlayer.ShiftCooldownTime;
    }

    protected override void Deactivate() {
        //base.Deactivate();


        Destroy(this.gameObject);
    }
}
