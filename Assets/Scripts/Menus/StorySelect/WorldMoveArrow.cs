using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMoveArrow : MenuOption {
    public int dir;

    StorySelectMenu _storySelectMenu;

    // Use this for initialization
    protected override void Start() {
        base.Start();

        _storySelectMenu = FindObjectOfType<StorySelectMenu>();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    protected override void Select() {
        base.Select();

        _storySelectMenu.StartMoveWorlds(dir);
    }

    public override void Highlight() {
        //base.Highlight();

        _storySelectMenu.StartMoveWorlds(dir);
    }
}
