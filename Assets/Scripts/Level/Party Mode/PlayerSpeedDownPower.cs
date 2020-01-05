using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpeedDownPower : PowerUp {

    // Use this for initialization
    protected override void Start() {
        base.Start();

        _spriteRenderer.sprite = Resources.Load<Sprite>("Art/Effects/PowerUps/PLAYER_SPEED_DOWN");
        _powerText.SetText("SPEED DOWN");

        _activateTime = 10.0f;

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

        // Find opponent(s)
        PlayerController[] pCons = FindObjectsOfType<PlayerController>();
        // Decrease their speed
        foreach (PlayerController pCon in pCons) {
            if(pCon.team != _caughtPlayer.team) {
                pCon.speedModifier = 0.5f;
            }
        }
        //_caughtPlayer.speedModifier = 2.0f;
    }

    protected override void Deactivate() {
        //base.Deactivate();

        // Find opponent(s)
        PlayerController[] pCons = FindObjectsOfType<PlayerController>();
        // Decrease their speed
        foreach (PlayerController pCon in pCons) {
            if (pCon.team != _caughtPlayer.team) {
                pCon.speedModifier = 1.0f;
            }
        }

        //_caughtPlayer.speedModifier = 1.0f;

        Destroy(this.gameObject);
    }
}
