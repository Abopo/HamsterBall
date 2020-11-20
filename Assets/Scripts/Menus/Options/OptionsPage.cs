using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsPage : Menu {

    OptionsTab _tab;

    public OptionsPage leftPage;
    public OptionsPage rightPage;

    protected override void Awake() {
        base.Awake();

        _tab = GetComponentInChildren<OptionsTab>();
    }
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    public override void Activate() {
        base.Activate();

        // Change tabs
        _tab.Select();
    }

    public override void Deactivate() {
        base.Deactivate();

        // Change tab
        _tab.Deselect();
    }
}
