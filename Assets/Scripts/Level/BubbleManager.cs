using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public struct BubbleInfo {
    public bool isSet;
    public HAMSTER_TYPES type;
    public bool isGravity;
    public bool isIce;
}

public class BubbleManager : MonoBehaviour {

    public int team; // -1 = no team, 0 = left team, 1 = right team
    public bool testMode;

    protected Transform _ceiling;

    // 10 rows
    // alternating between 12 and 11 columns
    // should specify top and bottom rows
    public List<Node> nodeList = new List<Node>();
    protected int _baseLineLength = 12; // The longest a line will be
    protected int _topLineLength = 12;
    public int TopLineLength {
        get { return _topLineLength; }
    }

    protected Vector3 _firstNodePos = new Vector3(-10.86f, 7.34f, 0f);

    protected bool _justAddedBubble = false;
    protected bool _justRemovedBubble = false;
    protected Transform _bubblesParent;
    protected Transform _nodesParent;
    protected float _nodeHeight = 0.73f; // The height of a single node (i.e. how far down lines move)
    protected int _bottomRowStart {
        get {
            return nodeList.Count - _topLineLength;
        }
    }

    public static BubbleInfo[] startingBubbleInfo = new BubbleInfo[125];

    protected static List<int> _nextLineBubbles = new List<int>();
    protected int _nextLineIndex = 0; // counts up as new lines are added
    protected bool _setupDone;

    protected System.Random _random;
    protected System.Random _bubbleSeed;
    protected System.Random _specialSeed;
    int _linesSinceSpecial = 3; // How many lines have been added since the last special spawn

    protected int _comboCount = -1;
    //int _scoreTotal = 0;
    public int matchCount = 0;
    public bool wonGame;
    protected bool _gameOver = false;
    int _roundResult;
    protected bool _gameEndingSequence = false;

    GameObject _bubbleObj;
    GameObject _nodeObj;

    protected Bubble[] _bubbles;
    public Bubble[] Bubbles {
        get { return _bubbles; }
    }

    protected Bubble _lastBubbleAdded;
    public Bubble LastBubbleAdded {
        get { return _lastBubbleAdded; }
        set { _lastBubbleAdded = value; }
    }

    protected BubbleEffects _bubbleEffects;
    public BubbleEffects BubbleEffects {
        get { return _bubbleEffects; }
    }

    public int NextLineIndex {
        get { return _nextLineIndex; }
    }

    public int ComboCount {
        get { return _comboCount; }
    }

    public bool SetupDone {
        get { return _setupDone; }
    }

    HamsterMeter _hamsterMeter;
    public HamsterMeter HamsterMeter {
        get { return _hamsterMeter; }
    }

    public UnityEvent boardChangedEvent;
    bool _boardIsStable = true;

    int linesToAdd = 0;

    Vector3 _initialPos;
    bool _isShaking;
    float _shakeTime = 0.1f;
    float _shakeTimer = 0f;

    PlayerController[] players;

    Coroutine _checkbubbleDropPotentials;
    Coroutine _checkNodesCanBeHit;

    DividerFlash _divider;

    ScoreManager _scoreManager;

    BubbleManager _enemyBubbleManager;
    GameManager _gameManager;
    PlayerManager _playerManager;
    LevelManager _levelManager;
    AudioSource _audioSource;
    AudioClip _addLineClip;

    protected virtual void Awake() {
        _setupDone = false;

        Time.timeScale = 1;

        _bubbleObj = Resources.Load<GameObject>("Prefabs/Level/Bubble");
        _nodeObj = Resources.Load<GameObject>("Prefabs/Level/Node");

        // Get the right hamster meter
        HamsterMeter[] hamMeters = FindObjectsOfType<HamsterMeter>();
        foreach (HamsterMeter hM in hamMeters) {
            if (hM.team == team) {
                _hamsterMeter = hM;
                break;
            }
        }

        // Get the right score manager
        ScoreManager[] scoreMans = FindObjectsOfType<ScoreManager>();
        foreach (ScoreManager sM in scoreMans) {
            if (sM.team == team) {
                _scoreManager = sM;
                break;
            }
        }

        _gameManager = FindObjectOfType<GameManager>();
        _levelManager = FindObjectOfType<LevelManager>();

        _bubblesParent = transform.GetChild(0);
        _nodesParent = transform.GetChild(1);

        _topLineLength = _baseLineLength;
        BuildStartingNodes();


        int totalBubbleCount = (_baseLineLength * 6) + ((_baseLineLength - 1) * 6);
        _bubbles = new Bubble[totalBubbleCount];

        int startingBubbleCount = (_baseLineLength * 2) + ((_baseLineLength - 1) * 2);

        // If we are networked
        if (PhotonNetwork.connectedAndReady) {
            // If we are the master client
            if (PhotonNetwork.isMasterClient) {
                // Go ahead and make the starting bubbles
                SpawnStartingBubblesInfo(startingBubbleCount);
                _justAddedBubble = true;
            }
        } else {
            SpawnStartingBubblesInfo(startingBubbleCount);
            _justAddedBubble = true;
        }

        boardChangedEvent.Invoke();

        // Get the next line of bubbles
        if (!PhotonNetwork.connectedAndReady || (PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient)) {
            _random = new System.Random((int)Time.realtimeSinceStartup);
            _bubbleSeed = new System.Random((int)Time.realtimeSinceStartup);
            _specialSeed = new System.Random((int)Time.realtimeSinceStartup + 1);
            SeedNextLineBubbles();
        }

        //ReadyHamsterMeter();
        _hamsterMeter.Initialize(_baseLineLength, this);

        testMode = _gameManager.testMode;

        _audioSource = GetComponent<AudioSource>();
        _addLineClip = Resources.Load<AudioClip>("Audio/SFX/Add_Line");

        _initialPos = transform.position;
    }

    // Use this for initialization
    protected virtual void Start() {
        _bubbleEffects = GetComponentInChildren<BubbleEffects>();

        _gameOver = false;

        // Get enemy bubble manager
        BubbleManager[] bManagers = FindObjectsOfType<BubbleManager>();
        foreach (BubbleManager bM in bManagers) {
            if (bM != this) {
                _enemyBubbleManager = bM;
                break;
            }
        }

        // Send RPC if we are networked
        if (PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient) {
            GetComponent<PhotonView>().RPC("SyncLineBubbles", PhotonTargets.Others, _nextLineBubbles.ToArray());
        }

        boardChangedEvent.AddListener(UpdateAllAdjBubbles);
        boardChangedEvent.AddListener(OnBoardChanged);

        if (_gameManager.gameMode >= GAME_MODE.SURVIVAL) {
            transform.gameObject.AddComponent<SurvivalManager>();
        }

        players = FindObjectsOfType<PlayerController>();

        // Get divider
        DividerFlash[] dividers = FindObjectsOfType<DividerFlash>();
        foreach (DividerFlash df in dividers) {
            if (df.team == team) {
                _divider = df;
                break;
            }
        }
    }

    protected void BuildStartingNodes() {
        int lineLength = _baseLineLength;
        float baseXOffset = (_baseLineLength / 2) * -0.77f;
        float xOffset = baseXOffset;
        Vector3 nodeSpawnPos;
        GameObject newNode;
        for(int i = 0; i < 11; ++i) {
            for(int j = 0; j < lineLength; ++j) {
                nodeSpawnPos = new Vector3((transform.position.x+xOffset) + (0.84f * j), (transform.position.y + 2.9f) - (_nodeHeight * i), -1);
                newNode = Instantiate(_nodeObj, nodeSpawnPos, Quaternion.identity) as GameObject;
                newNode.GetComponent<Node>().number = nodeList.Count;
                nodeList.Add(newNode.GetComponent<Node>());
                newNode.transform.parent = _nodesParent;
            }

            if(lineLength == _baseLineLength) {
                lineLength = _baseLineLength - 1;
                xOffset = baseXOffset + 0.42f;
            } else {
                lineLength = _baseLineLength;
                xOffset = baseXOffset;
            }
        }
    }

    public void SpawnStartingBubblesInfo(int numBubbles) {
        // If the starting bubbles have not been built yet
        if (!startingBubbleInfo[0].isSet) {
            int tempType = 0;
            int[] typeCounts = new int[7]; // keeps track of how many of a type has been spawned
            bool specialSpawned = false; // if we've made a special bubble or not, only 1 can be spawned initially
            List<Bubble> tempMatches = new List<Bubble>();
            List<int> okTypes = new List<int>();

            for (int i = 0; i < numBubbles; ++i) {
                // Create and initialize a new bubble
                GameObject bub = Instantiate(_bubbleObj, nodeList[i].nPosition, Quaternion.identity) as GameObject;
                Bubble bubble = bub.GetComponent<Bubble>();
                bubble.transform.parent = _bubblesParent;
                bubble.node = i;
                // Add the new bubble to necessary lists
                nodeList[i].bubble = bubble;
                _bubbles[i] = bubble;

                // Temporarily initialize new bubble as a Dead bubble to prevent inaccurate match calculations.
                InitBubble(bubble, (int)HAMSTER_TYPES.SKULL);

                // Update the adjacent bubbles
                UpdateAllAdjBubbles();

                // Reinitialize okTypes
                okTypes.Clear();
                for (int j = 0; j < (int)HAMSTER_TYPES.NUM_NORM_TYPES; ++j) {
                    okTypes.Add(j);
                }
                // Check adjacent bubbles for matches
                foreach (Bubble b in bubble.adjBubbles) {
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
                if (_gameManager.SpecialBallsOn && i <= _baseLineLength && !specialSpawned) {
                    // There's a chance to spawn a special bubble
                    int rand = Random.Range(0, 20);
                    if (rand == 0) {
                        startingBubbleInfo[i].type = HAMSTER_TYPES.SPECIAL;
                        specialSpawned = true;
                        _linesSinceSpecial = 0;
                        // Actually initialize the bubble with the correct type
                        InitBubble(bubble, (int)HAMSTER_TYPES.SPECIAL);
                    } else {
                        // Randomly choose type for the new bubble based on available types
                        tempType = Random.Range(0, okTypes.Count);
                        startingBubbleInfo[i].type = (HAMSTER_TYPES)okTypes[tempType];
                        typeCounts[okTypes[tempType]] += 1;
                        // Actually initialize the bubble with the correct type
                        InitBubble(bubble, okTypes[tempType]);
                    }
                } else {
                    // Randomly choose type for the new bubble based on available types
                    tempType = Random.Range(0, okTypes.Count);
                    startingBubbleInfo[i].type = (HAMSTER_TYPES)okTypes[tempType];
                    typeCounts[okTypes[tempType]] += 1;
                    // Actually initialize the bubble with the correct type
                    InitBubble(bubble, okTypes[tempType]);
                }
                startingBubbleInfo[i].isSet = true;

                // Reset for next check
                for (int j = 0; j < numBubbles; ++j) {
                    if (_bubbles[j] != null) {
                        _bubbles[j].checkedForMatches = false;
                    } else { // Break out at the first null bubble since there won't be any after
                        break;
                    }
                }
            }
        } else { // If this rounds starting bubbles have already been decided
            for (int i = 0; i < startingBubbleInfo.Length; ++i) {
                // If this node is not supposed to be empty
                if (startingBubbleInfo[i].isSet && startingBubbleInfo[i].type >= 0) {
                    // Create a new bubble
                    GameObject bub = Instantiate(_bubbleObj, nodeList[i].nPosition, Quaternion.identity) as GameObject;
                    Bubble bubble = bub.GetComponent<Bubble>();
                    bubble.transform.parent = _bubblesParent;
                    // Set it to the corresponding type of the decided starting bubbles
                    HAMSTER_TYPES type = startingBubbleInfo[i].type;

                    InitBubble(bubble, (int)type);
                    bubble.SetGravity(startingBubbleInfo[i].isGravity);
                    bubble.SetIce(startingBubbleInfo[i].isIce);
                    bubble.node = i;
                    nodeList[i].bubble = bubble;
                    _bubbles[i] = bubble;
                }
            }
        }

        // assign adj bubbles for each starting bubble and empty node
        UpdateAllAdjBubbles();

        // set starting bubbles matches count
        for (int i = 0; i < numBubbles; ++i) {
            if (_bubbles[i] != null) {
                _bubbles[i].matches = _bubbles[i].CheckMatches(_bubbles[i].matches);
                _bubbles[i].numMatches = _bubbles[i].matches.Count;

                // Reset for next check
                for (int j = 0; j < numBubbles; ++j) {
                    if (_bubbles[j] != null) {
                        _bubbles[j].checkedForMatches = false;
                    }
                }
            }
        }

        _setupDone = true;
    }

    void InitBubble(Bubble bubble, int Type) {
        //bubble.leftTeam = leftTeam;
        bubble.team = team;
        bubble.HomeBubbleManager = this;
        bubble.Initialize((HAMSTER_TYPES)Type);
        bubble.locked = true;
    }

    // Assign adjBubbles for each bubble and empty node
    protected void UpdateAllAdjBubbles() {
        for (int i = 0; i < nodeList.Count; ++i) {
            if (_bubbles[i] != null) {
                _bubbles[i].ClearAdjBubbles();
                AssignAdjBubbles(_bubbles[i], i);
            }
            if (nodeList[i].bubble == null) {
                nodeList[i].ClearAdjBubbles();
                AssignAdjBubbles(null, i);
            }
        }
    }

    public void AssignAdjBubbles(Bubble bubble, int node) {
        // The adjacent bubbles are different based on where the bubble is and what the line order of the board is

        int leftNodes = (_baseLineLength * 2) - 1;

        // If the top line is a big line
        if (_topLineLength == _baseLineLength) {
            // Corners
            if (node == 0) {
                GetMiddleRight(bubble, node);
                GetBottomRight(bubble, node);
            } else if (node == _topLineLength - 1) {
                GetMiddleLeft(bubble, node);
                GetBottomLeft(bubble, node);
            } else if (node == nodeList.Count - _topLineLength + 1 /*138*/) {
                GetTopLeft(bubble, node);
                GetTopRight(bubble, node);
                GetMiddleRight(bubble, node);
            } else if (node == nodeList.Count - 1 /*149*/) {
                GetTopLeft(bubble, node);
                GetTopRight(bubble, node);
                GetMiddleLeft(bubble, node);
            }
            // Outer Lefts
            else if (node == leftNodes || node == leftNodes * 2 || node == leftNodes * 3 || node == leftNodes * 4 || node == leftNodes * 5) {
                GetTopRight(bubble, node);
                GetMiddleRight(bubble, node);
                GetBottomRight(bubble, node);
                if (bubble != null) {
                    bubble.is13Edge = -1;
                }
            }
            // Outer Rights
            else if (node == leftNodes + _topLineLength - 1 || node == leftNodes * 2 + _topLineLength - 1 || node == leftNodes * 3 + _topLineLength - 1 || node == leftNodes * 4 + _topLineLength - 1 || node == leftNodes * 5 + _topLineLength - 1) {
                GetTopLeft(bubble, node);
                GetMiddleLeft(bubble, node);
                GetBottomLeft(bubble, node);
                if (bubble != null) {
                    bubble.is13Edge = 1;
                }
            }
            // Inner Lefts
            else if (node == _topLineLength || node == leftNodes + _topLineLength || node == leftNodes * 2 + _topLineLength || node == leftNodes * 3 + _topLineLength || node == leftNodes * 4 + _topLineLength) {
                GetTopLeft(bubble, node);
                GetTopRight(bubble, node);
                GetMiddleRight(bubble, node);
                GetBottomLeft(bubble, node);
                GetBottomRight(bubble, node);
            }
            // Inner Rights
            else if (node == leftNodes - 1 || node == leftNodes * 2 - 1 || node == leftNodes * 3 - 1 || node == leftNodes * 4 - 1 || node == leftNodes * 2 - 1) {
                GetTopLeft(bubble, node);
                GetTopRight(bubble, node);
                GetMiddleLeft(bubble, node);
                GetBottomLeft(bubble, node);
                GetBottomRight(bubble, node);
            }
            // All other nodes
            else {
                GetTopLeft(bubble, node);
                GetTopRight(bubble, node);
                GetMiddleLeft(bubble, node);
                GetMiddleRight(bubble, node);
                GetBottomLeft(bubble, node);
                GetBottomRight(bubble, node);
            }
        } else { // if top line is a small line.
            // Corners
            if (node == 0) {
                GetMiddleRight(bubble, node);
                GetBottomRight(bubble, node);
                GetBottomLeft(bubble, node);
            } else if (node == _topLineLength - 1) {
                GetMiddleLeft(bubble, node);
                GetBottomLeft(bubble, node);
                GetBottomRight(bubble, node);
            } else if (node == nodeList.Count - _topLineLength - 1 /*137*/) {
                GetTopRight(bubble, node);
                GetMiddleRight(bubble, node);
            } else if (node == nodeList.Count - 2 /*148*/) {
                GetTopLeft(bubble, node);
                GetMiddleLeft(bubble, node);
            }
            // Outer Lefts
            else if (node == _topLineLength || node == leftNodes + _topLineLength || node == leftNodes * 2 + _topLineLength || node == leftNodes * 3 + _topLineLength || node == leftNodes * 4 + _topLineLength) {
                GetTopRight(bubble, node);
                GetMiddleRight(bubble, node);
                GetBottomRight(bubble, node);
                if (bubble != null) {
                    bubble.is13Edge = -1;
                }
            }
            // Outer Rights
            else if (node == leftNodes - 1 || node == leftNodes * 2 - 1 || node == leftNodes * 3 - 1 || node == leftNodes * 4 - 1 || node == leftNodes * 2 - 1) {
                GetTopLeft(bubble, node);
                GetMiddleLeft(bubble, node);
                GetBottomLeft(bubble, node);
                if (bubble != null) {
                    bubble.is13Edge = 1;
                }
            }
            // Inner Lefts
            else if (node == leftNodes || node == leftNodes * 2 || node == leftNodes * 3 || node == leftNodes * 4 || node == leftNodes * 5) {
                GetTopLeft(bubble, node);
                GetTopRight(bubble, node);
                GetMiddleRight(bubble, node);
                GetBottomLeft(bubble, node);
                GetBottomRight(bubble, node);
            }
            // Inner Rights
            else if (node == leftNodes + _topLineLength - 1 || node == leftNodes * 2 + _topLineLength - 1 || node == leftNodes * 3 + _topLineLength - 1 || node == leftNodes * 4 + _topLineLength - 1 || node == leftNodes * 5 + _topLineLength - 1) {
                GetTopLeft(bubble, node);
                GetTopRight(bubble, node);
                GetMiddleLeft(bubble, node);
                GetBottomLeft(bubble, node);
                GetBottomRight(bubble, node);
            }
            // All other nodes
            else {
                GetTopLeft(bubble, node);
                GetTopRight(bubble, node);
                GetMiddleLeft(bubble, node);
                GetMiddleRight(bubble, node);
                GetBottomLeft(bubble, node);
                GetBottomRight(bubble, node);
            }
        }

        // Make sure it's not assigned to itself
        if (bubble != null) {
            for (int i = 0; i < 6; ++i) {
                if (bubble.adjBubbles[i] == bubble) {
                    bubble.adjBubbles[i] = null;
                }
            }
        }
    }

    // All these Get?? functions can be called with "bubble" as null
    // in order to fill in the corresponding node's adjBubbles instead.
    void GetTopLeft(Bubble bubble, int node) {
        if (node - _baseLineLength >= 0) {
            if (bubble != null) {
                bubble.adjBubbles[0] = _bubbles[node - _baseLineLength]; // top left
                bubble.adjNodes[0] = nodeList[node - _baseLineLength];
            } else {
                nodeList[node].AdjBubbles[0] = _bubbles[node - _baseLineLength];
            }
        }
    }
    void GetTopRight(Bubble bubble, int node) {
        if (node - (_baseLineLength - 1) >= 0) {
            if (bubble != null) {
                bubble.adjBubbles[1] = _bubbles[node - (_baseLineLength - 1)]; // top right
                bubble.adjNodes[1] = nodeList[node - (_baseLineLength - 1)]; // top right
            } else {
                nodeList[node].AdjBubbles[1] = _bubbles[node - (_baseLineLength - 1)];
            }
        }
    }
    void GetMiddleLeft(Bubble bubble, int node) {
        if (node - 1 >= 0) {
            if (bubble != null) {
                bubble.adjBubbles[5] = _bubbles[node - 1]; // middle left
                bubble.adjNodes[5] = nodeList[node - 1]; // middle left
            } else {
                nodeList[node].AdjBubbles[5] = _bubbles[node - 1];
            }
        }
    }
    void GetMiddleRight(Bubble bubble, int node) {
        if (node + 1 < nodeList.Count) {
            if (bubble != null) {
                bubble.adjBubbles[2] = _bubbles[node + 1]; // middle right
                bubble.adjNodes[2] = nodeList[node + 1]; // middle right
            } else {
                nodeList[node].AdjBubbles[2] = _bubbles[node + 1];
            }
        }
    }
    void GetBottomLeft(Bubble bubble, int node) {
        if (node + (_baseLineLength - 1) < nodeList.Count) {
            if (bubble != null) {
                bubble.adjBubbles[4] = _bubbles[node + (_baseLineLength - 1)]; // bottom left
                bubble.adjNodes[4] = nodeList[node + (_baseLineLength - 1)]; // bottom left
            } else {
                nodeList[node].AdjBubbles[4] = _bubbles[node + (_baseLineLength - 1)];
            }
        }
    }
    void GetBottomRight(Bubble bubble, int node) {
        if (node + _baseLineLength < nodeList.Count) {
            if (bubble != null) {
                bubble.adjBubbles[3] = _bubbles[node + _baseLineLength]; // bottom right
                bubble.adjNodes[3] = nodeList[node + _baseLineLength]; // bottom right
            } else {
                nodeList[node].AdjBubbles[3] = _bubbles[node + _baseLineLength];
            }
        }
    }

    // Update is called once per frame
    protected virtual void Update() {
        // Wait until the game starts to update
        if (!_levelManager.gameStarted) {
            return;
        }

        if (_gameEndingSequence) {
            // Wait until all the bubbles are petrified (or something weird happened and the sequence ended prematurely)
            foreach (Bubble bub in _bubbles) {
                // If a bubble is still petrifying
                if(bub != null && bub.petrifying) {
                    // Keep going
                    return;
                }

                // If a bubble still isn't petrified
                //if (bub != null && !bub.Petrified) {
                 //   return;
                //}
            }

            _gameEndingSequence = false;

            _levelManager.ActivateResultsScreen(team, _roundResult);
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            TryAddLine();
        }
        if(Input.GetKeyDown(KeyCode.P)) {
            boardChangedEvent.Invoke();
        }

        _boardIsStable = IsBoardStable();

        // If for some reason the bubble haven't been setup yet
        if (!startingBubbleInfo[0].isSet && !_setupDone) {
            int startingBubbleCount = (_baseLineLength * 2) + ((_baseLineLength - 1) * 2);
            SpawnStartingBubblesInfo(startingBubbleCount);
        }

        if ((_justAddedBubble || _justRemovedBubble) && _boardIsStable) {
            // Check for anchor points
            List<Bubble> anchorBubbles = new List<Bubble>();
            foreach (Bubble b in _bubbles) {
                if (b != null) {
                    b.checkedForAnchor = false;
                    b.foundAnchor = false;
                    b.checkedForMatches = false;
                }
            }
            foreach (Bubble b in _bubbles) {
                if (b != null) {
                    anchorBubbles.Clear();
                    b.CheckForAnchor(anchorBubbles);
                }
            }

            boardChangedEvent.Invoke();
            _justAddedBubble = false;
            _justRemovedBubble = false;
        }

        // If the board is waiting to add a line
        if (linesToAdd > 0 && !_gameOver) {
            // Shake the board
            StartShaking();

            // Once the board is stable
            if (_boardIsStable) {
                --linesToAdd;
                AddLine();
                StopShaking();
            }
        }

        if (_isShaking) {
            _shakeTimer += Time.deltaTime;
            if (_shakeTimer >= _shakeTime) {
                ShakeMovement();
                _shakeTimer = 0f;
            }
        }
    }

    private void LateUpdate() {
        if (!_gameOver && _boardIsStable && !_justAddedBubble && !_justRemovedBubble) {
            CheckWinConditions();
        }
    }

    public void AddBubble(Bubble newBubble) {
        int closestNode;
        closestNode = FindClosestNode(newBubble);

        if (closestNode == -1) {
            // something really wrong happened
            // if we ever get here, maybe add more backup nodes.
            Debug.Log("No node found for bubble, Destroying");
            Destroy(newBubble.gameObject);
            return;
        }

        newBubble.node = closestNode;
        newBubble.transform.position = nodeList[closestNode].nPosition;

        // assign adj bubbles
        AssignAdjBubbles(newBubble, closestNode);

        newBubble.transform.parent = _bubblesParent;

        // add to list
        _bubbles[newBubble.node] = newBubble;
        nodeList[newBubble.node].bubble = newBubble;

        UpdateAllAdjBubbles();

        _justAddedBubble = true;
    }

    public void AddBubble(Bubble newBubble, int node) {
        if (node == -1) {
            // something really wrong happened
            // if we ever get here, maybe add more backup nodes.
            Debug.Log("No node found for bubble, Destroying");
            Destroy(newBubble.gameObject);
            return;
        }

        newBubble.node = node;
        newBubble.transform.position = nodeList[node].nPosition;

        // assign adj bubbles
        AssignAdjBubbles(newBubble, node);

        newBubble.transform.parent = _bubblesParent;

        // add to list
        _bubbles[newBubble.node] = newBubble;
        nodeList[newBubble.node].bubble = newBubble;

        UpdateAllAdjBubbles();

        _justAddedBubble = true;
    }

    int FindClosestNode(Bubble bubble) {
        // TODO: Maybe make this function just sort all nodes by distance and go down the list until a free one is found.

        // find closest node
        int closestNode = -1;
        // Have the main node, and some backup nodes just in case the main node is taken.
        // Keep nodes in order from closest found to 3rd closest found.
        int node1 = -1, node2 = -1, node3 = -1;
        float dist1 = 1000000, dist2 = 2000000, dist3 = 3000000;
        float tempDist = 0;
        for (int i = 0; i < nodeList.Count; ++i) {
            tempDist = Vector2.Distance(nodeList[i].nPosition, bubble.transform.position);
            if (tempDist < dist1) {
                dist3 = dist2;
                dist2 = dist1;
                dist1 = tempDist;
                node3 = node2;
                node2 = node1;
                node1 = i;
            } else if (tempDist < dist2) {
                dist3 = dist2;
                dist2 = tempDist;
                node3 = node2;
                node2 = i;
            } else if (tempDist < dist3) {
                dist3 = tempDist;
                node3 = i;
            }
        }

        // Check if node1 is taken
        for (int i = 0; i < _bubbles.Length - 1; ++i) {
            if (_bubbles[i] == null) {
                continue;
            }
            if (_bubbles[i].node == node1) {
                // Already a bubble in that node, eliminate it
                node1 = -1;
            } else if (_bubbles[i].node == node2) {
                // Already a bubble in that node, eliminate it
                node2 = -1;
            } else if (_bubbles[i].node == node3) {
                // Already a bubble in that node, eliminate it
                node3 = -1;
            }
        }

        if (node1 != -1) {
            closestNode = node1;
        } else if (node2 != -1) {
            closestNode = node2;
        } else if (node3 != -1) {
            closestNode = node3;
        }

        return closestNode;
    }

    public void RemoveBubble(int node) {
        if (node == -1) {
            Debug.Log("Shiiit");
        }
        _bubbles[node] = null;
        nodeList[node].bubble = null;

        //boardChangedEvent.Invoke();
        _justRemovedBubble = true;
    }

    public void TryAddLine() {
        if (!_boardIsStable || _gameOver) {
            ++linesToAdd;
        } else {
            AddLine();
        }
    }

    public virtual void AddLine() {
        // We're gonna modify the nodesList so we should stop this coroutine in case it's mid-stuff
        if (_checkNodesCanBeHit != null) {
            StopCoroutine(_checkNodesCanBeHit);
        }
        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.CrowdMedium1);
        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.NewLine);

        // Since bottomRowStart is calculated using topLineLength, we can't change topLineLength ahead of these next few operations
        // If we do then everything gets offset improperly

        for (int i = 0; i < nodeList.Count; ++i) {
            if(nodeList[i] == null) {
                continue;
            }

            // Delete bottom line
            if (i >= _bottomRowStart) {
                Destroy(nodeList[i].gameObject);
            // Move nodes down
            } else {
                nodeList[i].transform.Translate(new Vector3(0.0f, -_nodeHeight, 0.0f));
                nodeList[i].number += _topLineLength;
            }
        }

        // Remove the deleted nodes from the nodeList
        nodeList.RemoveRange(_bottomRowStart, _topLineLength);

        // Swap top line length and set xOFfset for new nodes
        float xOffset;
        if (_topLineLength == _baseLineLength) {
            _topLineLength = _baseLineLength - 1;
            xOffset = (_baseLineLength / 2) * -0.77f + 0.42f;
        } else {
            _topLineLength = _baseLineLength;
            xOffset = (_baseLineLength / 2) * -0.77f;
        }

        // Create a new node line and put on the top of the nodelist
        Vector3 nodeSpawnPos;
        GameObject newNode;
        for (int j = _topLineLength-1; j >= 0; --j) {
            nodeSpawnPos = new Vector3((transform.position.x + xOffset) + (0.84f * j), (transform.position.y + 2.9f), -5);
            newNode = Instantiate(_nodeObj, nodeSpawnPos, Quaternion.identity) as GameObject;
            newNode.transform.parent = _nodesParent;
            newNode.transform.SetAsFirstSibling();
            newNode.GetComponent<Node>().number = j;
            nodeList.Insert(0, newNode.GetComponent<Node>());
        }

        // Move bubbles down one line
        Bubble[] tempBubbles = new Bubble[nodeList.Count];
        for (int i = 0; i < nodeList.Count; ++i) {
            if (i >= _bubbles.Length) {
                break;
            }
            tempBubbles[i] = _bubbles[i];
        }
        _bubbles = new Bubble[nodeList.Count];
        List<Bubble> testList = new List<Bubble>();
        foreach (Bubble b in tempBubbles) {
            if (b != null) {
                b.node += _topLineLength;
                b.transform.position = nodeList[b.node].nPosition;
                _bubbles[b.node] = b;
                testList.Add(b);
            }
        }

        // Spawn bubbles on top line
        for (int i = 0; i < _topLineLength; ++i, ++_nextLineIndex) {
            GameObject bub = Instantiate(_bubbleObj, nodeList[i].nPosition, Quaternion.identity) as GameObject;
            Bubble bubble = bub.GetComponent<Bubble>();
            bubble.transform.parent = _bubblesParent;

            // Init bubble using the types that were decided ahead of time
            InitBubble(bubble, _nextLineBubbles[_nextLineIndex]);
            bubble.node = i;
            nodeList[i].bubble = bubble;
            _bubbles[i] = bubble;
        }

        // If we're near the end of the generated line bubbles
        if (_nextLineIndex >= _nextLineBubbles.Count - _baseLineLength * 2 && (!PhotonNetwork.connectedAndReady || (PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient))) {
            // Generate some more!
            SeedNextLineBubbles();
            // Send RPC if we are networked
            if (PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient) {
                GetComponent<PhotonView>().RPC("SyncLineBubbles", PhotonTargets.Others, _nextLineBubbles);
            }
        }

        UpdateAllAdjBubbles();

        _justAddedBubble = true;

        // Play sound
        PlayAddLineClip();
    }

    protected void SeedNextLineBubbles() {
        int[] lineBubbles;
        int lineLength = _topLineLength == _baseLineLength ? _baseLineLength - 1 : _baseLineLength;
        for (int i = 0; i < 10; ++i) {
            lineBubbles = DecideNextLine(lineLength);
            foreach (int bub in lineBubbles) {
                _nextLineBubbles.Add(bub);
            }
            // swap to next line length
            lineLength = lineLength == _baseLineLength ? _baseLineLength - 1 : _baseLineLength;

            _linesSinceSpecial++;
        }
    }

    protected virtual int[] DecideNextLine(int lineLength) {
        int[] nextLineBubbles = new int[lineLength];
        int[] typeCounts = new int[7]; // Keeps track of how many of each color has been made
        int tempType = 0;
        bool special = false;

        for (int i = 0; i < lineLength; ++i) {
            // If we've added enough lines since the last special ball
            if (_gameManager.SpecialBallsOn && _linesSinceSpecial >= 3) {
                // Theres a chance to spawn another one
                if (_specialSeed.Next(0, 30 - 2 * _linesSinceSpecial) == 0) {
                    special = true;
                    _linesSinceSpecial = 0;
                }
            }

            if (special) {
                nextLineBubbles[i] = (int)HAMSTER_TYPES.SPECIAL;
                special = false;
            } else {
                // If the line already has too many of the type, try again
                // TODO: This could potentially be really slow, maybe optimize it sometime
                do {
                    tempType = _bubbleSeed.Next(0, (int)HAMSTER_TYPES.NUM_NORM_TYPES);
                } while (typeCounts[tempType] > 2);

                // Increase count of type
                typeCounts[tempType] += 1;

                // Save type for next line
                nextLineBubbles[i] = tempType;
            }
        }

        return nextLineBubbles;
    }

    // Used in single player puzzle boards. Pushes the board down one line, but covers the top line with a wall instead of bubbles.
    public void PushBoardDown() {
        int tempBottomRowStart = _bottomRowStart;
        for (int i = 0; i < nodeList.Count; ++i) {
            // Delete bottom line nodes
            if (i >= tempBottomRowStart && nodeList[i] != null) {
                Destroy(nodeList[i].gameObject);
            }
        }

        // Remove the deleted nodes from the nodeList
        nodeList.RemoveRange(tempBottomRowStart, (_topLineLength == _baseLineLength ? _baseLineLength-1 : _baseLineLength));

        // Move the entire bubble manager down one line
        transform.Translate(0f, -0.67f, 0f, Space.World);

        // Spawn/push down the ceiling
        if(_ceiling == null) {
            _ceiling = GameObject.FindGameObjectWithTag("Ceiling").transform;
        }
        _ceiling.Translate(0f, -0.67f, 0f, Space.World);

        UpdateAllAdjBubbles();

        // TODO: this might be unnecessary
        boardChangedEvent.Invoke();

        // Play sound
        PlayAddLineClip();
    }

    // TODO: move this to the level manager?
    public void CheckWinConditions() {
        // Check single player challenge goals
        if (_gameManager.isSinglePlayer) {
            switch (_gameManager.gameMode) {
                case GAME_MODE.SP_POINTS:
                    // As soon as the player reaches the required score, the game ends
                    if (_scoreManager.TotalScore >= _gameManager.goalCount) {
                        EndGame(1);

                        // TODO: Ranking is based on throw count

                    // Otherwise, if the player is out of throws
                    } else if (PlayerController.totalThrowCount >= _gameManager.conditionLimit && IsBoardStable()) {
                        // Make sure score manager is fully updated
                        _scoreManager.CombineScore();

                        // Check one more time if the player won the stage
                        if (_scoreManager.TotalScore >= _gameManager.goalCount) {
                            EndGame(1);
                        } else {
                            EndGame(-1);
                        }
                        return;
                    }

                    break;
                case GAME_MODE.SP_MATCH:
                    if (matchCount >= _gameManager.goalCount) {
                        EndGame(1);
                        return;
                    }

                    break;
                case GAME_MODE.SP_CLEAR:
                    if (IsBoardClear()) {
                        EndGame(1);
                        return;
                    }

                    break;
            }

            // If there is a timeLimit and we have passed it
            if (_gameManager.gameMode == GAME_MODE.SP_CLEAR && _gameManager.conditionLimit > 0 && _levelManager.LevelTimer >= _gameManager.conditionLimit) {
                // This is actually a loss so handle that
                EndGame(-1);
                return;
            }
        }

        // If there's a bubble on the bottom line
        if (CheckBottomLine()) {
            _gameEndingSequence = true;

            // Check for a tie
            if (_enemyBubbleManager != null && _enemyBubbleManager.CheckBottomLine()) {
                // It's a tie!
                EndGame(0);
            } else {
                // We lost :(
                EndGame(-1);
            }
        }
    }

    public bool CheckBottomLine() {
        bool bubbleOnBottomLine = false;

        for (int i = _bottomRowStart; i < _bubbles.Length; ++i) {
            if (_bubbles[i] != null) {
                bubbleOnBottomLine = true;

                // Game is over, begin petrification from bubble that hit bottom line
                StartCoroutine(_bubbles[i].Petrify());
            }
        }

        return bubbleOnBottomLine;
    }

    // TODO: Move this to the level manager?
    // result - -1 = lost; 0 = tied; 1 = won
    public void EndGame(int result) {
        _gameOver = true;
        _levelManager.GameEnd();
        _scoreManager.CombineScore();

        _roundResult = result;

        // Stop shaking if we are
        if(_isShaking) {
            StopShaking();
        }

        if (_roundResult == 0 || _roundResult == 1) {
            wonGame = true;
        } else {
            wonGame = false;
        }

        if (_enemyBubbleManager != null) {
            _enemyBubbleManager._gameOver = true;
            if (_roundResult == 0) {
                _enemyBubbleManager.wonGame = true;
            } else {
                _enemyBubbleManager.wonGame = !wonGame;
            }
        }

        // Clear data for next round
        ClearAllData();

        // Send the winning team and score to the game manager
        if (_roundResult == 1) {
            _gameManager.EndGame(team, _scoreManager.TotalScore);
        } else if (_roundResult == -1) {
            _gameManager.EndGame(team == 0 ? 1 : 0, _scoreManager.TotalScore);
        } else if (_roundResult == 0) {
            _gameManager.EndGame(-1, 0);
        }

        if(!_gameEndingSequence) {
            _levelManager.ActivateResultsScreen(team, _roundResult);
        }
    }

    void OnBoardChanged() {
        //Debug.Log("Board has changed");

        // If we have AI
        if (_gameManager.playerManager.AreAI) {
            // Update important bubble information
            if (_checkbubbleDropPotentials != null) {
                StopCoroutine(_checkbubbleDropPotentials);
            }
            _checkbubbleDropPotentials = StartCoroutine(CheckBubbleDropPotentials());

            if (_checkNodesCanBeHit != null) {
                StopCoroutine(_checkNodesCanBeHit);
            }
            _checkNodesCanBeHit = StartCoroutine(CheckNodesCanBeHit());
        }

        CheckSecondToLastRow();
    }

    // Drop potentials are used by the AI to determine how many bubbles will drop if a particular bubble is popped.
    IEnumerator CheckBubbleDropPotentials() {
        foreach (Bubble b in _bubbles) {
            if (b != null && b.CouldMaybeBeHit()) {
                b.CheckDropPotential();
                yield return null;
            }
        }

    }
    // Nodes will check if they can be hit by a player, which is used by the AI to determine which nodes to aim at.
    IEnumerator CheckNodesCanBeHit() {
        bool isRelevant = false;
        foreach (Node n in nodeList.ToArray()) {
            if (n != null) {
                isRelevant = n.CheckRelevancy();
                // We want to get to the relevant nodes quickly, so only yield when we get to one
                if (isRelevant) {
                    yield return null;
                }
            }
        }
    }

    void CheckSecondToLastRow() {
        int firstNode = nodeList.Count - (_baseLineLength * 3 - 2) + _baseLineLength - 1;
        foreach (Bubble b in _bubbles) {
            if (b != null && b.node >= firstNode) {
                _divider.StartFlashing();
                return;
            }
        }

        if (_divider.isFlashing) {
            _divider.StopFlashing();
        }
    }

    public void RefreshRainbowBubbles() {
        for (int i = 0; i < nodeList.Count; ++i) {
            if (_bubbles[i] != null && _bubbles[i].type == HAMSTER_TYPES.RAINBOW) {
                _bubbles[i].checkedForMatches = false;
            }
        }
    }

    bool IsBoardClear() {
        foreach (Bubble b in _bubbles) {
            if (b != null) {
                return false;
            }
        }

        return true;
    }

    public int GetNextLineBubble(int index) {
        if (index < _nextLineBubbles.Count) {
            return _nextLineBubbles[index];
        }

        return -1;
    }
    public void SetNextLineBubbles(int[] lineBubbles) {
        _nextLineBubbles.Clear();
        foreach (int bub in lineBubbles) {
            _nextLineBubbles.Add(bub);
        }
    }

    public void IncreaseScore(int incScore) {
        _scoreManager.IncreaseScore(incScore);
    }

    public void StartShaking() {
        if (!_isShaking && !_gameOver) {
            _isShaking = true;
            transform.Translate(-0.05f, 0f, 0f, Space.World);
        }
    }
    public void StopShaking() {
        _isShaking = false;
        transform.position = new Vector3(_initialPos.x, transform.position.y, transform.position.z);
    }
    void ShakeMovement() {
        if (transform.position.x < _initialPos.x) {
            transform.Translate(0.1f, 0f, 0f, Space.World);
        } else if (transform.position.x > _initialPos.x) {
            transform.Translate(-0.1f, 0f, 0f, Space.World);
        }
    }

    private void OnDestroy() {
    }

    public static void ClearAllData() {
        // Clear out starting bubbles to prepare for next round
        for (int i = 0; i < startingBubbleInfo.Length; ++i) {
            startingBubbleInfo[i].isSet = false;
        }

        _nextLineBubbles.Clear();
    }

    bool IsBoardStable() {
        if (AreThereBubblesMidAir() || AreThereBusyBubbles()) {
            return false;
        }

        return true;
    }

    bool AreThereBubblesMidAir() {
        if(players == null || players.Length == 0) {
            players = FindObjectsOfType<PlayerController>();
        }
        // TODO: If players (or AI) are created mid-match, this will be inaccurate
        foreach (PlayerController p in players) {
            // If the player is on our side
            if (p.team == team && !p.shifted || p.team != team && p.shifted) {
                // If the player's bubble was thrown but hasn't locked with the board yet
                if (p.heldBall != null && p.heldBall.wasThrown && !p.heldBall.locked) {
                    // It's still mid-throw
                    return true;
                }
            }
        }

        return false;
    }

    bool AreThereBusyBubbles() {
        foreach (Bubble b in _bubbles) {
            // If a bubble is popping or dropping
            if (b != null && (b.Popping || !b.locked)) {
                return true;
            }
        }

        return false;
    }

    protected void PlayAddLineClip() {
        _audioSource.clip = _addLineClip;
        _audioSource.Play();
    }
}
