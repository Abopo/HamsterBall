using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoardEditor : MonoBehaviour {
    public InputField fileNameField;
    public InputField timeLimitField;
    public Dropdown gameMode;
    public InputField throwsField;

    public Text warningText;
    public GameObject hamsterSpriteObj;
    public GameObject levelObj;

    public Toggle rainbowToggle;
    public Toggle skullToggle;
    public Toggle bombToggle;


    List<EditorNode> _nodes = new List<EditorNode>();
    Rect _boundingRect;
    string _fileName;
    int _timeLimit = 0;

    int _numLines = 9;
    int _lineLength = 12;

    string _levelScene;

    GameManager _gameManager;

    // Use this for initialization
    void Start () {
        GameObject[] nodeObjs = GameObject.FindGameObjectsWithTag("Node1");
        int i = 0;
        foreach(GameObject nO in nodeObjs) {
            _nodes.Add(nO.GetComponent<EditorNode>());
            ++i;
        }

        // Sort nodes by node number (ascending).
        _nodes.Sort((a, b) => a.nodeNum.CompareTo(b.nodeNum));

        // Create the boundingRect
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        float x, y, width, height;
        x = boxCollider.bounds.min.x;
        y = boxCollider.bounds.min.y;
        width = boxCollider.bounds.max.x - x;
        height = boxCollider.bounds.max.y - y;
        _boundingRect = new Rect(x, y, width, height);

        _levelScene = "Laboratory - SinglePlayer";
        _fileName = "";

        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.prevMenu = MENU.EDITOR;
        if (_gameManager.prevLevel != "") {
            LoadBoard(_gameManager.prevLevel);
        }
    }

    // Update is called once per frame
    void Update () {

        /*
        Vector2 v1 = new Vector2(boundingRect.x, boundingRect.y);
        Vector2 v2 = new Vector2(boundingRect.x + boundingRect.width, boundingRect.y);
        Vector2 v3 = new Vector2(boundingRect.x, boundingRect.y + boundingRect.height);
        Vector2 v4 = new Vector2(boundingRect.x + boundingRect.width, boundingRect.y + boundingRect.height);
        Debug.DrawLine(v1, v2);
        Debug.DrawLine(v1, v3);
        Debug.DrawLine(v2, v4);
        Debug.DrawLine(v3, v4);
        */
    }

    public int FindClosestNode(Vector3 pos) {
        int closestNode = -1;

        float bestDist = 1000, tempDist = 0;
        for (int i = 0; i < _nodes.Count; ++i) {
            tempDist = Vector2.Distance(_nodes[i].transform.position, pos);

            if(tempDist < bestDist) {
                closestNode = i;
                bestDist = tempDist;
            }
        }

        return closestNode;
    }

    public int FindClosestFreeNode(Vector3 pos) {
        // find closest node
        int closestNode = -1;

        // Have the main node, and some backup nodes just in case the main node is taken.
        // Keep nodes in order from closest found to 3rd closest found.
        int node1 = -1, node2 = -1, node3 = -1;
        float dist1 = 1000000, dist2 = 2000000, dist3 = 3000000;
        float tempDist = 0;
        for (int i = 0; i < _nodes.Count; ++i) {
            tempDist = Vector2.Distance(_nodes[i].transform.position, pos);
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
        
        if (node1 != -1 && GetNode(node1).bubble == null) {
            closestNode = node1;
        } else if (node2 != -1 && GetNode(node2).bubble == null) {
            closestNode = node2;
        } else if (node3 != -1 && GetNode(node3).bubble == null) {
            closestNode = node3;
        }

        return closestNode;
    }

    public bool IsWithinBounds(Vector3 pos) {
        if(_boundingRect.Contains(pos)) {
            return true;
        }

        return false;
    }

    public void SaveBoard() {
        if(_fileName == "") {
            DisplayWarningText("No filename chosen.");
            return;
        }

#if UNITY_EDITOR
        string fullFileName = "Assets/Resources/Text/Created Boards/" + _fileName + ".txt";
#else
        string fullFileName = Application.dataPath + "/Created Boards/" + _fileName + ".txt";
#endif
        // Delete old content of file
        File.WriteAllText(fullFileName, "");
        
        FileStream file = File.Open(fullFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamWriter streamWriter = new StreamWriter(file);

        // Save the filename
        //streamWriter.WriteLine("Stage Name");
        streamWriter.WriteLine(_fileName);
        streamWriter.WriteLine("\n");

    #region Save Layout
        // Save the bubble layout
        string tempLine = "";
        tempLine = "Bubble Layout";
        streamWriter.WriteLine(tempLine);
        tempLine = "";

        HAMSTER_TYPES tempType;
        bool longLine = true;
        int lineLength = 0;
        int index = 0;
        for(int i = 0; i < _numLines; ++i) {
            if(longLine) {
                lineLength += _lineLength;
                longLine = false;
            } else {
                lineLength += _lineLength-1;
                longLine = true;
            }

            for(int j = index; j < lineLength; ++j, ++index) {
                if (_nodes[j].bubble != null) {
                    tempType = _nodes[j].bubble.Type;
                    if(_nodes[j].bubble.isGravity) {
                        tempLine += "G";
                    }
                    switch (tempType) {
                        case HAMSTER_TYPES.GREEN:
                            tempLine += "0";
                            break;
                        case HAMSTER_TYPES.RED:
                            tempLine += "1";
                            break;
                        case HAMSTER_TYPES.YELLOW:
                            tempLine += "2";
                            break;
                        case HAMSTER_TYPES.GRAY:
                            tempLine += "3";
                            break;
                        case HAMSTER_TYPES.BLUE:
                            tempLine += "4";
                            break;
                        case HAMSTER_TYPES.PINK:
                            tempLine += "5";
                            break;
                        case HAMSTER_TYPES.PURPLE:
                            tempLine += "6";
                            break;
                        case HAMSTER_TYPES.SKULL:
                            tempLine += "D";
                            break;
                        case HAMSTER_TYPES.RAINBOW:
                            tempLine += "R";
                            break;
                        case HAMSTER_TYPES.BOMB:
                            tempLine += "B";
                            break;
                    }
                    if(_nodes[j].bubble.isIce) {
                        tempLine += "I";
                    }
                } else {
                    tempLine += "N";
                }
            }

            tempLine += ",";
            streamWriter.WriteLine(tempLine);

            tempLine = "";
        }
#endregion

    #region Save data
        // End the bubble layout with an escape character
        tempLine = "E";
        streamWriter.WriteLine(tempLine);
        tempLine = "";
        streamWriter.WriteLine(tempLine);


        // Gotta write out all the data so the board can be tested

        // Handicaps
        /*
        tempLine = "Handicaps";
        streamWriter.WriteLine(tempLine);
        tempLine = "9";
        streamWriter.WriteLine(tempLine);
        tempLine = "9";
        streamWriter.WriteLine(tempLine);
        tempLine = "";
        streamWriter.WriteLine(tempLine);
        */

        // Spawn Rate
        tempLine = "HSM";
        streamWriter.WriteLine(tempLine);
        tempLine = "6";
        streamWriter.WriteLine(tempLine);
        tempLine = "";
        streamWriter.WriteLine(tempLine);

        //Special Hamsters
        tempLine = "Special Hamsters";
        streamWriter.WriteLine(tempLine);
        tempLine = "Rainbow";
        streamWriter.WriteLine(tempLine);
        tempLine = rainbowToggle.isOn ? "Y" : "N";
        streamWriter.WriteLine(tempLine);
        tempLine = "Dead";
        streamWriter.WriteLine(tempLine);
        tempLine = skullToggle.isOn ? "Y" : "N";
        streamWriter.WriteLine(tempLine);
        tempLine = "Bomb";
        streamWriter.WriteLine(tempLine);
        tempLine = bombToggle.isOn ? "Y" : "N";
        streamWriter.WriteLine(tempLine);
        tempLine = "Gravity";
        streamWriter.WriteLine(tempLine);
        tempLine = "N";
        streamWriter.WriteLine(tempLine);
        tempLine = "End";
        streamWriter.WriteLine(tempLine);
        tempLine = "";
        streamWriter.WriteLine(tempLine);

        //AI
        //None
        tempLine = "AI";
        streamWriter.WriteLine(tempLine);
        tempLine = "None";
        streamWriter.WriteLine(tempLine);
        tempLine = "";
        streamWriter.WriteLine(tempLine);

        //Mode
        //Clear
        //120
        tempLine = "Mode";
        streamWriter.WriteLine(tempLine);
        // Game mode text
        tempLine = gameMode.captionText.text;
        streamWriter.WriteLine(tempLine);
        // Time Limit
        tempLine = _timeLimit.ToString();
        streamWriter.WriteLine(tempLine);
        // Throws if we're in points mode
        if(gameMode.captionText.text == "Points") {
            tempLine = throwsField.text;
            streamWriter.Write(tempLine);
        }
        // Space
        tempLine = "";
        streamWriter.WriteLine(tempLine);
        streamWriter.WriteLine(tempLine);

        // Save the board
        tempLine = "Board";
        streamWriter.WriteLine(tempLine);
        streamWriter.Write(_levelScene);

        // Save the level
        /*
        tempLine = "Level";
        streamWriter.WriteLine(tempLine);
        streamWriter.Write(_levelPrefab);
        */
    #endregion

        tempLine = "\n";
        streamWriter.WriteLine(tempLine);
        tempLine = "Done";
        streamWriter.WriteLine(tempLine);

        streamWriter.Close();

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    public void LoadBoard(string path) {
        int nodeIndex = 0;
        int stringIndex = 0;
        int fileIndex = 1;

        // Clear the board of current bubbles
        ClearBoard();

        // Fill the name in
        fileNameField.text = path;

        string[] _linesFromFile;

#if UNITY_EDITOR
        TextAsset textAsset = Resources.Load<TextAsset>("Text/Created Boards/" + path);
        _linesFromFile = textAsset.text.Split("\n"[0]);
#else
        string allText = "";
        if (File.Exists(Application.dataPath + "/Created Boards/" + path + ".txt")) {
            Debug.Log("File exists!");
            allText = File.ReadAllText(Application.dataPath + "/Created Boards/" + path + ".txt");
        } else {
            Debug.Log("File does not exist!");
        }
        _linesFromFile = allText.Split("\n"[0]);
#endif

        int i = 0;
        foreach (string line in _linesFromFile) {
            _linesFromFile[i] = line.Replace("\r", "");
            //Debug.Log(_linesFromFile[i]);
            i++;
        }

        // Get to the layout data
        while (_linesFromFile[fileIndex] != "Bubble Layout") {
            fileIndex++;
        }
        fileIndex++;

        char _readChar = _linesFromFile[fileIndex][stringIndex++];
        while (_readChar != 'E') {
            if (_readChar != ',') {
                switch (_readChar) {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                        CreateBubbleSprite((int)char.GetNumericValue(_readChar), nodeIndex);
                        break;
                    case 'D': // Dead
                        CreateBubbleSprite((int)HAMSTER_TYPES.SKULL, nodeIndex);
                        break;
                    case 'R': // Rainbow
                        CreateBubbleSprite((int)HAMSTER_TYPES.RAINBOW, nodeIndex);
                        break;
                    case 'B': // Bomb
                        CreateBubbleSprite((int)HAMSTER_TYPES.BOMB, nodeIndex);
                        break;
                    case 'G': // Gravity
                        // Get the next char for the type
                        _readChar = _linesFromFile[fileIndex][stringIndex++];
                        CreateBubbleSprite((int)char.GetNumericValue(_readChar) + 11, nodeIndex);
                        break;
                    case 'I': // Ice
                        // Set the previous sprite to be ice
                        _nodes[--nodeIndex].bubble.SetIsIce(true);
                        break;
                    case 'N': // None
                        break;

                }
                nodeIndex++;
            } else {
                fileIndex++;
                stringIndex = 0;
            }
            //_readChar = (char)_reader.Read();
            _readChar = _linesFromFile[fileIndex][stringIndex++];
        }

        // Load special hamster spawns
        string _readLine = _linesFromFile[fileIndex];
        while (_readLine != "Special Hamsters") {
            _readLine = _linesFromFile[fileIndex++];
        }
        // Rainbow
        fileIndex++;
        _readLine = _linesFromFile[fileIndex++];
        if(_readLine == "Y") {
            rainbowToggle.isOn = true;
        }
        // Skull
        fileIndex++;
        _readLine = _linesFromFile[fileIndex++];
        if (_readLine == "Y") {
            skullToggle.isOn = true;
        }
        // Gravity
        fileIndex++;
        _readLine = _linesFromFile[fileIndex++];
        if (_readLine == "Y") {
            //plasmaToggle.isOn = true;
        }
        // Bomb
        fileIndex++;
        _readLine = _linesFromFile[fileIndex++];
        if (_readLine == "Y") {
            bombToggle.isOn = true;
        }

        // Load the game data
        while (_readLine != "Mode") {
            _readLine = _linesFromFile[fileIndex++];
        }
        // Get the game mode
        _readLine = _linesFromFile[fileIndex++];
        if(_readLine == "Points") {
            gameMode.value = 1;
        }
        // Get the time limit
        _readLine = _linesFromFile[fileIndex++];
        _timeLimit = int.Parse(_readLine);
        timeLimitField.text = _readLine;
        // If it's points, get the throw limit
        if(gameMode.value == 1) {
            _readLine = _linesFromFile[fileIndex++];
            throwsField.text = _readLine;
        }

        // Load the board
        _readLine = _linesFromFile[fileIndex++];
        while(_readLine != "Board") {
            _readLine = _linesFromFile[fileIndex++];
        }
        _readLine = _linesFromFile[fileIndex++];
        GameObject newLevel = LoadLevel(_readLine);

        // Delete and replace the old level
        ChangeLevel(newLevel, _readLine);
    }

    GameObject LoadLevel(string boardString) {
        GameObject levelPrefab = null;

        if (boardString.Contains("Forest")) {
            levelPrefab = Resources.Load<GameObject>("Prefabs/Level/Boards/Multiplayer/ForestBoard");
        } else if (boardString.Contains("Mountain")) {
            levelPrefab = Resources.Load<GameObject>("Prefabs/Level/Boards/Multiplayer/MountainBoard");
        } else if (boardString.Contains("Beach")) {
            levelPrefab = Resources.Load<GameObject>("Prefabs/Level/Boards/Multiplayer/BeachBoard");
        } else if (boardString.Contains("City")) {
            levelPrefab = Resources.Load<GameObject>("Prefabs/Level/Boards/Multiplayer/CityBoard");
        } else if (boardString.Contains("Corporation")) {
            levelPrefab = Resources.Load<GameObject>("Prefabs/Level/Boards/Multiplayer/CorporationBoard");
        } else if (boardString.Contains("Laboratory")) {
            levelPrefab = Resources.Load<GameObject>("Prefabs/Level/Boards/Multiplayer/LaboratoryBoard");
        } else if (boardString.Contains("Airship")) {
            levelPrefab = Resources.Load<GameObject>("Prefabs/Level/Boards/Multiplayer/AirshipBoard");
        }

        GameObject newLevel = GameObject.Instantiate(levelPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        HamsterSpawner[] spawners = newLevel.GetComponentsInChildren<HamsterSpawner>();
        foreach (HamsterSpawner hS in spawners) {
            hS.enabled = false;
        }
        Ceiling ceiling = FindObjectOfType<Ceiling>();
        ceiling.enabled = false;

        return newLevel;
    }

    public void ClearBoard() {
        // Delete all of the bubble sprites currently on the board
        BubbleSprite[] bSprites = FindObjectsOfType<BubbleSprite>();
        foreach(BubbleSprite bS in bSprites) {
            Destroy(bS.gameObject);
        }
    }

    void CreateBubbleSprite(int type, int node) {
        GameObject bSpriteObj = GameObject.Instantiate(hamsterSpriteObj, transform.position, Quaternion.identity);
        BubbleSprite bSprite = bSpriteObj.GetComponent<BubbleSprite>();
        bSprite.SetType(type);
        bSprite.node = node;
        bSprite.transform.position = (Vector2)_nodes[node].nPosition;
        _nodes[node].bubble = bSprite;
    }

    public void ChangeLevel(GameObject newLevel, string levelScene) {
        Destroy(levelObj);
        levelObj = newLevel;
        _levelScene = levelScene;
    }

    public void DisplayWarningText(string text) {
        warningText.text = text;
        warningText.gameObject.SetActive(true);
    }

    public void HideWarningText() {
        warningText.gameObject.SetActive(false);
    }

    public EditorNode GetNode(int i) {
        if (i >= 0 && i < _nodes.Count) {
            return _nodes[i];
        }

        return null;
    }

    public void ChangeFileName() {
        _fileName = fileNameField.text;
    }

    public void ChangeTimeLimit() {
        _timeLimit = int.Parse(timeLimitField.text);
    }

    public void TestLevel() {
        if (_fileName != "") {
            _gameManager.prevLevel = _fileName;
            _gameManager.isSinglePlayer = true;
            SaveBoard();
            GetComponent<BoardLoader>().ReadCreatedBoard(_fileName);
        } else {
            DisplayWarningText("No filename chosen.");
        }
    }
}
