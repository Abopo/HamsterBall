using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpHamsterSpawn : HamsterSpawnAnimation {

    public Transform leftEleDoor;
    public Transform rightEleDoor;
    public SpriteRenderer eleButton;

    // Start is called before the first frame update
    void Start() {
        // Make sure we default to off
        EndAnim();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    public override void SpawnAnim() {
        base.SpawnAnim();

        // Move doors to the side
        leftEleDoor.localPosition = new Vector3(-0.391f, leftEleDoor.localPosition.y, leftEleDoor.localPosition.z);
        rightEleDoor.localPosition = new Vector3(0.475f, rightEleDoor.localPosition.y, rightEleDoor.localPosition.z);

        // Change button sprite

    }

    public override void EndAnim() {
        base.EndAnim();

        // Move doors back to closed
        leftEleDoor.localPosition = new Vector3(-0.161f, leftEleDoor.localPosition.y, leftEleDoor.localPosition.z);
        rightEleDoor.localPosition = new Vector3(0.192f, rightEleDoor.localPosition.y, rightEleDoor.localPosition.z);

        // Reset button sprite

    }
}
