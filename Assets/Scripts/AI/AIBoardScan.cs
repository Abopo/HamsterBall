using UnityEngine;
using System.Collections.Generic;

// This class will scan the boards in play to determine potential bubble
// matches and good spots for the AI to throw hamsters to.
public class AIBoardScan : MonoBehaviour {
    BubbleManager _bubbleManagerLeft;
    BubbleManager _bubbleManagerRight;

    List<Bubble> _availableBubbles = new List<Bubble>();
    List<Node> _availableNodes = new List<Node>();
    List<Bubble> _opponentBubbles = new List<Bubble>();
    List<Node> _opponentNodes = new List<Node>();

    float scanTimer = 0.0f;
    float scanTime = 1.0f;

    public List<Bubble> AvailableBubbles {
        get { return _availableBubbles; }
    }
    public List<Node> AvailableNodes {
        get { return _availableNodes; }
    }
    public List<Bubble> OpponentBubbles {
        get { return _opponentBubbles; }
    }
    public List<Node> OpponentNodes {
        get { return _opponentNodes; }
    }

    PlayerController _playerController;

    // Use this for initialization
    void Start () {
        _bubbleManagerLeft = GameObject.FindGameObjectWithTag("BubbleManager1").GetComponent<BubbleManager>();
        _bubbleManagerRight = GameObject.FindGameObjectWithTag("BubbleManager2").GetComponent<BubbleManager>();

        _playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update () {
        // For now just scan every second.
        // TODO: Only scan when the board changes.
        scanTimer += Time.deltaTime;
        if(scanTimer >= scanTime) {
            ScanBoard();
            //scanTimer = 0f;
        }
	}

    void ScanBoard() {
        // For now just doing this with our teams board.
        if (_playerController.team == 0) {
            _availableBubbles = SortBubbles(_bubbleManagerLeft);
            _opponentBubbles = SortBubbles(_bubbleManagerRight);
            _availableNodes = GetNodes(_bubbleManagerLeft);
            _opponentNodes = GetNodes(_bubbleManagerRight);
        } else if(_playerController.team == 1) {
            _availableBubbles = SortBubbles(_bubbleManagerRight);
            _opponentBubbles = SortBubbles(_bubbleManagerLeft);
            _availableNodes = GetNodes(_bubbleManagerRight);
            _opponentNodes = GetNodes(_bubbleManagerLeft);
        }
    }

    List<Bubble> SortBubbles(BubbleManager bubbleManager) {
        List<Bubble> allBubbles = new List<Bubble>();
        List<Bubble> sortedBubbles = new List<Bubble>();
        // Turn bubbles array into list in reverse for easy sorting.
        for (int i = bubbleManager.Bubbles.Length - 1; i >= 0; --i) {
            if (bubbleManager.Bubbles[i] != null) {
                allBubbles.Add(bubbleManager.Bubbles[i]);
            }
        }

        // Sort bubbles by node number (descending).
        allBubbles.Sort((x, y) => y.node.CompareTo(x.node));

        sortedBubbles = new List<Bubble>();
        // Only need to search the lowest 2 rows or so.
        for (int i = 0; i < allBubbles.Count; ++i) {
            if (allBubbles[i].canBeHit) {
                sortedBubbles.Add(allBubbles[i]);
            }
        }

        // Check matches of the available bubbles
        /*
        List<Bubble> tempBubs = new List<Bubble>();
        foreach (Bubble b in _availableBubbles) {
            tempBubs = b.CheckMatches(tempBubs);
            b.numMatches = tempBubs.Count;
            tempBubs.Clear();
        }
        */

        // Sort the available bubbles by how many matches there are.
        sortedBubbles.Sort((x, y) => y.numMatches.CompareTo(x.numMatches));

        return sortedBubbles;
    }

    List<Node> GetNodes(BubbleManager bubbleManager) {
        List<Node> freeNodes = new List<Node>();

        // Turn nodes array into list in reverse.
        for(int i = bubbleManager.nodeList.Count-1; i >= 0; --i) {
            // If this node is free and has adjacent bubbles.
            if(bubbleManager.nodeList[i].bubble == null && bubbleManager.nodeList[i].isRelevant) {
                // Add it to the list.
                freeNodes.Add(bubbleManager.nodeList[i]);
            }
        }

        return freeNodes;
    }
}
