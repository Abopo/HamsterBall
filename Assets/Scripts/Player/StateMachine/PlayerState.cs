using UnityEngine;
using System.Collections;

public enum PLAYER_STATE { IDLE=0, WALK, JUMP, FALL, BUBBLE, THROW, HIT, SHIFT, ATTACK, NUM_STATES };

public class PlayerState {
	protected int _direction;	// -1 for left, 1 for right

	protected PlayerController playerController;
	public PlayerController PlayerController {
		get {
			return playerController;
		}
	}

	// Use this for initialization
	public virtual void Initialize(PlayerController playerIn) {
		playerController = playerIn;

	}
    // Update is called once per frame
    public virtual void Update(){}
	// GetInput is used for all Input handling
	public virtual void CheckInput(InputState inputState){}
	// returns the PLAYER_STATE that represents this state
	public virtual PLAYER_STATE getStateType(){
		return PLAYER_STATE.NUM_STATES;
	}
	//	use this for destruction
	public virtual void End() {}
}
