using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDisplay : MonoBehaviour {
    public GameObject hamsterSpriteObj;
    public GameObject nodeParent;

    List<EditorNode> _nodes = new List<EditorNode>();

    private void Awake() {
        EditorNode[] nodes = nodeParent.GetComponentsInChildren<EditorNode>();
        foreach (EditorNode eN in nodes) {
            _nodes.Add(eN);
        }

        // Sort nodes by node number (ascending).
        _nodes.Sort((a, b) => a.nodeNum.CompareTo(b.nodeNum));
    }

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void LoadBoard(string path) {
        int nodeIndex = 0;
        int stringIndex = 0;
        int fileIndex = 1;

        // Clear the board of current bubbles
        ClearBoard();

        string[] _linesFromFile;

#if UNITY_EDITOR
        TextAsset textAsset = Resources.Load<TextAsset>("Text/" + path);
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

        //_readChar = (char)_reader.Read();
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
                        CreateBubbleSprite((int)HAMSTER_TYPES.DEAD, nodeIndex);
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

        // Load the time limit
        string _readLine = _linesFromFile[fileIndex];
        while (_readLine != "Mode") {
            _readLine = _linesFromFile[fileIndex++];
        }
        // Skip over an extra line
        fileIndex++;
        _readLine = _linesFromFile[fileIndex++];
    }

    public void ClearBoard() {
        // Delete all of the bubble sprites currently on the board
        BubbleSprite[] bSprites = FindObjectsOfType<BubbleSprite>();
        foreach (BubbleSprite bS in bSprites) {
            DestroyObject(bS.gameObject);
        }
    }
    
    void CreateBubbleSprite(int type, int node) {
        GameObject bSpriteObj = GameObject.Instantiate(hamsterSpriteObj, transform.position, Quaternion.identity, this.transform);
        BubbleSprite bSprite = bSpriteObj.GetComponent<BubbleSprite>();
        bSprite.SetType(type);
        bSprite.node = node;
        bSprite.transform.position = (Vector2)_nodes[node].nPosition;
        bSprite.transform.localScale = nodeParent.transform.localScale;
        _nodes[node].bubble = bSprite;
    }
}
