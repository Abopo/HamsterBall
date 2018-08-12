using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour {

    PlayerController _playerController;

	// Use this for initialization
	void Start () {
        _playerController = GetComponentInParent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ThrowBall() {
        ThrowState throwState = (ThrowState)_playerController.GetPlayerState(PLAYER_STATE.THROW);
        throwState.Throw();
    }

    public void NetSwingOn() {
        BubbleState bubbleState = (BubbleState)_playerController.GetPlayerState(PLAYER_STATE.BUBBLE);
        bubbleState.Activate();
    }

    public void NetSwingOff() {
        BubbleState bubbleState = (BubbleState)_playerController.GetPlayerState(PLAYER_STATE.BUBBLE);
        bubbleState.Deactivate();
    }

    public void NetSwingFinished() {
        BubbleState bubbleState = (BubbleState)_playerController.GetPlayerState(PLAYER_STATE.BUBBLE);
        bubbleState.Finish();
    }
}
