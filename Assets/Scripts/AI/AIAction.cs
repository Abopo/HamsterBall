using UnityEngine;
using System.Collections.Generic;

// An Action is a combination of Wants that the AI will have.
// If a given Action is chosen, the AI will act according to that Action's Wants.
public class AIAction {
    public int vertWant;
    public int horWant;
    public Hamster hamsterWant;
    public Bubble bubbleWant;
    public Node nodeWant;
    public int weight;
    public bool requiresShift = false;
    public PlayerController opponent;

    // TODO: Add variables for switching

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
        }
    }

    // Determine this Action's weight
    public void DetermineWeight() {
        weight = 0;

        // TODO: if action is identical to current action, increase weight?

        // if we are chasing a hamster
        if (hamsterWant != null) {
            if (hamsterWant.type == HAMSTER_TYPES.RAINBOW) {
                weight += 10;
            }

            // Increase weight based on distance from player, the closer the better.
            weight += (int)(20 / ((Vector2)(hamsterWant.transform.position - _playerController.transform.position)).magnitude);

            // if the bubble is on the opponents board
            if (bubbleWant != null && bubbleWant.team != _playerController.team) {
                weight += OpponentBoardChecks(hamsterWant.type);
            } else {
                weight += MyBoardChecks(hamsterWant.type);
            }
            // if we already have a hamster
        } else if (_playerController.heldBubble != null) {
            // if the bubble is on the opponents board
            if (bubbleWant != null && bubbleWant.team != _playerController.team) {
                weight += OpponentBoardChecks(_playerController.heldBubble.type);
            } else {
                weight += MyBoardChecks(_playerController.heldBubble.type);
            }
        } else if(opponent != null) {
            weight += OpponentChecks();
        }
    }

    int MyBoardChecks(HAMSTER_TYPES type) {
        int addWeight = 0;

        // if we want a bubble on our board that matches the hamster we want/have.
        if (bubbleWant != null && bubbleWant.team == _playerController.team && (type == bubbleWant.type || type == HAMSTER_TYPES.RAINBOW || type == HAMSTER_TYPES.DEAD)) {
            addWeight += 10;

            // increase weight based on how many matches the bubble currently has
            addWeight += 5 * bubbleWant.numMatches;
            addWeight += bubbleWant.numMatches > 3 ? 30 : 0;

            // If this action requires a shift
            if (requiresShift) {
                // if we have already shifted
                if (_playerController.shifted) {
                    // attempt to only do shifted actions.
                    addWeight += 100;
                } else {
                    // and we have the ability to shift
                    // obviously if we can't switch, don't do this action
                    addWeight += _playerController.CanShift ? 5 : -100;
                }
            }

            // If any adjBubbles is a Dead bubble (try to match and drop it)
            foreach (Bubble b in bubbleWant.adjBubbles) {
                if (b != null && b.type == HAMSTER_TYPES.DEAD) {
                    addWeight += 20;
                }
            }

            // If the bubble is on the bottom most lines 
            if (bubbleWant.node > 100) {
                // and has matches
                // if it doesn't have matches, don't throw at it!
                addWeight += bubbleWant.numMatches >= 2 ? 50 : -80;
            }

            // Check for any potential drops from this match
            addWeight += DropChecks();

            // if we want a bubble on our board and we've want/have a bomb hamster
        } else if (bubbleWant != null && bubbleWant.team == _playerController.team && type == HAMSTER_TYPES.BOMB) {
            // We don't want to blow up matches, so reduce weight for each match
            addWeight += bubbleWant.numMatches * -20;

            // Increase weight for each adjacent bubble
            addWeight += nodeWant.AdjBubbles.Length * 20;

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

        int matchCount = 0;
        foreach(Bubble b in nodeWant.AdjBubbles) {
            if(b != null && type == b.type) {
                matchCount++;
            }
        }
        if (type == HAMSTER_TYPES.BOMB) {
            addWeight += 20 * matchCount;
        } else {
            // reduce weight based on any same colored bubbles adj to nodeWant;
            addWeight -= 20 * matchCount;
        }

        // if the type is different from the node's relevant adjBubbles
        bool nonMatched = false;
        // This check is a bit annoying, if *either* of the relevent adjBubbles
        // both *exist and match* the type, then it's bad
        if(nodeWant.AdjBubbles[0] != null) {
            if (type != nodeWant.AdjBubbles[0].type) {
                nonMatched = true;
            } else {
                nonMatched = false;
            }
        }
        if (nodeWant.AdjBubbles[1] != null) {
            if (type != nodeWant.AdjBubbles[1].type) {
                nonMatched = true;
            } else {
                nonMatched = false;
            }
        }

        if (nonMatched) {
            if (type == HAMSTER_TYPES.RAINBOW) {
                // don't give your opponent a rainbow, come on!
                addWeight -= 500;
            } else if (type == HAMSTER_TYPES.DEAD) {
                // increase weight based on how many matches the bubble currently has
                addWeight += (6 * bubbleWant.numMatches);
                addWeight += bubbleWant.numMatches > 5 ? 8 : 0;
                // If we're already shifted, highly consider throwing the dead hamster.
                addWeight += _playerController.shifted ? 30 : 0;
            } else {
                // increase weight based on how many matches the bubble currently has
                addWeight += 3 * bubbleWant.numMatches;
                addWeight += bubbleWant.numMatches > 5 ? 8 : 0;
            }
            // if it's the same type, probably don't throw at it!
        } else {
            addWeight -= 100;
        }

        // If this action requires a shift
        if (requiresShift) {
            // if we have already shifted
            if (_playerController.shifted) {
                // Then actions on this side are good to do. (should only shift back if it's an extra good idea)
                addWeight += 30;

                // aim for closer bubbles when throwing to opponents board because there is limited time.
                float distanceX = bubbleWant.transform.position.x - _playerController.transform.position.x;
                addWeight += (int)(60 / distanceX);
            } else {
                // and we have the ability to shift
                // obviously if we can't switch, don't do this action
                addWeight += _playerController.CanShift ? 5 : -100;
            }
        }

        // If the bubble is on the bottom most lines 
        addWeight += bubbleWant.node > 100 ? 30 : 0;
        // Add even more if the node is in the kill line
        addWeight += bubbleWant.node > 138 ? 100 : 0;

        return addWeight;
    }

    int OpponentChecks() {
        int addWeight = 0;

        // If the player hasn't been punched yet
        if(opponent.curState != PLAYER_STATE.HIT) {
            addWeight += 20;

            // If the opponent is holding a bubble, try to stop them before they throw it!
            if(opponent.heldBubble != null) {
                addWeight += 35;
            }
        }

        return addWeight;
    }

    int DropChecks() {
        int addWeight = 0;

        List<Bubble> bubbles = new List<Bubble>();
        foreach (Bubble b in bubbleWant.adjBubbles) {
            // Reset variables for next bubble
            foreach (Bubble lB in bubbles) {
                lB.checkedForAnchor = false;
                lB.foundAnchor = false;
                lB.checkedForMatches = false;
            }
            bubbles.Clear();

            // Make sure the bubbleWant's match list is up to date
            if(bubbleWant.matches.Count == 0) {
                bubbleWant.matches = bubbleWant.CheckMatches(bubbleWant.matches);
            }
            // If a bubble can't find an anchor without the matchedBubbles
            // matches aren't set up for inital bubbles
            if (b != null && !b.CheckForAnchor(bubbles, bubbleWant.matches)) {
                // Then add weight based on how many bubbles would be dropped
                int dropCount = DropCount(b, bubbleWant.matches);
                addWeight += 30 * dropCount;
                //Debug.Log("Potential Drop of: " + dropCount);
            }
        }

        return addWeight;
    }

    // Counts how many bubbles will be dropped along with the inBubble
    int DropCount(Bubble bub, List<Bubble> matchedBubbles) {
        int dropCount = 1;

        foreach(Bubble b in bub.adjBubbles) {
            if (b != null && !b.checkedForMatches && !matchedBubbles.Contains(b)) {
                dropCount++;
                b.checkedForMatches = true;
                dropCount += DropCount(b, matchedBubbles);
            }
        }

        return dropCount;
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
