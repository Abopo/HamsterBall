using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// This class will handle making the main decisions of the AI.
// It will create many possible actions, and then choose an action based on their weights.
public class AIBrain : MonoBehaviour {
    // For debugging purposes
    public int vertWant;
    public int horWant;
    public Hamster hamsterWant;
    public Bubble bubbleWant;
    public Node nodeWant;
    public int weight;
    public bool requiresShift = false;
    public PlayerController opponent;
    public CharacterAI characterAI;

    public LayerMask throwMask;

    PlayerController _playerController;
    HamsterScan _hamsterScan;
    AIBoardScan _boardScan;

    List<AIAction> _actions = new List<AIAction>();
    public AIAction curAction;

    int _difficulty = 3; // 0 - Easy, 1 - Medium, 2 - Hard, 3 - Expert

    float _decisionTime = 0.1f;
    float _decisionTimer = 0.0f;

    float _doubleCheckTime = 0.5f;
    float _doubleCheckTimer = 0f;
    int _turnCounter = 0;

    List<Hamster> _myHamsters = new List<Hamster>(); // List of Hamster types on the field.
    List<Hamster> _theirHamsters = new List<Hamster>(); // List of Hamster types on the field.

    List<PlayerController> _opponents = new List<PlayerController>(); // List of opponents in the game.

    public int Difficulty {
        get { return _difficulty; }
        set { _difficulty = value; }
    }

    // Use this for initialization
    void Start () {
        _playerController = GetComponent<PlayerController>();
        _playerController.aiControlled = true;

        _hamsterScan = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<HamsterScan>();
        _boardScan = GetComponent<AIBoardScan>();

        SetupDifficultySettings();
        GetOpponents();
    }

    void SetupDifficultySettings() {
        switch (_difficulty) {
            case 0:
                _decisionTime = 0.5f;
                break;
            case 1:
                _decisionTime = 0.3f;
                break;
            case 2:
                _decisionTime = 0.2f;
                break;
            case 3:
                _decisionTime = 0.1f;
                break;
        }
    }

    void GetOpponents() {
        GameObject[] opps = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject opp in opps) {
            if(opp.GetComponent<PlayerController>().team != _playerController.team) {
                _opponents.Add(opp.GetComponent<PlayerController>());
            }
        }
    }

    // Update is called once per frame
    void Update () {
        _decisionTimer += Time.deltaTime;
        if (_decisionTimer >= _decisionTime && _playerController.curState != PLAYER_STATE.THROW) {
            // Clear out old actions
            _actions.Clear();

            // Make some Actions
            MakeDecision();

            _decisionTimer = 0;
        }

        if (!ActionIsRelevant(curAction)) {
            ChooseNewAction();
        } else {
            curAction.Update();
        }

        if (curAction != null) {
            DecideVertWant();
            DecideHorWant();
                        
            DrawDebugInfo();
        } else {
            vertWant = -2;
            horWant = -2;
            hamsterWant = null;
            bubbleWant = null;
            nodeWant = null;
            weight = 0;
            requiresShift = false;
            opponent = null;
        }
    }

    public void MakeDecision() {
        // If the opponent is on our side and we aren't already holding a bubble
        if (_playerController.heldBubble == null) {
            foreach(PlayerController opp in _opponents) {
                if(opp.shifted && !_playerController.shifted && _playerController.curState != PLAYER_STATE.SHIFT) {
                    // Go after the opponent!
                    AIAction newAction = new AIAction(_playerController);
                    newAction.opponent = opp;
                    _actions.Add(newAction);
                }
            }
        }

        CreateHamsterAndNodeActions();

        // If we're in the beach level
        if(SceneManager.GetActiveScene().name.Contains("Beach")) {
            // Make some actions regarding water bubbles
            MakeWaterBubbleActions();
        }

        // Clear out bad actions
        _actions.RemoveAll(action => !ActionIsRelevant(action));

        //MakeSomeRandomActions();
        //_actions.RemoveAll(action => !ActionIsRelevant(action));

        // Determine the weights of the actions
        foreach (AIAction action in _actions) {
            if (curAction != null) {
                curAction.DetermineWeight();

                // If this AI has a characteristic
                if (characterAI != null) {
                    // Adjust the weight based on it
                    characterAI.AdjustActionWeight(curAction);
                }
            }

            action.DetermineWeight();
            if(characterAI != null) {
                characterAI.AdjustActionWeight(action);
            }
        }

        // If we've shifted, don't look for new actions unless it's necessary, since shift time is very short.
        if (_actions.Count > 0 && !(_playerController.shifted && curAction != null && curAction.requiresShift && ActionIsRelevant(curAction))) {
            ChooseAction();
        }
    }

    // This is a long series of fail-checks to make sure the AI is not attempting to do an impossible action
    bool ActionIsRelevant(AIAction action) {
        if (action == null) {
            return false;
        } else {
            // Checks based on there being a HamsterWant
            if (action.hamsterWant != null) {
                // If hamsterWant has already fallen out of bounds
                if (action.hamsterWant.Destroy1) {
                    return false;
                }

                // If the hamster we want is on our team, but the action says it requires a shift
                // TODO: This should never happen in the first place, look into action generation code.
                if (action.hamsterWant.team == _playerController.team && action.requiresShift) {
                    return false;
                }

                if(!action.hamsterWant.exitedPipe) {
                    return false;
                }
            }
            // If we are not chasing a hamster or opponent and are not holding a bubble
            if (action.hamsterWant == null && action.opponent == null && _playerController.heldBubble == null && action.waterBubble == null) {
                return false;
            }
            if (action.bubbleWant != null) {
                // If bubbleWant can't be hit
                //if (!action.bubbleWant.canBeHit) {
                //    return false;
                //}
            } else {
                // if we don't have a bubbleWant
                // but we are holding a bubble
                if(_playerController.heldBubble != null) {
                    return false;
                }
            }
            // If we are trying to chase an opponent but they have not shifted
            if (action.opponent != null && !action.opponent.GetComponent<PlayerController>().shifted) {
                return false;
            }
            // If this action requires a switch but we can't right now
            if(action.requiresShift && !_playerController.CanShift) {
                return false;
            }
            // If we have already shifted, but the action is back on our side of the field
            //if(!action.requiresShift && _playerController.shifted) {
            //    return false;
            //}
            // If nodeWant is in the bottom row of our board it'll kill us!
            if(action.nodeWant != null) {
                if (action.nodeWant.number > 137 && !action.requiresShift && action.bubbleWant.numMatches < 2) {
                    return false;
                }
                if(!action.nodeWant.isRelevant) {
                    return false;
                }
            }
            // The easiest AI won't ever shift
            if(_difficulty == 0 && action.requiresShift) {
                return false;
            }

            // check character specific AI
            if(characterAI != null) {
                return characterAI.ActionIsRelevant(action);
            }
        }

        return true;
    }

    void CreateHamsterAndNodeActions() {
        // Get the hamsters on our side of the field.
        if (_playerController.team == 0) {
            _myHamsters = _hamsterScan.AvailableLeftHamsters;
            _theirHamsters = _hamsterScan.AvailableRightHamsters;
        } else if (_playerController.team == 1) {
            _myHamsters = _hamsterScan.AvailableRightHamsters;
            _theirHamsters = _hamsterScan.AvailableLeftHamsters;
        }

        // If we don't have not caught a hamster yet.
        if (_playerController.heldBubble == null) {
            MakeNeedHamsterActions();

        // if we've already caught a hamster  
        } else {
            MakeHaveHamsterActions();
        }
    }

    void MakeNeedHamsterActions() {
        // Get the bubble with the most matches from the boardScan.
        foreach (Node n in _boardScan.AvailableNodes) {
            if (!_playerController.shifted) {
                foreach (Bubble b in n.AdjBubbles) {
                    if (b == null) {
                        continue;
                    }
                    // Check if there is a corresponding hamster of that type.
                    foreach (Hamster h in _myHamsters) {
                        // If there is, Want it.
                        AIAction newAction = new AIAction(_playerController, h, b, n, false);
                        _actions.Add(newAction);
                    }
                }
                // if we are able to shift or already shifted, also look at the opponents hamsters
            } else if (_playerController.CanShift || _playerController.shifted) {
                foreach (Bubble b in n.AdjBubbles) {
                    // Other side's hamsters
                    foreach (Hamster h in _theirHamsters) {
                        if ((b != null && b.type == h.type) || h.type == HAMSTER_TYPES.RAINBOW || h.type == HAMSTER_TYPES.DEAD) {
                            AIAction newAction = new AIAction(_playerController, h, b, n, _playerController.shifted ? false : true);
                            _actions.Add(newAction);
                        }
                    }
                }
            }
        }
        if (_playerController.CanShift || _playerController.shifted) {
            // These are actions that involving throwing hamster at the opponents board.
            int r = 0;
            bool requiresShift = false;
            foreach (Node n in _boardScan.OpponentNodes) {
                foreach (Bubble ob in n.AdjBubbles) {
                    if (ob == null) {
                        continue;
                    }
                    r = Random.Range(0, _hamsterScan.AvailableHamsters.Count - 1);
                    if (_hamsterScan.AvailableHamsters[r] != null) {
                        // Figure out if this action will require a shift
                        if(!_playerController.shifted) {
                            requiresShift = true;
                        } else if(_playerController.shifted && _hamsterScan.AvailableHamsters[r].team == 0) {
                            requiresShift = false;
                        }

                        AIAction newAction = new AIAction(_playerController, _hamsterScan.AvailableHamsters[r], ob, n, requiresShift);
                        _actions.Add(newAction);
                    }
                }
            }
        }
    }

    void MakeHaveHamsterActions() {
        foreach (Node n in _boardScan.AvailableNodes) {
            // Get the bubble with the most matches from the boardScan.
            foreach (Bubble b in n.AdjBubbles) {
                // Check if the types match with the bubble we've got
                if ((b != null /*&& b.type == _playerController.heldBubble.type) || _playerController.heldBubble.type == HAMSTER_TYPES.RAINBOW*/)) {
                    AIAction newAction = new AIAction(_playerController, null, b, n, _playerController.shifted ? true : false);
                    _actions.Add(newAction);
                }
            }
        }
        // If we can shift or have already shifted, make some actions involving the opponents board
        if (_playerController.CanShift || _playerController.shifted) {
            foreach (Node n in _boardScan.OpponentNodes) {
                foreach (Bubble ob in n.AdjBubbles) {
                    if (ob == null) {
                        continue;
                    }
                    if (ob.type != _playerController.heldBubble.type || _playerController.heldBubble.type == HAMSTER_TYPES.DEAD) {
                        AIAction newAction = new AIAction(_playerController, null, ob, n, _playerController.shifted ? false : true);
                        _actions.Add(newAction);
                    }
                }
            }
        }
    }

    void MakeSomeRandomActions() {
        if (_playerController.heldBubble == null) {
            if(_myHamsters.Count == 0) {
                return;
            }
            for (int i = 0; i < 5; ++i) {
                int rH = Random.Range(0, _myHamsters.Count);
                int rN = Random.Range(0, _boardScan.AvailableNodes.Count);
                if (_boardScan.AvailableNodes.Count > 0 && _boardScan.AvailableNodes[rN] != null && _boardScan.AvailableNodes[rN].AdjBubbles[0] != null) {
                    AIAction newAction = new AIAction(_playerController, _myHamsters[rH], _boardScan.AvailableNodes[rN].AdjBubbles[0], _boardScan.AvailableNodes[rN]);
                    _actions.Add(newAction);
                } else if (_boardScan.AvailableNodes.Count > 0 && _boardScan.AvailableNodes[rN] != null && _boardScan.AvailableNodes[rN].AdjBubbles[1] != null) {
                    AIAction newAction = new AIAction(_playerController, _myHamsters[rH], _boardScan.AvailableNodes[rN].AdjBubbles[1], _boardScan.AvailableNodes[rN]);
                    _actions.Add(newAction);
                }
            }
        } else {
            for (int i = 0; i < 5; ++i) {
                int rN = Random.Range(0, _boardScan.AvailableNodes.Count);
                if (_boardScan.AvailableNodes[rN] != null && _boardScan.AvailableNodes[rN].AdjBubbles[0] != null) {
                    AIAction newAction = new AIAction(_playerController, null, _boardScan.AvailableNodes[rN].AdjBubbles[0], _boardScan.AvailableNodes[rN]);
                    _actions.Add(newAction);
                } else if (_boardScan.AvailableNodes[rN] != null && _boardScan.AvailableNodes[rN].AdjBubbles[1] != null) {
                    AIAction newAction = new AIAction(_playerController, null, _boardScan.AvailableNodes[rN].AdjBubbles[1], _boardScan.AvailableNodes[rN]);
                    _actions.Add(newAction);
                }
            }
        }
    }

    void MakeWaterBubbleActions() {
        WaterBubble[] waterBubbles = FindObjectsOfType<WaterBubble>();
        foreach(WaterBubble wB in waterBubbles) {
            // If this water bubble is on our side
            if (wB.team == _playerController.team) {
                AIAction newAction = new AIAction(_playerController);
                newAction.waterBubble = wB;
                _actions.Add(newAction);
            }
        }
    }

    bool ActionAlreadyMade(AIAction action) {
        foreach(AIAction a in _actions) {
            if(a.bubbleWant != null && action.bubbleWant != null && a.bubbleWant == action.bubbleWant) {
                return true;
            }
        }

        return false;
    }

    void ChooseAction() {
        // Sort bubbles by node number (descending).
        _actions.Sort((x, y) => y.weight.CompareTo(x.weight));

        int choice;
        int offset = 0; // this slightly increases the chance of choosing each subsequent action.
        foreach (AIAction action in _actions) {
            choice = Random.Range(0, action.weight+offset);
            // this check will be against larger numbers as the AI get's less smart?
            switch (_difficulty) {
                case 0:
                    if (choice > action.weight / 0.9f && curAction != null && action.weight >= curAction.weight - 20) { // No chance to do best actions, chance to choose an action increases each loop.
                        curAction = action;
                        return;
                    }
                    offset += 2;
                    break;
                case 1:
                    if (choice > action.weight / 1.75f && curAction != null && action.weight >= curAction.weight-5) { // ~50% chance to choose this action.
                        curAction = action;
                        return;
                    }
                    offset += 3;
                    break;
                case 2:
                    // smart AI will only change actions if it is a BETTER idea.
                    if (choice > action.weight / 4f && curAction != null && action.weight > curAction.weight) { // 75% chance to choose this action.
                        curAction = action;
                        return;
                    }
                    break;
                case 3:
                    // smartest AI will only change actions if it is a MUCH better idea. (this saves time from switching back and forth between actions)
                    if (choice > action.weight / 10 && curAction != null && action.weight > curAction.weight+10) { // 90% chance to choose this action.
                        curAction = action;
                        return;
                    }
                    break;
            }
        }

        // If we get all the way through withough picking any actions, pick one at random
        if (curAction == null && _actions.Count > 0) {
            choice = Random.Range(0, _actions.Count);
            curAction = _actions[choice];
        }
    }

    public void ChooseNewAction() {
        if (curAction != null) {
            curAction.CleanUp();
            curAction = null;
        }

        // Clear out bad actions just in case
        _actions.RemoveAll(action => !ActionIsRelevant(action));

        ChooseAction();
    }

    void DecideVertWant() {
        if (_playerController.heldBubble == null) {
            if (curAction.hamsterWant != null) {
                VerticalChase(curAction.hamsterWant.gameObject);
            } else if (curAction.opponent != null) {
                VerticalChase(curAction.opponent.gameObject);
            } else if(curAction.waterBubble != null) {
                VerticalChase(curAction.waterBubble.gameObject);
            }
        } else if(_playerController.heldBubble != null && _playerController.shifted) {
            if (Mathf.Abs(_opponents[0].transform.position.y) - Mathf.Abs(transform.position.y) < -1) {
                curAction.vertWant = -1;
            } else if (Mathf.Abs(_opponents[0].transform.position.y) - Mathf.Abs(transform.position.y) > 1) {
                curAction.vertWant = 1;
            } else {
                curAction.vertWant = 0;
            }
        }
    }

    void DecideHorWant() {
        //foreach (AIAction action in _actions) {
        // If we don't have a bubble yet, and want a hamster
        if (_playerController.heldBubble == null) {
            if (curAction.hamsterWant != null) {
                HorizontalChase(curAction.hamsterWant.gameObject);
            } else if(curAction.opponent != null) {
                HorizontalChase(curAction.opponent.gameObject);
            } else if(curAction.waterBubble != null) {
                HorizontalChase(curAction.waterBubble.gameObject);
            }
        // If we've got a bubble and are looking to throw it
        } else if (_playerController.heldBubble != null && curAction.nodeWant != null) {
            // Move to a decent throwing spot
            float offset = 0;
            if (curAction.nodeWant.AdjBubbles[3] != null) {
                offset = 2.4f;
            } else if (curAction.nodeWant.AdjBubbles[4] != null) {
                offset = -2.4f;
            }
            if (curAction.nodeWant.transform.position.x - transform.position.x < (-2f + offset) || GetComponent<EntityPhysics>().IsTouchingWallRight) {
                if(curAction.horWant == 1) {
                    // We're turning around
                    _turnCounter++;
                }
                curAction.horWant = -1;
            } else if (curAction.nodeWant.transform.position.x - transform.position.x > (2f + offset) || GetComponent<EntityPhysics>().IsTouchingWallLeft) {
                if (curAction.horWant == -1) {
                    // We're turning around
                    _turnCounter++;
                }
                curAction.horWant = 1;
            } else {
                if(CheckForClearThrow()) {
                    // This timer makes the AI check for a clear throw more than once, from slightly different positions
                    // hopefully making throwing slightly more accurate.
                    curAction.horWant = 0;
                    _turnCounter = 0;
                }
            }

            // If we've turned around twice, something is wrong and we can't find the nodeWant
            if(_turnCounter >= 2) {
                // So choose a new action, but keep the same horWant
                int horWant = curAction.horWant;
                ChooseNewAction();
                curAction.horWant = horWant;
                _turnCounter = 0;
            }
        }
    }

    void VerticalChase(GameObject chaseObj) {
        if (Mathf.Abs(chaseObj.transform.position.y) - Mathf.Abs(transform.position.y) < -1) {
            curAction.vertWant = 1;
        } else if (Mathf.Abs(chaseObj.transform.position.y) - Mathf.Abs(transform.position.y) > 1) {
            curAction.vertWant = -1;
        } else {
            curAction.vertWant = 0;
        }
    }

    void HorizontalChase(GameObject chaseObj) {
        // if the hamster is to our left
        if (chaseObj.transform.position.x - transform.position.x < 0f) {
            curAction.horWant = -1;
            // if the hamster is to our right
        } else if (chaseObj.transform.position.x - transform.position.x > 0f) {
            curAction.horWant = 1;
            // if the hamster is close enough to grab
        } else {
            curAction.horWant = 0;
        }
    }

    bool CheckForClearThrow() {
        int layer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        curAction.nodeWant.gameObject.layer = LayerMask.NameToLayer("Node");

        Vector2 heldBubblePos;
        Vector2 nodeWantPos;
        Vector2 toNode;
        RaycastHit2D throwHit;
        int hitCount = 0;

        // TODO: Something is happening here where some of the rays won't hit ANYTHING somehow.
        for (int i = -1; i < 2; ++i) {
            heldBubblePos = new Vector3(_playerController.heldBubble.transform.position.x + 0.39f * i, 
                                        _playerController.heldBubble.transform.position.y);
            nodeWantPos = new Vector3(curAction.nodeWant.transform.position.x + 0.39f * i,
                                        curAction.nodeWant.transform.position.y);
            toNode = nodeWantPos - heldBubblePos;
            throwHit = Physics2D.Raycast(heldBubblePos, toNode, 15, throwMask); // 14 is bubble layer
            Debug.DrawRay(heldBubblePos, toNode * 10);
            // If our check hits a bubble and that bubble is our bubbleWant
            if (throwHit) {
                Node hitbubble = throwHit.collider.GetComponent<Node>();
                // If we hit THE node
                if (hitbubble != null) {
                    ++hitCount;
                    Debug.DrawLine(heldBubblePos, throwHit.point, Color.green);
                } else {
                    Debug.DrawLine(heldBubblePos, throwHit.point, Color.red);
                }
            }
        }

        gameObject.layer = layer;
        curAction.nodeWant.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        if (hitCount == 3) {
            _doubleCheckTimer += Time.deltaTime;
            if(_doubleCheckTimer > _doubleCheckTime) {
                // we're in a good spot to throw
                _doubleCheckTimer = 0;
                return true;
            }
        }

        return false;        
    }

    void DrawDebugInfo() {
        vertWant = curAction.vertWant;
        horWant = curAction.horWant;
        hamsterWant = curAction.hamsterWant;
        bubbleWant = curAction.bubbleWant;
        nodeWant = curAction.nodeWant;
        weight = curAction.weight;
        requiresShift = curAction.requiresShift;
        opponent = curAction.opponent;

        if (curAction.bubbleWant != null) {
            Vector3 end = new Vector3(curAction.bubbleWant.transform.position.x,
                                        curAction.bubbleWant.transform.position.y + 0.5f,
                                        curAction.bubbleWant.transform.position.z);
            Debug.DrawLine(curAction.bubbleWant.transform.position, end);
        }

        if(curAction.hamsterWant != null) {
            Vector3 end = new Vector3(curAction.hamsterWant.transform.position.x,
                                       curAction.hamsterWant.transform.position.y + 0.5f,
                                       curAction.hamsterWant.transform.position.z);
            Debug.DrawLine(curAction.hamsterWant.transform.position, end);
        }
    }
}
