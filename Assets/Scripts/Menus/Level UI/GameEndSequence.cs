using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndSequence : MonoBehaviour {

    public Banner leftBanner;
    public Banner rightBanner;

    public Transform playerPos1;
    public Transform playerPos2;

    public SuperTextMesh leftText;
    public SuperTextMesh rightText;

    public List<PlayerController> _leftTeam = new List<PlayerController>();
    public List<PlayerController> _rightTeam = new List<PlayerController>();

    FMOD.Studio.EventInstance _matchEndMusic;

    int _gameResult;
    int _sequence = 0;

    // Start is called before the first frame update
    void Start() {
        // Make sure banners are above stage
		SoundManager.mainAudio.MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
        SoundManager.mainAudio.MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

        leftBanner.transform.position = new Vector3(leftBanner.transform.position.x, 22f, leftBanner.transform.position.z);
        rightBanner.transform.position = new Vector3(rightBanner.transform.position.x, 22f, rightBanner.transform.position.z);

        _matchEndMusic = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.MatchEndMusic);
    }

    void GetPlayers() {
        // Get players
        PlayerController[] _playerControllers = FindObjectsOfType<PlayerController>();

        // Sort players by team
        foreach (PlayerController pC in _playerControllers) {
            if (pC.team == 0) {
                _leftTeam.Add(pC);
            } else if (pC.team == 1) {
                _rightTeam.Add(pC);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void StartSequence(int winningTeam) {
        _gameResult = winningTeam;

        GetPlayers();

        // Set the banner sprites and text
        switch(winningTeam) {
            case 0: // Draw
                leftBanner.SetSprites(true);
                rightBanner.SetSprites(true);
                break;
            case -1: // Left Team won
                leftBanner.SetSprites(true);
                leftText.text = "<c=rainbow><w>WIN!";
                rightBanner.SetSprites(false);
                rightText.text = "<w=fancy>LOSE";
                break;
            case 1: // Right team won
                leftBanner.SetSprites(false);
                leftText.text = "<w=fancy>LOSE";
                rightBanner.SetSprites(true);
                rightText.text = "<c=rainbow><w>WIN!";
                break;
            case 2: // Team survival or something, both teams
                leftBanner.SetSprites(true);
                rightBanner.SetSprites(true);
                break;
        }

        // Start the banners moving down
        leftBanner.StartFall();
        rightBanner.StartFall();
    }

    public void ShiftFirstPlayers() {
        if (_sequence < 1) {
            ShiftState tempShiftState;

            tempShiftState = (ShiftState)_leftTeam[0].GetPlayerState(PLAYER_STATE.SHIFT);
            tempShiftState.endGameShift = true;
            _leftTeam[0].ChangeState(PLAYER_STATE.SHIFT);
            _leftTeam[0].Animator.SetBool("Won Game", _gameResult == -1 || _gameResult == 0);
            tempShiftState.SetLandingPosition(new Vector3(-playerPos1.position.x, playerPos1.position.y, playerPos1.position.z));

            tempShiftState = (ShiftState)_rightTeam[0].GetPlayerState(PLAYER_STATE.SHIFT);
            tempShiftState.endGameShift = true;
            _rightTeam[0].ChangeState(PLAYER_STATE.SHIFT);
            _rightTeam[0].Animator.SetBool("Won Game", _gameResult == 1 || _gameResult == 0);
            tempShiftState.SetLandingPosition(new Vector3(playerPos1.position.x, playerPos1.position.y, playerPos1.position.z));


            _matchEndMusic.start();

            _sequence++;
        }
    }
    public void ShiftSecondPlayers() {
        if (_sequence < 2) {
            ShiftState tempShiftState;

            if (_leftTeam.Count > 1) {
                tempShiftState = (ShiftState)_leftTeam[1].GetPlayerState(PLAYER_STATE.SHIFT);
                tempShiftState.endGameShift = true;
                _leftTeam[1].ChangeState(PLAYER_STATE.SHIFT);
                _leftTeam[1].Animator.SetBool("Won Game", _gameResult == -1 || _gameResult == 0);
                tempShiftState.SetLandingPosition(new Vector3(-playerPos2.position.x, playerPos2.position.y, playerPos2.position.z));

                //_leftTeam[1].transform.position = new Vector3(-playerPos2.position.x, playerPos2.position.y, playerPos2.position.z);
                //_leftTeam[1].transform.localScale = new Vector3(2f, 2f, 2f);
                //_leftTeam[1].SpriteRenderer.sortingOrder = 25;
            }
            if (_rightTeam.Count > 1) {
                tempShiftState = (ShiftState)_rightTeam[1].GetPlayerState(PLAYER_STATE.SHIFT);
                tempShiftState.endGameShift = true;
                _rightTeam[1].ChangeState(PLAYER_STATE.SHIFT);
                _rightTeam[1].Animator.SetBool("Won Game", _gameResult == 1 || _gameResult == 0);
                tempShiftState.SetLandingPosition(new Vector3(playerPos2.position.x, playerPos2.position.y, playerPos2.position.z));
                
                //_rightTeam[1].transform.position = playerPos2.position;
                //_rightTeam[1].transform.localScale = new Vector3(2f, 2f, 2f);
                //_rightTeam[1].SpriteRenderer.sortingOrder = 25;
            }

            _sequence++;
        }
    }

    public void FinishSequence() {
        FindObjectOfType<LevelManager>().ActivateFinalResultsScreen(_gameResult);
        leftText.gameObject.SetActive(true);
        rightText.gameObject.SetActive(true);
    }

    private void OnDestroy() {
        _matchEndMusic.release();
    }
}
