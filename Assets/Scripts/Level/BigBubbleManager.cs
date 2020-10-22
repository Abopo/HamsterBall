using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBubbleManager : BubbleManager {

    protected override void Awake() {
        _baseLineLength = 27;

        base.Awake();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
	}

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    protected override int[] DecideNextLine(int lineLength) {
        int[] nextLineBubbles = new int[lineLength];
        int[] typeCounts = new int[7]; // Keeps track of how many of each color has been made
        int tempType = 0;

        for (int i = 0; i < lineLength; ++i) {
            // If the line already has too many of the type, try again
            // TODO: This could potentially be really slow, maybe optimize it sometime
            do {
                tempType = Random.Range(0, (int)HAMSTER_TYPES.NUM_NORM_TYPES);
            } while (typeCounts[tempType] > 7);

            // Increase count of type
            typeCounts[tempType] += 1;

            // Save type for next line
            nextLineBubbles[i] = tempType;
        }

        return nextLineBubbles;
    }
}
