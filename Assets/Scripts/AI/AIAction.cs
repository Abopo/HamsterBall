using UnityEngine;
using System.Collections.Generic;

// An Action is a combination of Wants that the AI will have.
// If a given Action is chosen, the AI will act according to that Action's Wants.
[System.Serializable]
public class AIAction {
    public int vertWant;
    public int horWant;
    public Hamster hamsterWant;
    public Bubble bubbleWant;
    public Node nodeWant;
    public int weight;
    public bool requiresShift = false;
    public PlayerController opponent;
    public WaterBubble waterBubble;

    PlayerController _playerController;

    public AIAction(PlayerController p) {
        _playerController = p;
    }
    public AIAction(PlayerController p, Hamster h, Bubble b, Node n) {
        _playerController = p;
        hamsterWant = h;
        bubbleWant = b;
        nodeWant = n;
    }
    public AIAction(PlayerController p, Hamster h, Bubble b, Node n, bool rs) {
        _playerController = p;
        hamsterWant = h;
        bubbleWant = b;
        nodeWant = n;
        requiresShift = rs;
    }

	public void Update () {
        // On some boards hamsters can walk between sides, so keep track of requiresShift
        if (hamsterWant != null) {
            if (hamsterWant.transform.position.x > 0) {
                if (_playerController.team == 0) {
                    requiresShift = true;
                } else {
                    requiresShift = false;
                }
            } else if (hamsterWant.transform.position.x < 0) {
                if (_playerController.team == 0) {
                    requiresShift = false;
                } else {
                    requiresShift = true;
                }
            }
        // also keep track of if an action requires a shift
        } else if(nodeWant != null) {
            // As long as the player and the node we want are on the same side of the stage, it doesn't require a shift
            if((nodeWant.transform.position.x > 0 && _playerController.transform.position.x > 0) || 
                (nodeWant.transform.position.x < 0 && _playerController.transform.position.x < 0)) {
                requiresShift = false;
            } else {
                requiresShift = true;
            }
        }
    }

    // Determine this Action's weight
    public void DetermineWeight() {
        weight = 0;

        // TODO: if action is identical to current action, increase weight?

        // if we are chasing a hamster
        if (hamsterWant != null) {
            if (hamsterWant.type == HAMSTER_TYPES.RAINBOW) {
                weight += 20;
            }

            // Increase weight based on distance from player, the closer the better.
            int distanceX = Mathf.CeilToInt(((Vector2)(hamsterWant.transform.position - _playerController.transform.position)).magnitude);

            // if the bubble is on the opponents board
            if (bubbleWant != null && bubbleWant.team != _playerController.team) {
                weight += 30 / distanceX;
                weight += OpponentBoardChecks(hamsterWant.type);
            } else {
                weight += 5 / distanceX;
                weight += MyBoardChecks(hamsterWant.type);
            }
        // if we already have a hamster
        } else if (_playerController != null && _playerController.heldBall != null) {
            // if the bubble is on the opponents board
            if (bubbleWant != null && bubbleWant.team != _playerController.team) {
                weight += OpponentBoardChecks(_playerController.heldBall.type);
            } else {
                weight += MyBoardChecks(_playerController.heldBall.type);
            }
        } else if(opponent != null) {
            weight += OpponentChecks();
        } else if(waterBubble != null) {
            weight += WaterBubbleChecks();
        }
    }

    int MyBoardChecks(HAMSTER_TYPES type) {
        int addWeight = 0;

        // if we want a bubble on our board that matches the hamster we want/have.
        if (bubbleWant != null && bubbleWant.team == _playerController.team && (type == bubbleWant.type || type == HAMSTER_TYPES.RAINBOW || type == HAMSTER_TYPES.SKULL)) {
            addWeight += 10;

            // increase weight based on how many matches the bubble currently has
            addWeight += 5 * bubbleWant.numMatches;
            addWeight += bubbleWant.numMatches > 3 ? 30 : 0;

            // If bubbleWant was thrown by our teammate and can match/combo
            if(bubbleWant.numMatches > 1 && bubbleWant.canCombo && bubbleWant.PlayerController != _playerController) {
                // Go for a team combo!
                addWeight += 50;
            }

            // If this action requires a shift
            if (requiresShift) {
                // if we have already shifted
                if (_playerController.shifted) {
                    // attempt to only do shifted actions.
                    addWeight -= 100;
                } else {
                    // and we have the ability to shift
                    // obviously if we can't switch, don't do this action
                    addWeight += _playerController.CanShift ? 5 : -100;
                }

                // If the hamster we want/have is DEAD, it's best to use it in a switch
                //addWeight += type == HAMSTER_TYPES.DEAD ? 20 : 0;

            // If this action doesn't require a shift and the hamster we want/have is DEAD
            } else if (type == HAMSTER_TYPES.SKULL) {
                // If we're on the opponent's side
                if (_playerController.shifted) {
                    // We wanna throw that dead hamster
                    addWeight += 30;
                } else {
                    // We don't want to throw dead hamsters at our board
                    addWeight -= 30;
                }
            }

            // If any adjBubbles is a Dead bubble (try to match and drop it)
            foreach (Bubble b in bubbleWant.adjBubbles) {
                if (b != null && b.type == HAMSTER_TYPES.SKULL) {
                    addWeight += 20;
                }
            }

            // If the bubble is on the bottom most lines 
            if (bubbleWant.node > 100) {
                // and has matches
                // if it doesn't have matches, don't throw at it!
                addWeight += bubbleWant.numMatches >= 2 ? 50 : -80;
            }

            // Add weight based on potential drops of the bubbleWant
            addWeight += bubbleWant.dropPotential * 30;

        // if we want a bubble on our board and we want/have a bomb hamster
        } else if (bubbleWant != null && bubbleWant.team == _playerController.team && type == HAMSTER_TYPES.BOMB) {
            // We don't want to blow up matches, so reduce weight for each match
            addWeight += bubbleWant.numMatches * -20;

            // Increase weight for each adjacent bubble
            foreach(Bubble b in nodeWant.AdjBubbles) {
                if(b != null) {
                    addWeight += 20;
                }
            }
        // if we want a bubble on our board that doesn't match the hamster we want/have.
        } else if (bubbleWant != null && bubbleWant.team == _playerController.team) {
            // if the bubble isn't near the bottom
            if (bubbleWant.node < 100) {
                // increase weight based on how many matches the bubble currently has
                // but by less because it doesn't match our hamster.
                addWeight += 2 * bubbleWant.numMatches;
                addWeight += bubbleWant.numMatches > 5 ? 5 : 0;
            } else {
                addWeight += -80;
            }
        }

        return addWeight;
    }

    int OpponentBoardChecks(HAMSTER_TYPES type) {
        int addWeight = 0;

        if (type == HAMSTER_TYPES.BOMB) {
            // Increase weight by how many bubbles we can blow up
            addWeight += 20 * nodeWant.AdjBubbles.Length;
        } else {
            int matchCount = 0;
            foreach (Bubble b in nodeWant.AdjBubbles) {
                if (b != null && type == b.type) {
                    matchCount++;
                }
            }

            // reduce weight based on any same colored bubbles adj to nodeWant;
            addWeight -= 50 * matchCount;
        }

        // if the type is different from the node's relevant adjBubbles
        bool nonMatched = false;
        // This check is a bit annoying, if *any* of the adjBubbles
        // both *exist and match* the type, then it's bad
        foreach(Bubble b in nodeWant.AdjBubbles) {
            if(b != null) {
                if(type != b.type) {
                    nonMatched = true;
                } else {
                    nonMatched = false;
                }
            }
        }

        if (nonMatched) {
            if (type == HAMSTER_TYPES.RAINBOW) {
                // don't give your opponent a rainbow, come on!
                addWeight -= 500;
            } else if (type == HAMSTER_TYPES.SKULL) {
                // increase weight based on how many matches the bubble currently has
                addWeight += (10 * bubbleWant.numMatches);
                addWeight += bubbleWant.numMatches > 5 ? 20 : 0;
                // If we're already shifted, highly consider throwing the dead hamster.
                addWeight += _playerController.shifted ? 50 : 0;
            } else {
                // increase weight based on how many matches the bubble currently has
                addWeight += 5 * bubbleWant.numMatches;
                addWeight += bubbleWant.numMatches > 5 ? 10 : 0;
            }
        // if it's the same type, probably don't throw at it!
        } else {
            addWeight -= 100;
        }

        // If this action doesn't require a shift
        if (!requiresShift) {
            // if we have already shifted
            if (_playerController.shifted) {
                // Then actions on this side are good to do. (should only shift back if it's an extra good idea)
                addWeight += 50;

                // aim for closer bubbles when throwing to opponents board because there is limited time.
                int distanceX = Mathf.CeilToInt(Mathf.Abs(bubbleWant.transform.position.x - _playerController.transform.position.x));
                addWeight += 60 / distanceX;
            } else {
                // and we have the ability to shift
                // obviously if we can't switch, don't do this action
                //addWeight += _playerController.CanShift ? 5 : -100;
            }
        // If it requires a shift and we haven't shifted
        } else if(!_playerController.shifted) {
            addWeight += _playerController.CanShift ? 25 : -100;
        }

        // If the bubble is on the bottom most lines 
        addWeight += nodeWant.number >= 100 ? 30 : 0;
        // Add even more if the node is in the kill line
        addWeight += nodeWant.number >= 138 ? 100 : 0;

        return addWeight;
    }

    int OpponentChecks() {
        int addWeight = 0;

        // If we are on their board
        if(_playerController.shifted == true) {
            // If the player hasn't been punched yet
            if (opponent.CurState != PLAYER_STATE.HIT) {
                addWeight += 20;

                // If the opponent is holding a bubble, try to stop them before they throw it!
                if (opponent.heldBall != null) {
                    addWeight += 50;
                }
            }

        // If they are on our board
        } else {
            // If the player hasn't been punched yet
            if (opponent.CurState != PLAYER_STATE.HIT) {
                addWeight += 75;

                // If the opponent is holding a bubble, try to stop them before they throw it!
                if (opponent.heldBall != null) {
                    addWeight += 50;
                }
            }
        }

        return addWeight;
    }

    int WaterBubbleChecks() {
        int addWeight = 0;

        // If the bubble has fully spawned and is below the center line
        if (!waterBubble.IsSpawning && waterBubble.transform.position.y < 0f) {

            // Water bubbles are dangerous, so it's not a bad idea to get rid of them even if it hasn't caught a hamster yet
            addWeight += 50;

            // If the water bubble has caught a bubble, definitely try to pop it
            if (waterBubble.CaughtBubble != null) {
                addWeight += 100;
            }
        }

        return addWeight;
    }

    // Use this for any cleanup neede when choosing a new action.
    public void CleanUp() {
        /*
        if (nodeWant != null) {
            nodeWant.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        */
    }
}
