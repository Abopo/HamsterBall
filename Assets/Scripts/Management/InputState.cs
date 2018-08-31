using UnityEngine;

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

    // Used within the class to keep track of joystick inputs
    bool upJustPressed;
    bool downJustPressed;
    bool leftJustPressed;
    bool rightJustPressed;

    public int controllerNum; // 0-1 keyboard, 2-5 joysticks

	static int joysticksTaken = 0;
	static int keyboardTaken = 0;

	public InputState(){
		timeStamp = -1.0f;
		timeDelta = -1.0f;
        up = new button();
        down = new button();
        left = new button ();
		right = new button ();
		jump = new button();
		swing = new button ();
		attack = new button ();
        shift = new button ();
	}

	public static void Reset() {
		joysticksTaken = 0;
		keyboardTaken = 0;
	}

    public static int GetValidJoystick() {
        int joystick = 0;
        string[] joystickNames = Input.GetJoystickNames();

        for (int i = 0; i < joystickNames.Length; ++i) {
            if (joystickNames[i] != "") {
                joystick = i + 2;
                break;
            }
        }

        return joystick;
    }

	void GetJoysticks() {
		//string[] joysticks = Input.GetJoystickNames ();
		//joystickNum = joysticks.Length;
	}

	public static int AssignController() {
		string[] joysticks = Input.GetJoystickNames ();
        int badJoysticks = 0;
        // remove disconnected joysticks
        foreach (string joystick in joysticks) {
            if(joystick == "") {
                badJoysticks++;
            }
        }
        if (joysticksTaken < joysticks.Length-badJoysticks) {
			return 3 + joysticksTaken++;
		} else if (keyboardTaken < 2) {
			return 1 + keyboardTaken++;
		}

		return -1;
	}

	public static InputState GetInput(InputState currInput){
		// Keyboard 1
		if(currInput.controllerNum == 1) {
            currInput.up.isDown = Input.GetButton("Up 1");
            currInput.up.isJustPressed = Input.GetButtonDown("Up 1");
            currInput.up.isJustReleased = Input.GetButtonUp("Up 1");
            currInput.down.isDown = Input.GetButton("Down 1");
            currInput.down.isJustPressed = Input.GetButtonDown("Down 1");
            currInput.down.isJustReleased = Input.GetButtonUp("Down 1");
            currInput.left.isDown = Input.GetButton ("Left 1");
			currInput.left.isJustPressed = Input.GetButtonDown ("Left 1");
			currInput.left.isJustReleased = Input.GetButtonUp ("Left 1");
			currInput.right.isDown = Input.GetButton ("Right 1");
			currInput.right.isJustPressed = Input.GetButtonDown ("Right 1");
			currInput.right.isJustReleased = Input.GetButtonUp ("Right 1");
			currInput.jump.isDown = Input.GetButton ("Jump 1");
			currInput.jump.isJustPressed = Input.GetButtonDown ("Jump 1");
			currInput.jump.isJustReleased = Input.GetButtonUp ("Jump 1");
			currInput.swing.isDown = Input.GetButton ("Bubble 1");
			currInput.swing.isJustPressed = Input.GetButtonDown ("Bubble 1");
			currInput.swing.isJustReleased = Input.GetButtonUp ("Bubble 1");
            currInput.attack.isDown = Input.GetButton("Attack 1");
            currInput.attack.isJustPressed = Input.GetButtonDown("Attack 1");
            currInput.attack.isJustReleased = Input.GetButtonUp("Attack 1");
            currInput.shift.isDown = Input.GetButton ("Shift 1");
			currInput.shift.isJustPressed = Input.GetButtonDown ("Shift 1");
			currInput.shift.isJustReleased = Input.GetButtonUp ("Shift 1");
		// keyboard 2
		} if(currInput.controllerNum == 2) {
            currInput.up.isDown = Input.GetButton("Up 2");
            currInput.up.isJustPressed = Input.GetButtonDown("Up 2");
            currInput.up.isJustReleased = Input.GetButtonUp("Up 2");
            currInput.down.isDown = Input.GetButton("Down 2");
            currInput.down.isJustPressed = Input.GetButtonDown("Down 2");
            currInput.down.isJustReleased = Input.GetButtonUp("Down 2");
            currInput.left.isDown = Input.GetButton ("Left 2");
			currInput.left.isJustPressed = Input.GetButtonDown ("Left 2");
			currInput.left.isJustReleased = Input.GetButtonUp ("Left 2");
			currInput.right.isDown = Input.GetButton ("Right 2");
			currInput.right.isJustPressed = Input.GetButtonDown ("Right 2");
			currInput.right.isJustReleased = Input.GetButtonUp ("Right 2");
			currInput.jump.isDown = Input.GetButton ("Jump 2");
			currInput.jump.isJustPressed = Input.GetButtonDown ("Jump 2");
			currInput.jump.isJustReleased = Input.GetButtonUp ("Jump 2");
			currInput.swing.isDown = Input.GetButton ("Bubble 2");
			currInput.swing.isJustPressed = Input.GetButtonDown ("Bubble 2");
			currInput.swing.isJustReleased = Input.GetButtonUp ("Bubble 2");
            currInput.attack.isDown = Input.GetButton("Attack 2");
            currInput.attack.isJustPressed = Input.GetButtonDown("Attack 2");
            currInput.attack.isJustReleased = Input.GetButtonUp("Attack 2");
            currInput.shift.isDown = Input.GetButton ("Shift 2");
			currInput.shift.isJustPressed = Input.GetButtonDown ("Shift 2");
			currInput.shift.isJustReleased = Input.GetButtonUp ("Shift 2");
			// Joystick 1
		} else if(currInput.controllerNum == 3) {
            // Get vertical input, turn it into button format.
            float vertStick = Input.GetAxis("Vertical 1");
            float vertPad = Input.GetAxis("D-Pad Y 1");
            if (vertStick < -0.1f || vertPad < -0.1f) {
                currInput = GetUp(currInput);
            } else if (vertStick > 0.1f || vertPad > 0.1f) {
                currInput = GetDown(currInput);
            } else {
                currInput = GetIdleVertical(currInput);
            }
            // Get horizontal input, turn it into button format.
            float horStick = Input.GetAxis("Horizontal 1");
            float horPad = Input.GetAxis("D-Pad X 1");
            if (horStick < -0.1f || horPad < -0.1f) {
				currInput = GetLeft(currInput);
			} else if(horStick > 0.1f || horPad > 0.1f) {
				currInput = GetRight(currInput);
			} else {
				currInput = GetIdleHorizontal(currInput);
			}

			currInput.jump.isDown = Input.GetButton ("Joystick Jump 1");
			currInput.jump.isJustPressed = Input.GetButtonDown ("Joystick Jump 1");
			currInput.jump.isJustReleased = Input.GetButtonUp ("Joystick Jump 1");
			currInput.swing.isDown = Input.GetButton ("Joystick Bubble 1");
			currInput.swing.isJustPressed = Input.GetButtonDown ("Joystick Bubble 1");
			currInput.swing.isJustReleased = Input.GetButtonUp ("Joystick Bubble 1");
            currInput.attack.isDown = Input.GetButton("Joystick Attack 1");
            currInput.attack.isJustPressed = Input.GetButtonDown("Joystick Attack 1");
            currInput.attack.isJustReleased = Input.GetButtonUp("Joystick Attack 1");
            currInput.shift.isDown = Input.GetButton ("Joystick Shift 1");
			currInput.shift.isJustPressed = Input.GetButtonDown ("Joystick Shift 1");
			currInput.shift.isJustReleased = Input.GetButtonUp ("Joystick Shift 1");
			// joystick 2
		} else if(currInput.controllerNum == 4) {
            // Get vertical input, turn it into button format.
            float vert = Input.GetAxis("Vertical 2");
            if (vert < -0.1f) {
                currInput = GetUp(currInput);
            } else if (vert > 0.1f) {
                currInput = GetDown(currInput);
            } else {
                currInput = GetIdleVertical(currInput);
            }
            // Get horizontal input, turn it into button format.
            float hor = Input.GetAxis("Horizontal 2");
			if(hor < -0.1f) {
				currInput = GetLeft(currInput);
			} else if(hor > 0.1f) {
				currInput = GetRight(currInput);
			} else {
				currInput = GetIdleHorizontal(currInput);
			}
			
			currInput.jump.isDown = Input.GetButton ("Joystick Jump 2");
			currInput.jump.isJustPressed = Input.GetButtonDown ("Joystick Jump 2");
			currInput.jump.isJustReleased = Input.GetButtonUp ("Joystick Jump 2");
			currInput.swing.isDown = Input.GetButton ("Joystick Bubble 2");
			currInput.swing.isJustPressed = Input.GetButtonDown ("Joystick Bubble 2");
			currInput.swing.isJustReleased = Input.GetButtonUp ("Joystick Bubble 2");
            currInput.attack.isDown = Input.GetButton("Joystick Attack 2");
            currInput.attack.isJustPressed = Input.GetButtonDown("Joystick Attack 2");
            currInput.attack.isJustReleased = Input.GetButtonUp("Joystick Attack 2");
            currInput.shift.isDown = Input.GetButton ("Joystick Shift 2");
			currInput.shift.isJustPressed = Input.GetButtonDown ("Joystick Shift 2");
			currInput.shift.isJustReleased = Input.GetButtonUp ("Joystick Shift 2");
		} else if(currInput.controllerNum == 5) {
            // Get vertical input, turn it into button format.
            float vert = Input.GetAxis("Vertical 3");
            if (vert < -0.1f) {
                currInput = GetUp(currInput);
            } else if (vert > 0.1f) {
                currInput = GetDown(currInput);
            } else {
                currInput = GetIdleVertical(currInput);
            }
            // Get horizontal input, turn it into button format.
            float hor = Input.GetAxis("Horizontal 3");
			if(hor < -0.1f) {
				currInput = GetLeft(currInput);
			} else if(hor > 0.1f) {
				currInput = GetRight(currInput);
			} else {
				currInput = GetIdleHorizontal(currInput);
			}
			
			currInput.jump.isDown = Input.GetButton ("Joystick Jump 3");
			currInput.jump.isJustPressed = Input.GetButtonDown ("Joystick Jump 3");
			currInput.jump.isJustReleased = Input.GetButtonUp ("Joystick Jump 3");
			currInput.swing.isDown = Input.GetButton ("Joystick Bubble 3");
			currInput.swing.isJustPressed = Input.GetButtonDown ("Joystick Bubble 3");
			currInput.swing.isJustReleased = Input.GetButtonUp ("Joystick Bubble 3");
            currInput.attack.isDown = Input.GetButton("Joystick Attack 3");
            currInput.attack.isJustPressed = Input.GetButtonDown("Joystick Attack 3");
            currInput.attack.isJustReleased = Input.GetButtonUp("Joystick Attack 3");
            currInput.shift.isDown = Input.GetButton ("Joystick Shift 3");
			currInput.shift.isJustPressed = Input.GetButtonDown ("Joystick Shift 3");
			currInput.shift.isJustReleased = Input.GetButtonUp ("Joystick Shift 3");
		} else if(currInput.controllerNum == 6) {
            // Get vertical input, turn it into button format.
            float vert = Input.GetAxis("Vertical 4");
            if (vert < -0.1f) {
                currInput = GetUp(currInput);
            } else if (vert > 0.1f) {
                currInput = GetDown(currInput);
            } else {
                currInput = GetIdleVertical(currInput);
            }
            // Get horizontal input, turn it into button format.
            float hor = Input.GetAxis("Horizontal 4");
			if(hor < -0.1f) {
				currInput = GetLeft(currInput);
			} else if(hor > 0.1f) {
				currInput = GetRight(currInput);
			} else {
				currInput = GetIdleHorizontal(currInput);
			}
			
			currInput.jump.isDown = Input.GetButton ("Joystick Jump 4");
			currInput.jump.isJustPressed = Input.GetButtonDown ("Joystick Jump 4");
			currInput.jump.isJustReleased = Input.GetButtonUp ("Joystick Jump 4");
			currInput.swing.isDown = Input.GetButton ("Joystick Bubble 4");
			currInput.swing.isJustPressed = Input.GetButtonDown ("Joystick Bubble 4");
			currInput.swing.isJustReleased = Input.GetButtonUp ("Joystick Bubble 4");
            currInput.attack.isDown = Input.GetButton("Joystick Attack 4");
            currInput.attack.isJustPressed = Input.GetButtonDown("Joystick Attack 4");
            currInput.attack.isJustReleased = Input.GetButtonUp("Joystick Attack 4");
            currInput.shift.isDown = Input.GetButton ("Joystick Shift 4");
			currInput.shift.isJustPressed = Input.GetButtonDown ("Joystick Shift 4");
			currInput.shift.isJustReleased = Input.GetButtonUp ("Joystick Shift 4");
		}

		return currInput;
	}

    public static bool GetAnyButtonDown(int controllerNum) {
        bool anyButtonDown = false;

        switch(controllerNum) {
            case 1:
            case 2:
                if (Input.GetButtonDown("Jump " + (controllerNum)) ||
                   Input.GetButtonDown("Bubble " + (controllerNum)) ||
                   Input.GetButtonDown("Attack " + (controllerNum)) ||
                   Input.GetButtonDown("Shift " + (controllerNum))) {
                    anyButtonDown = true;
                }
                break;
            case 3:
            case 4:
            case 5:
            case 6:
                if (Input.GetButtonDown("Joystick Jump " + (controllerNum - 2)) ||
                   Input.GetButtonDown("Joystick Bubble " + (controllerNum - 2)) ||
                   Input.GetButtonDown("Joystick Attack " + (controllerNum - 2)) ||
                   Input.GetButtonDown("Joystick Shift " + (controllerNum - 2))) {
                    anyButtonDown = true;
                }
                break;
        }

        return anyButtonDown;
    }

    static InputState GetUp(InputState currInput) {
        currInput.up.isDown = true;
        if (!currInput.upJustPressed) {
            currInput.up.isJustPressed = true;
            currInput.upJustPressed = true;
        } else {
            currInput.up.isJustPressed = false;
        }
        currInput.up.isJustReleased = false;

        if (!currInput.down.isJustReleased) {
            currInput.down.isJustReleased = true;
            currInput.downJustPressed = false;
        } else {
            currInput.down.isJustReleased = false;
        }

        return currInput;
    }

    static InputState GetDown(InputState currInput) {
        currInput.down.isDown = true;
        if (!currInput.downJustPressed) {
            currInput.down.isJustPressed = true;
            currInput.downJustPressed = true;
        } else {
            currInput.down.isJustPressed = false;
        }
        currInput.down.isJustReleased = false;

        if (!currInput.up.isJustReleased) {
            currInput.up.isJustReleased = true;
            currInput.upJustPressed = false;
        } else {
            currInput.up.isJustReleased = false;
        }

        return currInput;
    }

    static InputState GetIdleVertical(InputState currInput) {
        currInput.up.isDown = false;
        currInput.up.isJustPressed = false;
        if (!currInput.up.isJustReleased) {
            currInput.up.isJustReleased = true;
            currInput.upJustPressed = false;
        } else {
            currInput.up.isJustReleased = false;
        }

        currInput.down.isDown = false;
        currInput.down.isJustPressed = false;
        if (!currInput.down.isJustReleased) {
            currInput.down.isJustReleased = true;
            currInput.downJustPressed = false;
        } else {
            currInput.down.isJustReleased = false;
        }

        return currInput;
    }

    static InputState GetLeft(InputState currInput) {
		currInput.left.isDown = true;
		if(!currInput.leftJustPressed) {
			currInput.left.isJustPressed = true;
            currInput.leftJustPressed = true;
		} else {
			currInput.left.isJustPressed = false;
		}
		currInput.left.isJustReleased = false;

        currInput.right.isDown = false;
		if(!currInput.right.isJustReleased) {
			currInput.right.isJustReleased = true;
            currInput.rightJustPressed = false;
		} else {
			currInput.right.isJustReleased = false;
		}

        return currInput;
	}

	static InputState GetRight(InputState currInput) {
		currInput.right.isDown = true;
        if (!currInput.rightJustPressed) {
			currInput.right.isJustPressed = true;
            currInput.rightJustPressed = true;
		} else {
			currInput.right.isJustPressed = false;
		}
		currInput.right.isJustReleased = false;

        currInput.left.isDown = false;
		if(!currInput.left.isJustReleased) {
			currInput.left.isJustReleased = true;
            currInput.leftJustPressed = false;
		} else {
			currInput.left.isJustReleased = false;
		}

		return currInput;
	}

	static InputState GetIdleHorizontal(InputState currInput) {
		currInput.right.isDown = false;
		currInput.right.isJustPressed = false;
		if(!currInput.right.isJustReleased) {
			currInput.right.isJustReleased = true;
            currInput.rightJustPressed = false;
		} else {
			currInput.right.isJustReleased = false;
		}
		
		currInput.left.isDown = false;
		currInput.left.isJustPressed = false;
		if(!currInput.left.isJustReleased) {
			currInput.left.isJustReleased = true;
            currInput.leftJustPressed = false;
		} else {
			currInput.left.isJustReleased = false;
		}

		return currInput;
	}

    public static InputState ResetInput(InputState currInput) {
        currInput.up.isDown = false;
        currInput.up.isJustPressed = false;
        currInput.up.isJustReleased = false;
        currInput.down.isDown = false;
        currInput.down.isJustPressed = false;
        currInput.down.isJustReleased = false;
        currInput.left.isDown = false;
        currInput.left.isJustPressed = false;
        currInput.left.isJustReleased = false;
        currInput.right.isDown = false;
        currInput.right.isJustPressed = false;
        currInput.right.isJustReleased = false;
        currInput.jump.isDown = false;
        currInput.jump.isJustPressed = false;
        currInput.jump.isJustReleased = false;
        currInput.swing.isDown = false;
        currInput.swing.isJustPressed = false;
        currInput.swing.isJustReleased = false;
        currInput.attack.isDown = false;
        currInput.attack.isJustPressed = false;
        currInput.attack.isJustReleased = false;
        currInput.shift.isDown = false;
        currInput.shift.isJustPressed = false;
        currInput.shift.isJustReleased = false;

        return currInput;
    }
}
