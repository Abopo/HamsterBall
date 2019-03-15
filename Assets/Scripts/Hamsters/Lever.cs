using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : HamsterDoor {
    public override void Open() {
        if (!_isOpen) {
            transform.Rotate(0f, 0f, -60f);

            _isOpen = true;
        }

        _openTimer = 0f;
    }

    protected override void Close() {
        if (_isOpen) {
            transform.Rotate(0f, 0f, 60f);

            _isOpen = false;
        }
    }
}
