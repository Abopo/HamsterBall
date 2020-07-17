using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRoom : HamsterRoom { 
    // Start is called before the first frame update
    void Start() {
        
    }

    protected override void DirectHamster() {
        base.DirectHamster();

        FaceHamsterDown();
    }

    void FaceHamsterDown() {
        // Hamster is heading left
        // So set the hamster to our y position
        _containedHamsters[0].transform.position = new Vector3(transform.position.x,
                                                                transform.position.y,
                                                                _containedHamsters[0].transform.position.z);

        // Then face the hamster left
        _containedHamsters[0].FaceLeft();

        // Going down steps to door
        _containedHamsters[0].transform.Rotate(0f, 0f, 45f, Space.Self);
    }
}
