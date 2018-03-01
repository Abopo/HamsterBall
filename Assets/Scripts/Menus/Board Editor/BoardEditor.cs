using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class BoardEditor : MonoBehaviour {
    public InputField fileNameField;
    public Text warningText;
    public GameObject hamsterSpriteObj;
    public GameObject levelObj;

    List<EditorNode> nodes = new List<EditorNode>();
    Rect boundingRect;
    string fileName;

    // Use this for initialization
    void Start () {
        GameObject[] nodeObjs = GameObject.FindGameObjectsWithTag("Node1");
        int i = 0;
        foreach(GameObject nO in nodeObjs) {
            nodes.Add(nO.GetComponent<EditorNode>());
            ++i;
        }

        // Sort nodes by node number (ascending).
        nodes.Sort((a, b) => a.nodeNum.CompareTo(b.nodeNum));

        // Create the boundingRect
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        float x, y, width, height;
        x = boxCollider.bounds.min.x;
        y = boxCollider.bounds.min.y;
        width = boxCollider.bounds.max.x - x;
        height = boxCollider.bounds.max.y - y;
        boundingRect = new Rect(x, y, width, height);

        fileName = "";
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
        // find closest node
        int closestNode = -1;

        // Have the main node, and some backup nodes just in case the main node is taken.
        // Keep nodes in order from closest found to 3rd closest found.
        int node1 = -1, node2 = -1, node3 = -1;
        float dist1 = 1000000, dist2 = 2000000, dist3 = 3000000;
        float tempDist = 0;
        for (int i = 0; i < nodes.Count; ++i) {
            tempDist = Vector2.Distance(nodes[i].transform.position, pos);
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
        if(boundingRect.Contains(pos)) {
            return true;
        }

        return false;
    }

    public void SaveBoard() {
        if(fileName == "") {
            DisplayWarningText("No filename chosen.");
            return;
        }

        string fullFileName = "Assets/Resources/Text/Created Boards/" + fileName + ".txt";
        FileStream file = File.Open(fullFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamWriter streamWriter = new StreamWriter(file);

        streamWriter.WriteLine(fileName);

        HAMSTER_TYPES tempType;
        string tempLine = "";
        bool longLine = true;
        int lineLength = 0;
        int index = 0;
        for(int i = 0; i < 10; ++i) {
            if(longLine) {
                lineLength += 13;
                longLine = false;
            } else {
                lineLength += 12;
                longLine = true;
            }

            for(int j = index; j < lineLength; ++j, ++index) {
                if (nodes[j].bubble != null) {
                    tempType = nodes[j].bubble.Type;
                    switch (tempType) {
                        case HAMSTER_TYPES.GREEN:
                            tempLine += "0";
                            break;
                        case HAMSTER_TYPES.RED:
                            tempLine += "1";
                            break;
                        case HAMSTER_TYPES.ORANGE:
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
                        case HAMSTER_TYPES.DEAD:
                            tempLine += "D";
                            break;
                        case HAMSTER_TYPES.RAINBOW:
                            tempLine += "R";
                            break;
                        case HAMSTER_TYPES.BOMB:
                            tempLine += "B";
                            break;
                    }
                } else {
                    tempLine += "N";
                }
            }

            tempLine += ",";
            streamWriter.WriteLine(tempLine);

            tempLine = "";
        }

        // End the file with an escape character
        tempLine = "E";
        streamWriter.WriteLine(tempLine);

        streamWriter.Close();

        AssetDatabase.Refresh();
    }

    public void LoadBoard(string path) {
        int nodeIndex = 0;
        int stringIndex = 0;
        int fileIndex = 1;

        // Clear the board of current bubbles
        ClearBoard();

        // Fill the name in
        fileNameField.text = path;

        TextAsset textAsset = Resources.Load<TextAsset>("Text/Created Boards/" + path);
        string[] _linesFromFile = textAsset.text.Split("\n"[0]);
        int i = 0;
        foreach (string line in _linesFromFile) {
            _linesFromFile[i] = line.Replace("\r", "");
            i++;
        }

        //_readChar = (char)_reader.Read();
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
                        CreateBubbleSprite((HAMSTER_TYPES)char.GetNumericValue(_readChar), nodeIndex);
                        break;
                    case 'D': // Dead
                        CreateBubbleSprite(HAMSTER_TYPES.DEAD, nodeIndex);
                        break;
                    case 'R': // Rainbow
                        CreateBubbleSprite(HAMSTER_TYPES.RAINBOW, nodeIndex);
                        break;
                    case 'B': // Bomb
                        CreateBubbleSprite(HAMSTER_TYPES.BOMB, nodeIndex);
                        break;
                    case 'G': // Gravity
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
    }

    public void ClearBoard() {
        // Delete all of the bubble sprites currently on the board
        BubbleSprite[] bSprites = FindObjectsOfType<BubbleSprite>();
        foreach(BubbleSprite bS in bSprites) {
            DestroyObject(bS.gameObject);
        }
    }

    void CreateBubbleSprite(HAMSTER_TYPES type, int node) {
        GameObject bSpriteObj = GameObject.Instantiate(hamsterSpriteObj, transform.position, Quaternion.identity);
        BubbleSprite bSprite = bSpriteObj.GetComponent<BubbleSprite>();
        bSprite.SetType(type);
        bSprite.node = node;
        bSprite.transform.position = (Vector2)nodes[node].nPosition;
        nodes[node].bubble = bSprite;
    }

    public void ChangeLevel(GameObject newLevel) {
        DestroyObject(levelObj);
        levelObj = newLevel;
    }

    public void DisplayWarningText(string text) {
        warningText.text = text;
        warningText.gameObject.SetActive(true);
    }

    public void HideWarningText() {
        warningText.gameObject.SetActive(false);
    }

    public EditorNode GetNode(int i) {
        if (i >= 0 && i < nodes.Count) {
            return nodes[i];
        }

        return null;
    }

    public void ChangeFileName() {
        fileName = fileNameField.text;
    }
}
