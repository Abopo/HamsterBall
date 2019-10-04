using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banner : MonoBehaviour {

    public int team;
    public SpriteRenderer banner;
    public SpriteRenderer end;

    bool _moving;
    float _moveSpd = -10f;

    Sprite[] _bannerSprites;

    GameEndSequence _gameEndSequence;

    private void Awake() {
        _gameEndSequence = transform.parent.GetComponent<GameEndSequence>();
    }
    // Start is called before the first frame update
    void Start() {
        _bannerSprites = Resources.LoadAll<Sprite>("Art/UI/Level UI/Victory-Lose-Test-Banners");
    }

    // Update is called once per frame
    void Update() {
        if(_moving) {
            transform.Translate(0f, _moveSpd * Time.deltaTime, 0f);
            if (transform.position.y < 0) {
                EndFall();
            } else if (transform.position.y < 6) {
                _gameEndSequence.ShiftSecondPlayers();
            } else if (transform.position.y < 10) {
                _gameEndSequence.ShiftFirstPlayers();
            } 
        }
    }

    public void SetSprites(bool won) {
        // Left Team
        if (team == 0) {
            if (won) {
                banner.sprite = _bannerSprites[6];
                end.sprite = _bannerSprites[7];
            } else {
                banner.sprite = _bannerSprites[4];
                end.sprite = _bannerSprites[5];
            }
        } else if(team == 1) {
            if (won) {
                banner.sprite = _bannerSprites[2];
                end.sprite = _bannerSprites[3];
            } else {
                banner.sprite = _bannerSprites[0];
                end.sprite = _bannerSprites[1];
            }
        }
    }

    public void StartFall() {
        _moving = true;
    }

    void EndFall() {
        _moving = false;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        _gameEndSequence.FinishSequence();
    }
}
