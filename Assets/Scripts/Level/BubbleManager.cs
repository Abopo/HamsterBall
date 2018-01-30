using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class BubbleManager : MonoBehaviour {

	public GameObject bubbleObj;
	public GameObject NodeLine13;
	public GameObject NodeLine12;
	public HamsterMeter hamsterMeter;
    public ResultsScreen resultsScreen;

	//public bool leftTeam;
    public int team; // -1 = no team, 0 = left team, 1 = right team
	public int bubbleStock; // counts up to 12 then adds a line.
    public bool testMode;

    // 10 rows
    // alternating between 13 and 12 columns
    // should specify top and bottom rows
    public List<Node> nodeList = new List<Node>();
	int topLineLength = 13;
	public int TopLineLength {
		get {return topLineLength;}
	}

    public Bubble[] Bubbles {
        get { return bubbles; }
    }

    Vector3 nodeSpawnPos = new Vector3(-5.35f, 5.6f, 0f);

	int numNodes = 150;
	bool justAddedBubble = false;
    bool boardChangedFlag = false;
    Transform bubblesParent;
    Transform nodesParent;
    float nodeHeight = 0.67f; // The height of a single node (i.e. how far down lines move)
    int bottomRowStart {
        get {
            return numNodes - 25 + topLineLength;
        }
    }

    public static int[] startingBubbleTypes = Enumerable.Repeat<int>(-1, 50).ToArray(); // initializes 50 values to -1

    static List<int> _nextLineBubbles = new List<int>();
    int nextLineIndex = 0; // counts up as new lines are added
    bool _setupDone;

    int _comboCount = 0;

    Bubble[] bubbles;

    Bubble lastBubbleAdded;
    public Bubble LastBubbleAdded {
        get {return lastBubbleAdded;}
        set { lastBubbleAdded = value; }
    }

    BubbleEffects _bubbleEffects;
    public BubbleEffects BubbleEffects {
        get { return _bubbleEffects; }
    }

    public int NextLineIndex {
        get { return nextLineIndex; }
    }

    public int ComboCount {
        get { return _comboCount; }
    }

    //public int[] NextLineBubbles {
    //    get { return _nextLineBubbles; }
    //    set { _nextLineBubbles = value; }
    //}

    GameManager _gameManager;
    AudioSource _audioSource;
    AudioClip _bubblePopClip;
    AudioClip _addLineClip;

    void Awake() {
        _setupDone = false;

        Time.timeScale = 1;
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        bubblesParent = transform.GetChild(0);
        nodesParent = transform.GetChild(1);

        bubbles = new Bubble[numNodes];
        BuildStartingNodes();

        // If we are networked
        if (PhotonNetwork.connectedAndReady) {
            // If we are the master client
            if(PhotonNetwork.isMasterClient) {
                // Go ahead and make the starting bubbles
                SpawnStartingBubbles();
                justAddedBubble = true;
            }
        } else {
            SpawnStartingBubbles();
            justAddedBubble = true;
        }

        // Get the next line of bubbles
        if (!PhotonNetwork.connectedAndReady || (PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient)) {
            SeedNextLineBubbles();
        }

        testMode = _gameManager.testMode;

        _audioSource = GetComponent<AudioSource>();
        _bubblePopClip = Resources.Load<AudioClip>("Audio/SFX/Pop");
        _addLineClip = Resources.Load<AudioClip>("Audio/SFX/Add_Line");

        ReadyHamsterMeter();
    }

    // Use this for initialization
    void Start () {
        _bubbleEffects = GetComponentInChildren<BubbleEffects>();

        // Send RPC if we are networked
        if (PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient) {
            GetComponent<PhotonView>().RPC("SyncLineBubbles", PhotonTargets.Others, _nextLineBubbles.ToArray());
        }
    }

    void BuildStartingNodes() {
		bool longLine = true;
		for (int i = 0; i < 12; ++i) {
			nodeSpawnPos = new Vector3(transform.position.x, (transform.position.y + 2.9f) - (0.67f * i), 0);
			if(longLine) {
				GameObject nodeLine = Instantiate(NodeLine13, nodeSpawnPos, Quaternion.identity) as GameObject;
                for (int j = 0; j < 13; ++j) {
                    nodeLine.transform.GetChild(0).GetComponent<Node>().number = nodeList.Count;
                    nodeList.Add(nodeLine.transform.GetChild(0).GetComponent<Node>());
                    nodeLine.transform.GetChild(0).parent = nodesParent;
				}
				DestroyObject(nodeLine);
				longLine = false;
			} else {
				GameObject nodeLine = Instantiate(NodeLine12, nodeSpawnPos, Quaternion.identity) as GameObject;
				for(int j = 0; j < 12; ++j) {
                    nodeLine.transform.GetChild(0).GetComponent<Node>().number = nodeList.Count;
                    nodeList.Add(nodeLine.transform.GetChild(0).GetComponent<Node>());
                    nodeLine.transform.GetChild(0).parent = nodesParent;
				}
				DestroyObject(nodeLine);
				longLine = true;
			}
		}

		nodeSpawnPos = new Vector3(transform.position.x, (transform.position.y + 2.9f), 0f);
	}

    public void SpawnStartingBubbles() {
        int numBubbles = 50;

        // If the starting bubbles have not been built yet
        if (startingBubbleTypes[0] == -1) {
            int tempType = 0;
            List<Bubble> tempMatches = new List<Bubble>();
            List<int> okTypes = new List<int>();
            for (int i = 0; i < numBubbles; ++i) {
                // Create and initialize a new bubble
                GameObject bub = Instantiate(bubbleObj, nodeList[i].nPosition, Quaternion.identity) as GameObject;
                Bubble bubble = bub.GetComponent<Bubble>();
                bubble.transform.parent = bubblesParent;
                bubble.node = i;
                // Add the new bubble to necessary lists
                nodeList[i].bubble = bubble;
                bubbles[i] = bubble;

                // Temporarily initialize new bubble as a Dead bubble to prevent inaccurate match calculations.
                InitBubble(bubble, (int)HAMSTER_TYPES.DEAD);

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
                        if (tempMatches.Count > 3) {
                            // Don't make another bubble of that type,
                            // Remove that type from the okTypes list.
                            okTypes.Remove((int)b.type);
                        }

                        tempMatches.Clear();
                    }
                }

                // Randomly choose type for the new bubble based on available types
                tempType = Random.Range(0, okTypes.Count);
                startingBubbleTypes[i] = okTypes[tempType];

                InitBubble(bubble, okTypes[tempType]);

                // Reset for next check
                for (int j = 0; j < numBubbles; ++j) {
                    if (bubbles[j] != null) {
                        bubbles[j].checkedForMatches = false;
                    } else { // Break out at the first null bubble since there won't be any after
                        break;
                    }
                }
            }
        } else { // If this rounds starting bubbles have already been decided
            for (int i = 0; i < numBubbles; ++i) {
                // Create a new bubble
                GameObject bub = Instantiate(bubbleObj, nodeList[i].nPosition, Quaternion.identity) as GameObject;
                Bubble bubble = bub.GetComponent<Bubble>();
                bubble.transform.parent = bubblesParent;
                // Set it to the corresponding type of the decided starting bubbles
                int type = startingBubbleTypes[i];
                InitBubble(bubble, type);
                bubble.node = i;
                nodeList[i].bubble = bubble;
                bubbles[i] = bubble;
            }
        }

		// assign adj bubbles for each starting bubble and empty node
		UpdateAllAdjBubbles ();

        // set starting bubbles matches count
        for (int i = 0; i < numBubbles; ++i) {
            bubbles[i].matches = bubbles[i].CheckMatches(bubbles[i].matches);
            bubbles[i].numMatches = bubbles[i].matches.Count;
            
            // Reset for next check
            for (int j = 0; j < numBubbles; ++j) {
                bubbles[j].checkedForMatches = false;
            }
        }

        _setupDone = true;
	}

	void InitBubble(Bubble bubble, int Type) {
		//bubble.leftTeam = leftTeam;
        bubble.team = team;
		bubble.Initialize((HAMSTER_TYPES)Type);
		bubble.locked = true;
	}

    void SeedNextLineBubbles() {
        int[] lineBubbles;
        for (int i = 0; i < 10; ++i) {
            lineBubbles = DecideNextLine();
            foreach (int bub in lineBubbles) {
                _nextLineBubbles.Add(bub);
            }
        }
    }

    void ReadyHamsterMeter() {
        int handicap = 9;
        
        // Get team handicap from Game Manager.
        if (team == 0) {
            handicap = _gameManager.leftTeamHandicap;
        } else if (team == 1) {
            handicap = _gameManager.rightTeamHandicap;
        }

        hamsterMeter.Initialize(handicap);
    }

    // Assign adjBubbles for each bubble and empty node
    void UpdateAllAdjBubbles() {
		for(int i = 0; i < numNodes; ++i) {
			if(bubbles[i] != null) {
				bubbles[i].ClearAdjBubbles();
				AssignAdjBubbles(bubbles[i], i);
			}
            if(nodeList[i].bubble == null) {
                nodeList[i].ClearAdjBubbles();
                AssignAdjBubbles(null, i);
            }
		}
	}

	public void AssignAdjBubbles(Bubble bubble, int node) {
		// Now must adjust based on which size of line is on top
		if(topLineLength == 13) { // if top line is a 13 line.
			// Corners
			if (node == 0) {
				GetMiddleRight(bubble, node);
				GetBottomRight(bubble, node);
			} else if(node == 12) {
				GetMiddleLeft(bubble, node);
				GetBottomLeft(bubble, node);
			} else if(node == 138) {
				GetTopLeft(bubble, node);
				GetTopRight(bubble, node);
				GetMiddleRight(bubble, node);
			} else if(node == 149) {
				GetTopLeft(bubble, node);
				GetTopRight(bubble, node);
				GetMiddleLeft(bubble, node);
			} 
			// Outer Lefts
			else if (node == 25 || node == 50 || node == 75 || node == 100 || node == 125) {
				GetTopRight(bubble, node);
				GetMiddleRight(bubble, node);
				GetBottomRight(bubble, node);
                if (bubble != null) {
                    bubble.is13Edge = -1;
                }
			} 
			// Outer Rights
			else if (node == 37 || node == 62 || node == 87 || node == 112 || node == 137) {
				GetTopLeft(bubble, node);
				GetMiddleLeft(bubble, node);
				GetBottomLeft(bubble, node);
                if (bubble != null) {
                    bubble.is13Edge = 1;
                }
            }
            // Inner Lefts
            else if (node == 13 || node == 38 || node == 63 || node == 88 || node == 113) {
				GetTopLeft(bubble, node);
				GetTopRight(bubble, node);
				GetMiddleRight(bubble, node);
				GetBottomLeft(bubble, node);
				GetBottomRight(bubble, node);
			} 
			// Inner Rights
			else if (node == 24 || node == 49 || node == 74 || node == 99 || node == 124) {
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
		} else { // if top line is a 12 line.
			// Corners
			if (node == 0) {
				GetMiddleRight(bubble, node);
				GetBottomRight(bubble, node);
				GetBottomLeft(bubble, node);
			} else if(node == 11) {
				GetMiddleLeft(bubble, node);
				GetBottomLeft(bubble, node);
				GetBottomRight(bubble, node);
			} else if(node == 137) {
				GetTopRight(bubble, node);
				GetMiddleRight(bubble, node);
			} else if(node == 148) {
				GetTopLeft(bubble, node);
				GetMiddleLeft(bubble, node);
			} 
			// Outer Lefts
			else if (node == 12 || node == 37 || node == 62 || node == 87 || node == 112) {
				GetTopRight(bubble, node);
				GetMiddleRight(bubble, node);
				GetBottomRight(bubble, node);
                if (bubble != null) {
                    bubble.is13Edge = -1;
                }
            }
            // Outer Rights
            else if (node == 24 || node == 49 || node == 74 || node == 99 || node == 124) {
				GetTopLeft(bubble, node);
				GetMiddleLeft(bubble, node);
				GetBottomLeft(bubble, node);
                if (bubble != null) {
                    bubble.is13Edge = 1;
                }
            }
            // Inner Lefts
            else if (node == 25 || node == 50 || node == 75 || node == 100 || node == 125) {
				GetTopLeft(bubble, node);
				GetTopRight(bubble, node);
				GetMiddleRight(bubble, node);
				GetBottomLeft(bubble, node);
				GetBottomRight(bubble, node);
			} 
			// Inner Rights
			else if (node == 36 || node == 61 || node == 86 || node == 111 || node == 136) {
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
		if (node - 13 >= 0) {
            if (bubble != null) {
                bubble.adjBubbles[0] = bubbles[node - 13]; // top left
            } else {
                nodeList[node].AdjBubbles[0] = bubbles[node - 13];
            }
		}
	}
	void GetTopRight(Bubble bubble, int node) {
		if (node - 12 >= 0) {
            if (bubble != null) {
                bubble.adjBubbles[1] = bubbles[node - 12]; // top right
            } else {
                nodeList[node].AdjBubbles[1] = bubbles[node - 12];
            }
        }
	}
	void GetMiddleLeft(Bubble bubble, int node) {
		if (node - 1 >= 0) {
            if (bubble != null) {
                bubble.adjBubbles[5] = bubbles[node - 1]; // middle left
            } else {
                nodeList[node].AdjBubbles[5] = bubbles[node - 1];
            }
        }
	}
	void GetMiddleRight(Bubble bubble, int node) {
		if (node + 1 < numNodes) {
            if (bubble != null) {
                bubble.adjBubbles[2] = bubbles[node + 1]; // middle right
            } else {
                nodeList[node].AdjBubbles[2] = bubbles[node + 1];
            }
        }
	}
	void GetBottomLeft(Bubble bubble, int node) {
		if (node + 12 < numNodes) {
            if (bubble != null) {
                bubble.adjBubbles[4] = bubbles[node + 12]; // bottom left
            } else {
                nodeList[node].AdjBubbles[4] = bubbles[node + 12];
            }
		}
	}
	void GetBottomRight(Bubble bubble, int node) {
		if (node + 13 < numNodes) {
            if (bubble != null) {
                bubble.adjBubbles[3] = bubbles[node + 13]; // bottom right
            } else {
                nodeList[node].AdjBubbles[3] = bubbles[node + 13];
            }
		}
	}

	// Update is called once per frame
	void Update () {
        if(startingBubbleTypes[0] != -1 && !_setupDone) {
            SpawnStartingBubbles();
        }
        if (justAddedBubble) {
			// Check for anchor points
			List<Bubble> anchorBubbles = new List<Bubble>();
			foreach(Bubble b in bubbles) {
				if(b != null) {
					anchorBubbles.Clear();
					b.CheckForAnchor(anchorBubbles);
				}
			}

			if(CheckBottomRow()) {
                // Show end game screen
                EndGame();
			}

			justAddedBubble = false;
		}

        if(boardChangedFlag) {
            UpdateAllAdjBubbles();
            boardChangedFlag = false;
        }

        if(testMode) {
            CheckInput();
        }
	}

    void CheckInput() {
        if(Input.GetKeyDown(KeyCode.PageUp)) {
            //HandleEnemyMatch(1);
        }
    }

    public void AddBubble(Bubble newBubble) {
        int closestNode;
        closestNode = FindClosestNode(newBubble);

        if(closestNode == -1) {
            // something really wrong happened
            // if we ever get here, maybe add more backup nodes.
            Debug.Log("No node found for bubble, Destroying");
            Destroy(newBubble.gameObject);
            return;
        }

        newBubble.node = closestNode;
        newBubble.transform.position = nodeList[closestNode].nPosition;

		// assign adj bubbles
		AssignAdjBubbles (newBubble, closestNode);

        newBubble.transform.parent = bubblesParent;

        // add to list
        bubbles [newBubble.node] = newBubble;
        nodeList[newBubble.node].bubble = newBubble;

		UpdateAllAdjBubbles ();

		justAddedBubble = true;
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

        newBubble.transform.parent = bubblesParent;

        // add to list
        bubbles[newBubble.node] = newBubble;
        nodeList[newBubble.node].bubble = newBubble;

        UpdateAllAdjBubbles();

        justAddedBubble = true;
    }

    int FindClosestNode(Bubble bubble) {
        // find closest node
        int closestNode = -1;
        // Have the main node, and some backup nodes just in case the main node is taken.
        // Keep nodes in order from closest found to 3rd closest found.
        int node1 = -1, node2 = -1, node3 = -1;
        float dist1 = 1000000, dist2 = 2000000, dist3 = 3000000;
        float tempDist = 0;
        for (int i = 0; i < numNodes; ++i) {
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
        for (int i = 0; i < bubbles.Length - 1; ++i) {
            if (bubbles[i] == null) {
                continue;
            }
            if (bubbles[i].node == node1) {
                // Already a bubble in that node, eliminate it
                node1 = -1;
            } else if (bubbles[i].node == node2) {
                // Already a bubble in that node, eliminate it
                node2 = -1;
            } else if (bubbles[i].node == node3) {
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
        if(node == -1) {
            Debug.Log("Shiiit");
        }
		bubbles [node] = null;

		if (!_audioSource.isPlaying) {
            _audioSource.clip = _bubblePopClip;
			_audioSource.Play();
		}

        boardChangedFlag = true;
	}

    public void AddLine() {
		for (int i = 0; i < nodeList.Count; ++i) {
			// Delete bottom line
			if(i >= bottomRowStart) {
				DestroyObject(nodeList[i].gameObject);
			// Move nodes down
			} else {
				nodeList[i].transform.Translate(new Vector3(0.0f, -nodeHeight, 0.0f));
                nodeList[i].number += topLineLength;
			}
		}

        // Remove the deleted nodes from the nodeList
        nodeList.RemoveRange(bottomRowStart, (topLineLength == 13 ? 12 : 13));

		Node tempChild;
		// Add top line
		if(topLineLength == 12) {
			GameObject nodeLine = Instantiate(NodeLine13, nodeSpawnPos, Quaternion.identity) as GameObject;
            for (int j = 12; j >= 0; --j) {
				tempChild = nodeLine.transform.GetChild(j).GetComponent<Node>();
				tempChild.transform.parent = this.transform;
				tempChild.transform.SetAsFirstSibling();
                tempChild.number = j;
                nodeList.Insert(0, tempChild);
			}

			DestroyObject(nodeLine);
			topLineLength = 13;
		} else if(topLineLength == 13) {
			GameObject nodeLine = Instantiate(NodeLine12, nodeSpawnPos, Quaternion.identity) as GameObject;
            for (int j = 11; j >= 0; --j) {
                tempChild = nodeLine.transform.GetChild(j).GetComponent<Node>();
                tempChild.transform.parent = this.transform;
                tempChild.transform.SetAsFirstSibling();
                tempChild.number = j;
                nodeList.Insert(0, tempChild);
            }
            DestroyObject(nodeLine);
			topLineLength = 12;
		}

		// Move bubbles to nodes
		Bubble[] tempBubbles = new Bubble[numNodes];
		for (int i = 0; i < numNodes; ++i) {
			tempBubbles[i] = bubbles[i];
		}
		bubbles = new Bubble[numNodes];
		List<Bubble> testList = new List<Bubble> ();
		foreach (Bubble b in tempBubbles) {
			if(b != null) {
				b.node += topLineLength;
				b.transform.position = nodeList[b.node].nPosition;
				bubbles[b.node] = b;
				testList.Add(b);
			}
		}

        // Spawn bubbles on top line
		for (int i = 0; i < topLineLength; ++i, ++nextLineIndex) {
			GameObject bub = Instantiate(bubbleObj, nodeList[i].nPosition, Quaternion.identity) as GameObject;
			Bubble bubble = bub.GetComponent<Bubble>();
            bubble.transform.parent = bubblesParent;

            // Init bubble using the types that were decided ahead of time
            InitBubble(bubble, _nextLineBubbles[nextLineIndex]);
			bubble.node = i;
            nodeList[i].bubble = bubble;
			bubbles[i] = bubble;
		}

        // If we've hit the end of the generated line bubbles
        if(nextLineIndex >= _nextLineBubbles.Count && (!PhotonNetwork.connectedAndReady || (PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient))) {
            // Generate some more!
            SeedNextLineBubbles();
            // Send RPC if we are networked
            if (PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient) {
                GetComponent<PhotonView>().RPC("SyncLineBubbles", PhotonTargets.Others, _nextLineBubbles);
            }
        }

        UpdateAllAdjBubbles ();

		justAddedBubble = true;

        // Send RPC if we are networked
        if (PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient) {
            GetComponent<PhotonView>().RPC("AddLine", PhotonTargets.Others/*, _nextLineBubbles*/);
        }

        // Get the next line of bubbles
        //if (!PhotonNetwork.connectedAndReady || (PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient)) {
         //   _nextLineBubbles = DecideNextLine();
        //}

        // Play sound
        _audioSource.clip = _addLineClip;
        _audioSource.Play();
	}

    int[] DecideNextLine() {
        int[] nextLineBubbles = new int[13];
        int[] typeCounts = new int[7]; // Keeps track of how many of each color has been made
        int tempType = 0;

        for (int i = 0; i < topLineLength; ++i) {
            // If the line already has too many of the type, try again
            // TODO: This could potentially be really slow, maybe optimize it sometime
            do {
                tempType = Random.Range(0, (int)HAMSTER_TYPES.NUM_NORM_TYPES);
            } while (typeCounts[tempType] > 3);

            // Increase count of type
            typeCounts[tempType] += 1;

            // Save type for next line
            nextLineBubbles[i] = tempType;
        }

        return nextLineBubbles;
    }

    public bool CheckBottomRow() {
		for(int i = bottomRowStart; i < numNodes; ++i) {
			if(bubbles[i] != null) {
				return true;
			}
		}

		return false;
	}

    void EndGame() {
        // Clear out starting bubbles to prepare for next round
        for(int i = 0; i < 50; ++i) {
            startingBubbleTypes[i] = -1;
        }

        _gameManager.EndGame(team);

        // Pause the game
        _gameManager.Pause();

        if (resultsScreen != null) {
            resultsScreen.Activate(team);
        }
    }

    public void RefreshRainbowBubbles() {
		for (int i = 0; i < numNodes; ++i) {
			if(bubbles[i] != null && bubbles[i].type == HAMSTER_TYPES.RAINBOW) {
				bubbles[i].checkedForMatches = false;
			}
		}
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

    public void IncreaseComboCounter(Vector3 position) {
        _comboCount += 1;

        if(_comboCount > 2) {
            // Show combo graphic
            _bubbleEffects.MatchComboEffect(position, _comboCount-3);
        }
    }
    public void ResetComboCounter() {
        _comboCount = 0;
    }
}
