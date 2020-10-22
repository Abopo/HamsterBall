using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedBubbleManager : Photon.MonoBehaviour {
    public bool isBusy;

    BubbleManager _bubbleManager;
    GameManager _gameManager;

    GameObject _bubbleObj;

    int _playerLinesReady = 0;

    public List<Bubble> _nextLineBubbles = new List<Bubble>();

    int[] _bubbleSyncData;
    float _syncTime = 1.5f;
    float _syncTimer = 0f;

    bool _needToSync = false;

    private void Awake() {
        _bubbleManager = GetComponent<BubbleManager>();
        _gameManager = FindObjectOfType<GameManager>();

        _bubbleObj = Resources.Load<GameObject>("Prefabs/Level/Bubble");
    }

    // Use this for initialization
    void Start () {
        //_gameManager.gameOverEvent.AddListener(SendGameOverCheck);
        if (PhotonNetwork.isMasterClient) {
            _bubbleManager.boardChangedEvent.AddListener(SendBoardLayoutCheck);

            // Spawn a few lines worth of bubbles
            SpawnNextLineBubbles();
            SpawnNextLineBubbles();
            SpawnNextLineBubbles();
            SpawnNextLineBubbles();
            SpawnNextLineBubbles();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (PhotonNetwork.isMasterClient) {
            if (_gameManager.gameIsOver) {
                if (!_bubbleManager.GameOver) {
                    // Game is fully over so make sure other players are also ending
                    SendGameOverCheck();
                }
            } else if(_bubbleManager.BoardIsStable) {
                _syncTimer += Time.deltaTime;
                if(_syncTimer >= _syncTime) {
                    SendBoardLayoutCheck();
                    _syncTimer = 0f;
                }
            } else {
                _syncTimer = 0f;
            }
        }

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Q) && Input.GetKeyDown(KeyCode.F)) {
            // Drop some bubbles to test synching
            int count = 0;
            for(int i = _bubbleManager.Bubbles.Length-1; i > 0; --i) {
                if(_bubbleManager.Bubbles[i] != null) {
                    _bubbleManager.Bubbles[i].Drop();
                    count++;
                    if(count >= 10) {
                        break;
                    }
                }
            }
        }
        if (Input.GetKey(KeyCode.Q) && Input.GetKeyDown(KeyCode.G)) {
            // Drop some bubbles to test synching
            int count = 0;
            for (int i = _bubbleManager.Bubbles.Length-1; i > 0; --i) {
                if (_bubbleManager.Bubbles[i] != null) {
                    _bubbleManager.Bubbles[i].Pop();
                    count++;
                    if (count > 3) {
                        break;
                    }
                }
            }
        }
#endif

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
        } else {
        }
    }

    public void SpawnStartingNetworkBubbles(int numBubbles) {
        int tempType = 0;
        int[] typeCounts = new int[7]; // keeps track of how many of a type has been spawned
        bool specialSpawned = false; // if we've made a special bubble or not, only 1 can be spawned initially
        List<Bubble> tempMatches = new List<Bubble>();
        List<int> okTypes = new List<int>();

        int finalType = 0;

        if (!BubbleManager.startingBubbleInfo[0].isSet) {
            for (int i = 0; i < numBubbles; ++i) {
                // Create and initialize a new bubble (this bubble is temporary and only used to determine type)
                GameObject bub = Instantiate(_bubbleObj, _bubbleManager.nodeList[i].nPosition, Quaternion.identity) as GameObject;
                Bubble tempBubble = bub.GetComponent<Bubble>();
                tempBubble.transform.parent = _bubbleManager.BubblesParent;
                tempBubble.node = i;

                // v Pretty much everything from here down is JUST to determine what type to spawn the bubble as v

                // Temporarily initialize new bubble as a Dead bubble to prevent inaccurate match calculations.
                _bubbleManager.InitBubble(tempBubble, (int)HAMSTER_TYPES.SKULL);

                // Add the new bubble
                _bubbleManager.AddBubble(tempBubble, i);

                // Reinitialize okTypes
                okTypes.Clear();
                for (int j = 0; j < (int)HAMSTER_TYPES.NUM_NORM_TYPES; ++j) {
                    okTypes.Add(j);
                }
                // Check adjacent bubbles for matches
                foreach (Bubble b in tempBubble.adjBubbles) {
                    if (b != null) {
                        // Get the matches for each adjacent bubble
                        tempMatches = b.CheckMatches(tempMatches);
                        // If a bubble has more that 3 matches already
                        if (tempMatches.Count > 2) {
                            // Don't make another bubble of that type,
                            // Remove that type from the okTypes list.
                            okTypes.Remove((int)b.type);
                        }

                        tempMatches.Clear();
                    }
                }

                int maxTypeCount = Mathf.FloorToInt(numBubbles / 7);
                // Also remove any types that have been spawned too many times
                for (int j = 0; j < 7; ++j) {
                    if (typeCounts[j] > maxTypeCount) {
                        okTypes.Remove(j);
                    }
                }

                // If we can spawn special balls and we are within the first line
                if (_gameManager.gameSettings.SpecialBallsOn && i <= _bubbleManager.BaseLineLength && !specialSpawned) {
                    // There's a chance to spawn a special bubble
                    int rand = Random.Range(0, 20);
                    if (rand == 0) {
                        BubbleManager.startingBubbleInfo[i].type = HAMSTER_TYPES.SPECIAL;
                        specialSpawned = true;
                        _bubbleManager.linesSinceSpecial = 0;

                        // Hold onto the type for the bubble
                        finalType = (int)HAMSTER_TYPES.SPECIAL;

                        // Actually initialize the bubble with the correct type
                        //_bubbleManager.InitBubble(tempBubble, (int)HAMSTER_TYPES.SPECIAL);
                    } else {
                        // Randomly choose type for the new bubble based on available types
                        tempType = Random.Range(0, okTypes.Count);
                        BubbleManager.startingBubbleInfo[i].type = (HAMSTER_TYPES)okTypes[tempType];

                        typeCounts[okTypes[tempType]] += 1;

                        // Hold onto the type for the bubble
                        finalType = okTypes[tempType];

                        // Actually initialize the bubble with the correct type
                        //_bubbleManager.InitBubble(bubble, okTypes[tempType]);
                    }
                } else {
                    // Randomly choose type for the new bubble based on available types
                    tempType = Random.Range(0, okTypes.Count);
                    BubbleManager.startingBubbleInfo[i].type = (HAMSTER_TYPES)okTypes[tempType];
                    typeCounts[okTypes[tempType]] += 1;

                    // Hold onto the type for the bubble
                    finalType = okTypes[tempType];

                    // Actually initialize the bubble with the correct type
                    _bubbleManager.InitBubble(tempBubble, okTypes[tempType]);
                }
                BubbleManager.startingBubbleInfo[i].isSet = true;

                // Reset for next check
                for (int j = 0; j < numBubbles; ++j) {
                    if (_bubbleManager.Bubbles[j] != null) {
                        _bubbleManager.Bubbles[j].checkedForMatches = false;
                    } else { // Break out at the first null bubble since there won't be any after
                        break;
                    }
                }

                // Remove bubble from bubble manager and destroy it
                _bubbleManager.RemoveBubble(i);
                Destroy(bub);

                // Finally actually instantiate the networked bubble
                InstantiateNetworkBubble(-1, _bubbleManager.nodeList[i].nPosition, finalType, _bubbleManager.team, i);
            }
        } else { // If this rounds starting bubbles have already been decided
            for (int i = 0; i < BubbleManager.startingBubbleInfo.Length; ++i) {
                // If this node is not supposed to be empty
                if (BubbleManager.startingBubbleInfo[i].isSet && BubbleManager.startingBubbleInfo[i].type >= 0) {
                    // We've already got all the info so just spawn the damn thing
                    InstantiateNetworkBubble(-1, _bubbleManager.nodeList[i].nPosition, (int)BubbleManager.startingBubbleInfo[i].type, _bubbleManager.team, i);
                }
            }
        }
    }

    public void InstantiateNetworkBubble(int id, Vector3 nodePos, int type, int team, int node) {
        object[] data = new object[5];
        data[0] = id; // determines how it spawned
        data[1] = type;
        data[2] = false; // will never be plasma at start
        data[3] = team;
        data[4] = node;

        PhotonNetwork.Instantiate("Prefabs/Networking/Bubble_PUN", nodePos, Quaternion.identity, 0, data);
    }

    void SendBoardLayoutCheck() {
        int[] bubblesPerNode = new int[_bubbleManager.nodeList.Count];
        int i = 0;
        foreach (Node n in _bubbleManager.nodeList) {
            if (n.bubble != null) {
                bubblesPerNode[i] = (int)n.number;
            } else {
                bubblesPerNode[i] = -1;
            }
            ++i;
        }

        photonView.RPC("BoardLayoutCheck", PhotonTargets.Others, bubblesPerNode);
    }

    [PunRPC]
    void SyncLineBubbles(int[] lineBubblesList) {
        _bubbleManager.SetNextLineBubbles(lineBubblesList);
        _bubbleManager.HamsterMeter.RefreshStockSprites();
    }

    public void StartNewLineProcess() {
        Debug.Log("Start new line process");

        _playerLinesReady = 0;

        if (PhotonNetwork.isMasterClient) {
            // Delay the bubble updates so line adding is smoother
            DelayBubbleUpdates();
        }

        // Send out the rpc to add a line before creating the network bubbles
        photonView.RPC("AddLine", PhotonTargets.AllViaServer);
    }

    [PunRPC]
    void AddLine() {
        Debug.Log("Try add a line");

        // Use bubble manager to move all the bubbles down to prepare for adding the new line
        _bubbleManager.TryAddLine();
    }

    public void AddLineBubbles() {
        Debug.Log("Line has been added");

        // Add in our line bubbles
        if(_nextLineBubbles.Count <= 0) {
            Debug.LogError("Uh-oh no line bubbles ready");
            if(PhotonNetwork.isMasterClient) {
                SpawnNextLineBubbles();
            }
        }

        int count = 0;
        for(int i = 0; i < _bubbleManager.TopLineLength; ++i) {
            // Reactivate the bubble so it is visible
            _nextLineBubbles[i].gameObject.SetActive(true);

            // Make sure it's the right type
            _nextLineBubbles[i].SetType((int)_nextLineBubbles[i].type);

            // Add it to our bubble manager
            _nextLineBubbles[i].HomeBubbleManager.AddBubble(_nextLineBubbles[i], i);

            count++;
        }

        // Clear the line bubble list
        _nextLineBubbles.RemoveRange(0, count);

        // Tell the master client we've added our line
        photonView.RPC("PlayerLineAdded", PhotonTargets.MasterClient);

        // We've finished adding the line, so it should be safe to set the board to stable
        isBusy = false;
    }

    void DelayBubbleUpdates() {
        // Delay our own board sync
        _syncTimer = -3f;

        // As well as all the bubbles individual syncing
        foreach (Bubble bub in _bubbleManager.Bubbles) {
            if (bub != null) {
                bub.GetComponent<NetworkedBubble>().DelaySyncCheck(3);
            }
        }
    }

    [PunRPC]
    void PlayerLineAdded() {
        _playerLinesReady++;

        // Once all players are ready
        if(_playerLinesReady >= PhotonNetwork.playerList.Length) {
            Debug.Log("All player line added, spawn next line bubbles");

            SpawnNextLineBubbles();
        }
    }

    public void SpawnNextLineBubbles() {
        // Figure out how long the line should be
        int lineLength = _bubbleManager.TopLineLength == _bubbleManager.BaseLineLength ? _bubbleManager.BaseLineLength - 1 : _bubbleManager.BaseLineLength;

        // Create the network bubbles
        for (int i = 0; i < lineLength; ++i, ++_bubbleManager.nextLineIndex) {
            // TODO: Got a an index out of range here
            InstantiateNetworkBubble(-2, _bubbleManager.nodeList[i].nPosition, _bubbleManager.NextLineBubbles[_bubbleManager.nextLineIndex], _bubbleManager.team, i);
        }

        isBusy = false;

        // Send rpc that we've added the bubbles
        photonView.RPC("NewLineProcessFinished", PhotonTargets.Others);
    }

    public void HoldLineBubble(Bubble lineBubble) {
        _nextLineBubbles.Add(lineBubble);
    }

    [PunRPC]
    void NewLineProcessFinished() {
        Debug.Log("New line finished");

        // We've finished adding the line, so it should be safe to set the board to stable
        isBusy = false;
    }

    // Checks to make sure that the board matches the master client's board.
    [PunRPC]
    void BoardLayoutCheck(int[] boardBubbles) {
        if(_bubbleManager.BoardIsStable && _bubbleManager.linesToAdd == 0 && !isBusy) {
            _bubbleSyncData = boardBubbles;
            SyncViaServerData();
        } else {
            Debug.Log("Board busy scync canceled.");
        }
        //_bubbleSyncData = boardBubbles;
        //_needToSync = true;
    }

    void SyncViaServerData() {
        Debug.Log("Syncing bubble manager");

        int i = 0;
        foreach (Node n in _bubbleManager.nodeList) {
            // If there shouldn't be a bubble where there currently is one
            if (_bubbleSyncData[i] == -1 && n.bubble != null) {
                Debug.LogError("Bubble exists at node #" + n.number + " that shouldn't, fixing...");

                // If the bubble this node thinks it has, is actually at a different node
                foreach(Bubble bub in _bubbleManager.Bubbles) {
                    if(bub != null && bub == n.bubble) {
                        if(bub.node != n.number) {
                            // just remove it from this node
                            n.bubble = null;

                            Debug.Log("Removed bubble from node #" + n.number);
                        }
                    }
                }

                // This is maybe too dangerous, maybe instead just move them or hide them
                // hide that bubble
                //n.bubble.gameObject.SetActive(false);
                //n.bubble.Pop();
            }

            // If there should be a bubble here but we don't have one
            if (_bubbleSyncData[i] != -1 && n.bubble == null) {
                Debug.LogError("Bubble missing, finding...");

                // Find the bubble, and move it back to position
                Bubble[] allBubbles = Resources.FindObjectsOfTypeAll(typeof(Bubble)) as Bubble[];
                foreach (Bubble bub in allBubbles) {
                    // If we find a matching bubble
                    if (bub.node == _bubbleSyncData[i] && bub.team == _bubbleManager.team) {
                        // we need to move this bubble back to it's position and make sure it's set up
                        bub.gameObject.SetActive(true);
                        bub.SetType((int)bub.type);
                        bub.toDestroy = false;
                        bub.popped = false;

                        bub.GetComponent<BubblePopAnimation>().Cancel();

                        _bubbleManager.AddBubble(bub, bub.node);
                    }
                }

                // Make sure to check for anchors after moving the bubbles
                _bubbleManager.boardChangedEvent.Invoke();

                //GameObject bub = Instantiate(_bubbleObj, n.nPosition, Quaternion.identity) as GameObject;
                //Bubble bubble = bub.GetComponent<Bubble>();
                //_bubbleManager.AddBubble(bubble, n.number);
            }
            /*
            // if the bubble here isn't the right type
            if (boardBubbles[i] != (int)n.bubble.type) {
                // Switch to the correct type
                Debug.LogError("Bubble type wrong, fixing...");
                n.bubble.SetType(boardBubbles[i]);
            }
            */

            ++i;
        }
    }

    void SendGameOverCheck() {
        // TODO: this won't handle draws
        int result = _bubbleManager.wonGame ? 1 : -1;
        photonView.RPC("GameOverCheck", PhotonTargets.Others, result);
    }

    public void SendLostMessage() {
        // Try and make sure the board is synched
        SendBoardLayoutCheck();

        photonView.RPC("WeDied", PhotonTargets.Others);
    }

    [PunRPC]
    void WeDied() {
        // We have lost the game, petrify the bubbles
        bool bottomLine = _bubbleManager.CheckBottomLine();
        // if for some reason we still don't have a bubble on the bottom line
        if(!bottomLine) {
            // just petrify something
            for(int i = _bubbleManager.Bubbles.Length-1; i > 0; --i) {
                if(_bubbleManager.Bubbles[i] != null) {
                    _bubbleManager.StartPetrifySequence(_bubbleManager.Bubbles[i]);
                    //StartCoroutine(_bubbleManager.Bubbles[i].Petrify());
                    break;
                }
            }
        }

        _bubbleManager.PetrifySequence = true;

        // TODO: handle ties
        // End the game
        _bubbleManager.EndGame(-1);
    }

    [PunRPC]
    void GameOverCheck(int result) {
        // The master client game has ended, so make sure we've also ended just in case

        // If the game is not over
        if (!_gameManager.gameIsOver) {
            Debug.LogError("Game is over but we haven't ended game. Ending...");
            // That's bad so end the game
            _bubbleManager.EndGame(result);
        }
    }
}
