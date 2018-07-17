using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkUpPower : PowerUp {

	// Use this for initialization
    protected override void Start () {
        base.Start();

        _spriteRenderer.sprite = Resources.Load<Sprite>("Art/Effects/PowerUps/ATK_UP");
        _powerText.SetText("ATK UP");

        _activateTime = 10.0f;

        exitedPipe = false;
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();

        if(_isActive) {
            _activateTimer += Time.deltaTime;
            if(_activateTimer >= _activateTime) {
                Deactivate();
            }
        }
	}

    protected override void Activate() {
        base.Activate();

        _caughtPlayer.atkModifier = 2;
    }

    protected override void Deactivate() {
        //base.Deactivate();

        _caughtPlayer.atkModifier = 0;

        DestroyObject(this.gameObject);
    }
}
