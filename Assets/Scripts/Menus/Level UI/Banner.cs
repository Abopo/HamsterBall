using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banner : MonoBehaviour {

    public int team;
    public SpriteRenderer banner;

    bool _moving;
    float _moveSpd = -10f;

    Sprite[] _bannerSprites;

    GameEndSequence _gameEndSequence;

    private void Awake() {
        _gameEndSequence = transform.parent.GetComponent<GameEndSequence>();
    }
    // Start is called before the first frame update
    void Start() {
        _bannerSprites = Resources.LoadAll<Sprite>("Art/UI/Level UI/Victory-Lose-Banners");
    }

    // Update is called once per frame
    void Update() {
        if(_moving) {
            transform.Translate(0f, _moveSpd * Time.deltaTime, 0f);
            if (transform.localPosition.y < 3.75f) {
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
                banner.sprite = _bannerSprites[3];
            } else {
                banner.sprite = _bannerSprites[2];
            }
        } else if(team == 1) {
            if (won) {
                banner.sprite = _bannerSprites[1];
            } else {
                banner.sprite = _bannerSprites[0];
            }
        }
    }

    public void StartFall() {
        _moving = true;
    }

    void EndFall() {
        _moving = false;
        transform.position = new Vector3(transform.position.x, 3.75f, transform.position.z);

        _gameEndSequence.FinishSequence();
    }
}
