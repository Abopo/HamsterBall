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
    AIMapScan _mapScan;

    List<AIAction> _actions = new List<AIAction>();
    public AIAction curAction;

    int _difficulty = 3; // Scale from 1 (easy) to 10 (difficult)

    float _decisionTime = 0.1f;
    float _decisionTimer = 0.0f;

    float _doubleCheckTime = 0.5f;
    float _doubleCheckTimer = 0f;
    int _turnCounter = 0;

    List<Hamster> _myHamsters = new List<Hamster>(); // List of Hamster types on the field.
    List<Hamster> _theirHamsters = new List<Hamster>(); // List of Hamster types on the field.

    List<PlayerController> _opponents = new List<PlayerController>(); // List of opponents in the game.

    StopGoButton[] _buttons; // array of stopgo buttons (for city stage)

    GameManager _gameManager;

    public int Difficulty {
        get { return _difficulty; }
        set {
            _difficulty = value;
            if(_difficulty < 1) {
                _difficulty = 1;
            }
        }
    }

    private void Awake() {
        _playerController = GetComponent<PlayerController>();
        _hamsterScan = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<HamsterScan>();
        _boardScan = GetComponent<AIBoardScan>();
        _mapScan = GetComponent<AIMapScan>();

        _buttons = FindObjectsOfType<StopGoButton>();

        _gameManager = FindObjectOfType<GameManager>();
    }
    // Use this for initialization
    void Start () {
        _playerController.aiControlled = true;

        _playerController.significantEvent.AddListener(MakeDecision);

        GetOpponents();
        SetupDifficultySettings();
    }

    void SetupDifficultySettings() {
        _decisionTime = 0.5f - (0.045f * _difficulty);
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
        if (_decisionTimer >= _decisionTime && _playerController.CurState != PLAYER_STATE.THROW) {
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
        if (_playerController.heldBall == null) {
            foreach(PlayerController opp in _opponents) {
                if(opp.shifted && !_playerController.shifted && _playerController.CurState != PLAYER_STATE.SHIFT) {
                    // Go after the opponent!
                    AIAction newAction = new AIAction(_playerController);
                    newAction.opponent = opp;
                    _actions.Add(newAction);
                }
            }
        }

        CreateHamsterAndNodeActions();

        // If we're in the beach level
        if(_gameManager.selectedBoard == BOARDS.BEACH) {
            // Make some actions regarding water bubbles
            MakeWaterBubbleActions();
        } 

        // Clear out bad actions
        _actions.RemoveAll(action => !ActionIsRelevant(action));

        // Update curAction's weight to be accurate
        if (curAction != null) {
            curAction.DetermineWeight();

            // If this AI has a characteristic
            if (characterAI != null) {
                // Adjust the weight based on it
                characterAI.AdjustActionWeight(curAction);
            }
        }

        // Determine the weights of the actions
        foreach (AIAction action in _actions) {
            action.DetermineWeight();
            if(characterAI != null) {
                characterAI.AdjustActionWeight(action);
            }
        }

        // If we've shifted, don't look for new actions unless it's necessary, since shift time is very short.
        if (_actions.Count > 0 /*&& !(_playerController.shifted && curAction != null && ActionIsRelevant(curAction))*/) {
            ChooseAction();
        }

        // If we're in the city
        if(curAction != null && _gameManager.selectedBoard == BOARDS.CITY) {
            // We might need to hit the lever to get our hamster
            CheckCityLeverNeed();
        }
    }

    // This is a long series of fail-checks to make sure the AI is not attempting to do an impossible action
    bool ActionIsRelevant(AIAction action) {
        if (action == null) {
            return false;
        } else {
            // Checks based on there being a HamsterWant
            if (action.hamsterWant != null) {
                // If the hamster we want is on our team, but the action says it requires a shift
                // TODO: This should never happen in the first place, look into action generation code.
                if (action.hamsterWant.team == _playerController.team && action.requiresShift) {
                    return false;
                }

                if (action.bubbleWant != null) {
                    // If we have shifted, and the bubbleWant is on the opponent's board, but the hamsterWant is back on our side
                    if (_playerController.shifted && action.bubbleWant.team != _playerController.team && action.hamsterWant.team == _playerController.team) {
                        return false;
                    }
                }

                if(!action.hamsterWant.exitedPipe) {
                    return false;
                }
            }
            // If we are not chasing a hamster or opponent and are not holding a bubble
            if (action.hamsterWant == null && action.opponent == null && _playerController.heldBall == null && action.waterBubble == null) {
                return false;
            }

            // if we don't have a bubbleWant
            // but we are holding a bubble
            if(action.bubbleWant == null && _playerController.heldBall != null) {
                return false;
            }

            // If we are trying to chase an opponent but they are not on the same side
            if (action.opponent != null && action.opponent.GetComponent<PlayerController>().shifted == _playerController.shifted) {
                return false;
            }
            // If this action requires a switch but we can't right now
            if(action.requiresShift && !_playerController.CanShift) {
                return false;
            }

            if(action.nodeWant != null) {
                // If nodeWant is in the bottom row of our board it'll kill us!
                if (!_playerController.shifted) {
                    if (action.nodeWant.number > 137 && !action.requiresShift && (action.bubbleWant == null || action.bubbleWant.numMatches < 2)) {
                        return false;
                    }
                }

                if(!action.nodeWant.isRelevant) {
                    return false;
                }
            }

            // The easiest AI won't ever shift
            if(_difficulty < 3 && action.requiresShift) {
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
        if (_playerController.heldBall == null) {
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
            } 
        }

        if (_playerController.CanShift || _playerController.shifted) {
            // These are actions that involving throwing hamsters at the opponents board.
            int r = 0;
            bool requiresShift = false;
            foreach (Node n in _boardScan.OpponentNodes) {
                foreach (Bubble ob in n.AdjBubbles) {
                    if (ob == null) {
                        continue;
                    }
                    // Look at hamsters on opponents side
                    foreach(Hamster ham in _playerController.team == 0 ? _hamsterScan.AllRightHamsters : _hamsterScan.AllLeftHamsters) {
                        if(ham != null) {
                            // Figure out if this action will require a shift
                            if (!_playerController.shifted) {
                                requiresShift = true;
                            } else if (_playerController.shifted && _hamsterScan.AvailableHamsters[r].team == 0) {
                                requiresShift = false;
                            }

                            AIAction newAction = new AIAction(_playerController, ham, ob, n, requiresShift);
                            _actions.Add(newAction);
                        }
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
                    if (ob.type != _playerController.heldBall.type || _playerController.heldBall.type == HAMSTER_TYPES.SKULL) {
                        AIAction newAction = new AIAction(_playerController, null, ob, n, _playerController.shifted ? false : true);
                        _actions.Add(newAction);
                    }
                }
            }
        }
    }

    void MakeSomeRandomActions() {
        if (_playerController.heldBall == null) {
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

    void CheckCityLeverNeed() {
        // Start by resetting the otherWant
        curAction.otherWant = null;

        // if we don't want a hamster this is pointless
        if(curAction.hamsterWant == null) {
            return;
        }

        StopGoLever lever = FindObjectOfType<StopGoLever>();

        // If the hamster we want is stuck behind the stop gate
        if(curAction.hamsterWant.transform.position.y > -0.65f) {
            if(curAction.hamsterWant.transform.position.x < (6.4 * (_playerController.team == 0 ? -1 : 1)) && !lever.IsLeft) {
                // The hamster is stuck on the left side and we should hit the lever (which is on the right)
                // So we should want the right button
                curAction.otherWant = _buttons[0].transform;
            } else if(curAction.hamsterWant.transform.position.x > (6.4 * (_playerController.team == 0 ? -1 : 1)) && lever.IsLeft) {
                // The hamster is stuck on the right side and we should hit the lever
                // So we should want the left button
                curAction.otherWant = _buttons[1].transform;
            }

            if (curAction.otherWant != null) {
                // Whether or not this requires a shift depends on our position relative to the button
                if (Mathf.Sign(transform.position.x) != Mathf.Sign(curAction.otherWant.transform.position.x)) {
                    curAction.requiresShift = true;
                }
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

        // Remove best actions if difficulty is too low
        int removeWeight = 0;
        for (int i = _difficulty; i <= 3; ++i) {
            // Take the weigth of the smartest action
            removeWeight = _actions[0].weight;

            // Don't need to remove zero's (if we do we might end up with an empty list)
            if (removeWeight > 0) {
                // Remove all actions matching the weight
                _actions.RemoveAll(action => action.weight == removeWeight);
            }
        }

        int choice;
        int choiceOffset = 0; // this slightly increases the chance of choosing each subsequent action.
        int weightOffset = -17 + (_difficulty * 3); // this makes it easier/harder to change actions

        foreach (AIAction action in _actions) {
            choice = Random.Range(0, action.weight+choiceOffset);

            // The higher the difficulty, the higher chance to pick early options in the _actions list
            // and the harder it is to change the AI's mind about their current action
            if(choice > action.weight / _difficulty && curAction != null && action.weight >= curAction.weight - weightOffset) {
                //curAction = action;
                ChangeAction(action);
                return;
            }

            // Chance to choose an action increases each loop
            choiceOffset += 1 * _difficulty;
        }

        // If we get all the way through withough picking any actions, pick one at random
        if (curAction == null && _actions.Count > 0) {
            choice = Random.Range(0, _actions.Count);
            //curAction = _actions[choice];
            ChangeAction(_actions[choice]);
        }
    }

    void ChangeAction(AIAction action) {
        if (curAction != null) {
            // Keep the wants consistent until they can be reevaluated
            action.horWant = curAction.horWant;
            action.vertWant = curAction.vertWant;
        }
        curAction = action;
    }

    public void ChooseNewAction() {
        if (curAction != null) {
            curAction.CleanUp();
            curAction = null;
        }

        // Clear out bad actions just in case
        _actions.RemoveAll(action => !ActionIsRelevant(action));

        if (_actions.Count > 0) {
            ChooseAction();
        }
    }

    void DecideVertWant() {
        if (_playerController.heldBall == null) {
            if (curAction.hamsterWant != null) {
                if (curAction.otherWant != null) {
                    VerticalChase(curAction.otherWant.gameObject);
                } else {
                    VerticalChase(curAction.hamsterWant);
                }
            } else if (curAction.opponent != null) {
                VerticalChase(curAction.opponent.gameObject);
            } else if(curAction.waterBubble != null) {
                VerticalChase(curAction.waterBubble.gameObject);
            }
        } else if(_playerController.heldBall != null) {
            // If we're shifted
            if (_playerController.shifted) {
                // Try to keep away from the opponent
                if (Mathf.Abs(_opponents[0].transform.position.y) - Mathf.Abs(transform.position.y) < -1) {
                    curAction.vertWant = -1;
                } else if (Mathf.Abs(_opponents[0].transform.position.y) - Mathf.Abs(transform.position.y) > 1) {
                    curAction.vertWant = 1;
                } else {
                    curAction.vertWant = 0;
                }
            } else {
                // If we've got a ball, generally going up is better
                curAction.vertWant = 1;
            }
        }
    }

    void DecideHorWant() {
        //foreach (AIAction action in _actions) {
        // If we don't have a bubble yet, and want a hamster
        if (_playerController.heldBall == null) {
            if (curAction.hamsterWant != null) {
                if (curAction.otherWant != null) {
                    HorizontalChase(curAction.otherWant.gameObject);
                } else {
                    HorizontalChase(curAction.hamsterWant.gameObject);
                }
            } else if(curAction.opponent != null) {
                HorizontalChase(curAction.opponent.gameObject);
            } else if(curAction.waterBubble != null) {
                HorizontalChase(curAction.waterBubble.gameObject);
            }
        // If we've got a bubble and are looking to throw it
        } else if (_playerController.heldBall != null && curAction.nodeWant != null) {
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
        if (Mathf.Abs(chaseObj.transform.position.y) - Mathf.Abs(transform.position.y) < -0.5f) {
            curAction.vertWant = 1;
        } else if (Mathf.Abs(chaseObj.transform.position.y) - Mathf.Abs(transform.position.y) > 0.5f) {
            curAction.vertWant = -1;
        } else {
            curAction.vertWant = 0;
        }
    }
    void VerticalChase(Hamster hamster) {
        // If the hamster is out of the pipe and running in the stage
        if (hamster.exitedPipe) {
            if (Mathf.Abs(hamster.transform.position.y) - Mathf.Abs(transform.position.y) < -0.5f) {
                curAction.vertWant = 1;
            } else if (Mathf.Abs(hamster.transform.position.y) - Mathf.Abs(transform.position.y) > 0.5f) {
                curAction.vertWant = -1;
            } else {
                curAction.vertWant = 0;
            }
        // If the hamster is in the pipe
        } else {
            // Run to the pipe exit to meet the hamster as it comes out
            curAction.vertWant = 1;
        }
    }

    void HorizontalChase(GameObject chaseObj) {
        // if the hamster is to our left
        if (chaseObj.transform.position.x - transform.position.x < 0f) {
            // If there is lightning to our left and the lightning is closer than the obj
            if (_mapScan.LightningOnLeft && _mapScan.LightningDistLeft < Mathf.Abs(transform.position.x - chaseObj.transform.position.x)) {
                // Don't go left, maybe find new action
                ChooseNewAction();
            } else {
                curAction.horWant = -1;
            }
            // if the hamster is to our right
        } else if (chaseObj.transform.position.x - transform.position.x > 0f) {
            // If there is lightning to our right and the lightning is closer than the obj
            if (_mapScan.LightningOnRight && _mapScan.LightningDistRight < Mathf.Abs(transform.position.x - chaseObj.transform.position.x)) {
                // Don't go right, maybe find new action
                ChooseNewAction();
            } else {
                curAction.horWant = 1;
            }
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
            heldBubblePos = new Vector3(_playerController.heldBall.transform.position.x + 0.41f * i, 
                                        _playerController.heldBall.transform.position.y);
            nodeWantPos = new Vector3(curAction.nodeWant.transform.position.x + 0.32f * i,
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
        } else {
            _doubleCheckTimer = 0f;
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
