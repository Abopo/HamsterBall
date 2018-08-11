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
}
