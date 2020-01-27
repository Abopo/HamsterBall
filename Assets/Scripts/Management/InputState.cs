using UnityEngine;
using Rewired;

public class InputState {
	
	public struct button {
		public bool isDown;
		public bool isJustPressed;
		public bool isJustReleased;
	}
	
	public float 	timeStamp;	// when in Network.time this state happened
	public float 	timeDelta;  // The delta time for this update.
    public button   up;
    public button   down;
    public button 	left;
	public button 	right;
	public button	jump;
	public button	swing;
    public button   attack;
    public button	shift;
    public button   submit;
    public button   cancel;
    public button   start;
    public button   pause;
    public button   select;

    // Used within the class to keep track of joystick inputs
    bool upJustPressed;
    bool downJustPressed;
    bool leftJustPressed;
    bool rightJustPressed;

    public int playerID = -1;
    Player _player;

	public InputState(){
		timeStamp = -1.0f;
		timeDelta = -1.0f;
        MakeButtons();
	}

    public InputState(int iD) {
        SetPlayerID(iD);
        timeStamp = -1.0f;
        timeDelta = -1.0f;
        MakeButtons();
    }

    void MakeButtons() {
        up = new button();
        down = new button();
        left = new button();
        right = new button();
        jump = new button();
        swing = new button();
        attack = new button();
        shift = new button();
        select = new button();
    }

    public void SetPlayerID(int iD) {
        playerID = iD;
        _player = ReInput.players.GetPlayer(playerID);
    }
    public void SetPlayer(Player player) {
        _player = player;
    }

    public void Map() {
        _player.controllers.maps.SetMapsEnabled(false, "Gameplay");
        _player.controllers.maps.SetMapsEnabled(true, "Menu");
    }

    public void GetInput() {
        if (_player != null) {
            up.isDown = _player.GetButton("MoveUp");
            up.isJustPressed = _player.GetButtonDown("MoveUp");
            up.isJustReleased = _player.GetButtonUp("MoveUp");
            down.isDown = _player.GetButton("MoveDown");
            down.isJustPressed = _player.GetButtonDown("MoveDown");
            down.isJustReleased = _player.GetButtonUp("MoveDown");
            right.isDown = _player.GetButton("MoveRight");
            right.isJustPressed = _player.GetButtonDown("MoveRight");
            right.isJustReleased = _player.GetButtonUp("MoveRight");
            left.isDown = _player.GetButton("MoveLeft");
            left.isJustPressed = _player.GetButtonDown("MoveLeft");
            left.isJustReleased = _player.GetButtonUp("MoveLeft");
            jump.isDown = _player.GetButton("Jump");
            jump.isJustPressed = _player.GetButtonDown("Jump");
            jump.isJustReleased = _player.GetButtonUp("Jump");
            swing.isDown = _player.GetButton("Swing");
            swing.isJustPressed = _player.GetButtonDown("Swing");
            swing.isJustReleased = _player.GetButtonUp("Swing");
            attack.isDown = _player.GetButton("Attack");
            attack.isJustPressed = _player.GetButtonDown("Attack");
            attack.isJustReleased = _player.GetButtonUp("Attack");
            shift.isDown = _player.GetButton("Shift");
            shift.isJustPressed = _player.GetButtonDown("Shift");
            shift.isJustReleased = _player.GetButtonUp("Shift");

            if(shift.isDown) {
                upJustPressed = true;
            }

            // Menu inputs
            submit.isDown = _player.GetButton("Submit");
            submit.isJustPressed = _player.GetButtonDown("Submit");
            submit.isJustReleased = _player.GetButtonUp("Submit");
            cancel.isDown = _player.GetButton("Cancel");
            cancel.isJustPressed = _player.GetButtonDown("Cancel");
            cancel.isJustReleased = _player.GetButtonUp("Cancel");
            start.isDown = _player.GetButton("Start");
            start.isJustPressed = _player.GetButtonDown("Start");
            start.isJustReleased = _player.GetButtonUp("Start");
            pause.isDown = _player.GetButton("Pause");
            pause.isJustPressed = _player.GetButtonDown("Pause");
            pause.isJustReleased = _player.GetButtonUp("Pause");
            select.isDown = _player.GetButton("Select");
            select.isJustPressed = _player.GetButtonDown("Select");
            select.isJustReleased = _player.GetButtonUp("Select");
        } else if(playerID >= 0) {
            _player = ReInput.players.GetPlayer(playerID);
        }
    }

    public bool AnyButtonPressed() {
        if(jump.isJustPressed || swing.isJustPressed ||
            shift.isJustPressed || attack.isJustPressed ||
            submit.isJustPressed || cancel.isJustPressed ||
            start.isJustPressed || pause.isJustPressed ||
            select.isJustPressed) {
            return true;
        }

        return false;
    }

    public static Player AnyButtonOnAnyControllerPressed() {
        foreach (Player player in ReInput.players.Players) {
            if (player.GetAnyButtonDown()) {
                return player;
            }
        }

        return null;
    }
    public static bool GetButtonOnAnyControllerPressed(string button) {
        foreach(Player player in ReInput.players.AllPlayers) {
            if(player.GetButtonDown(button)) {
                return true;
            }
        }

        return false;
    }
    public static bool GetButtonOnAnyController(string button) {
        foreach (Player player in ReInput.players.AllPlayers) {
            if (player.GetButton(button)) {
                return true;
            }
        }

        return false;
    }
}
