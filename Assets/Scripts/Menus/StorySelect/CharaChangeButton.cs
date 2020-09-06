using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaChangeButton : MenuButton {

    SpriteChange[] _sprites;

    protected override void Awake() {
        base.Awake();

        _sprites = GetComponentsInChildren<SpriteChange>();
    }
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        // Don't scale these buttons
        _selectedScale = _baseScale;
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

    }

    public override void Highlight() {
        base.Highlight();

        if (_sprites == null) {
            _sprites = GetComponentsInChildren<SpriteChange>();
        }
        if (_sprites != null) {
            // Set all selectables to 'selected'
            foreach (SpriteChange sprite in _sprites) {
                sprite.On();
            }
        }
    }

    public override void Unhighlight() {
        base.Unhighlight();

        if (_sprites != null) {
            // Unselect selectables
            foreach (SpriteChange sprite in _sprites) {
                sprite.Off();
            }
        }
    }
}
