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

        TryMoveWorld();
    }

    public override void Highlight() {
        //base.Highlight();

        TryMoveWorld();
    }

    void TryMoveWorld() {
        if(!_storySelectMenu.MovingWorld) {
            if (dir == 1) {
                TryMoveRight();
            } else if (dir == -1) {
                TryMoveLeft();
                //_storySelectMenu.StartMoveWorlds(dir);
                //DeHighlightOtherOptions();
            }
        }
    }

    void TryMoveRight() {
        string storyProgress = ES3.Load<string>("StoryProgress");

        if (_storySelectMenu.CurWorld+1 < int.Parse(storyProgress[0].ToString())) {
            _storySelectMenu.StartMoveWorlds(dir);
            DeHighlightOtherOptions();
        }
    }

    void TryMoveLeft() {
        if(_storySelectMenu.CurWorld > 0) {
            _storySelectMenu.StartMoveWorlds(dir);
            DeHighlightOtherOptions();
        }
    }
}
