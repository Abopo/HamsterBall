﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRoom : HamsterRoom {
    // Start is called before the first frame update
    void Start() {

    }

    protected override void DirectHamster() {
        base.DirectHamster();

        FaceHamsterRight();
    }
}