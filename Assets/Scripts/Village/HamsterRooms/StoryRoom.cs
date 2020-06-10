using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryRoom : HamsterRoom {
    // Start is called before the first frame update
    void Start() {

    }

    protected override void DirectHamster() {
        base.DirectHamster();

        FaceHamsterLeft();
    }
}
