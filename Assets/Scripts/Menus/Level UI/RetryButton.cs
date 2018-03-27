using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryButton : MenuOption {
    BoardLoader _boardLoader;
    GameManager _gameManager;

    // Use this for initialization
    protected override void Start () {
        base.Start();

        _boardLoader = FindObjectOfType<BoardLoader>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    protected override void Select() {
        //base.Select();

        Retry();
    }

    public override void Highlight() {
        base.Highlight();
    }

    public void Retry() {
        _gameManager.CleanUp();

        if (_gameManager.LevelDoc != null) {
            _boardLoader.ReadBoardSetup(_gameManager.LevelDoc);
        } else {
            _gameManager.PlayAgainButton();
        }
    }
}
