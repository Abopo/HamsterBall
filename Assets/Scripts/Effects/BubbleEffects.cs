using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script creates/manages various effects made when bubbles pop.
public class BubbleEffects : MonoBehaviour {

    // Team Combo effects
    GameObject _teamEffectObj;
    Sprite _teamComboSprite;
    Sprite _teamDropSprite;
    float _teamEffectTime = 1.0f;
    float _teamEffectTimer = 0.0f;

    // Counter effects
    GameObject _counterEffectObj;
    Sprite _counterMatchSprite;
    Sprite _counterDropSprite;
    float _counterEffectTime = 1.0f;
    float _counterEffectTimer = 0f;

    // Match Combo effects
    GameObject _comboEffectObj;
    Sprite[] _matchComboSprites;
    float _comboEffectTime = 1.0f;
    float _comboEffectTimer = 0.0f;

    // Multi Match effects
    GameObject _multiMatchEffectObj;

    // Bank shot effect
    GameObject _bankEffectObj;

    // Stock orbs
    public GameObject stockOrbGeneratorObj;
    int _index;
    HamsterMeter _hamsterMeter;

    // Bomb bubble explosion
    GameObject _bombExplosionObj;
    List<GameObject> _bombExplosionObjects = new List<GameObject>();

    //SFX
    AudioSource _teamAudioSource;
    AudioClip _teamComboClip;
    AudioClip _teamDropClip;

    // Use this for initialization
    void Start () {
        // Team effects
        _teamEffectObj = new GameObject();
        _teamEffectObj.SetActive(false);
        _teamEffectObj.AddComponent<SpriteRenderer>();
        _teamEffectObj.AddComponent<AudioSource>();
        _teamComboSprite = Resources.Load<Sprite>("Art/Effects/TeamCombo");
        _teamDropSprite = Resources.Load<Sprite>("Art/Effects/TeamDrop");

        _teamAudioSource = _teamEffectObj.GetComponent<AudioSource>();
        _teamAudioSource.volume = 0.5f;
        _teamComboClip = Resources.Load<AudioClip>("Audio/SFX/TeamCombo");
        _teamDropClip = Resources.Load<AudioClip>("Audio/SFX/TeamDrop");

        // Counter effects
        _counterEffectObj = new GameObject();
        _counterEffectObj.SetActive(false);
        _counterEffectObj.AddComponent<SpriteRenderer>();
        _counterEffectObj.AddComponent<AudioSource>();
        _counterMatchSprite = Resources.Load<Sprite>("Art/Effects/CounterMatch");
        _counterDropSprite = Resources.Load<Sprite>("Art/Effects/CounterDrop");

        // Multi Match effects
        _multiMatchEffectObj = new GameObject();
        _multiMatchEffectObj.SetActive(false);
        _multiMatchEffectObj.AddComponent<SpriteRenderer>();
        _multiMatchEffectObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/Effects/MultiMatch");
        _multiMatchEffectObj.AddComponent<AudioSource>();
        _multiMatchEffectObj.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Audio/SFX/BankShot");
        _multiMatchEffectObj.AddComponent<DestroyTimer>();

        // Match Combo effects
        _comboEffectObj = new GameObject();
        _comboEffectObj.SetActive(false);
        _comboEffectObj.AddComponent<SpriteRenderer>();
        _comboEffectObj.AddComponent<AudioSource>();
        _matchComboSprites = Resources.LoadAll<Sprite>("Art/Effects/MatchCombo");

        // Bank shot effect
        _bankEffectObj = new GameObject();
        _bankEffectObj.SetActive(false);
        _bankEffectObj.AddComponent<SpriteRenderer>();
        _bankEffectObj.AddComponent<DestroyTimer>();
        _bankEffectObj.AddComponent<AudioSource>();
        _bankEffectObj.GetComponent<AudioSource>().volume = 0.5f;
        _bankEffectObj.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Audio/SFX/BankShot");
        _bankEffectObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/Effects/BankShot");

        // Bomb bubble explosion effect
        _bombExplosionObj = new GameObject();
        _bombExplosionObj.AddComponent<SpriteRenderer>();
        _bombExplosionObj.AddComponent<Animator>();
        _bombExplosionObj.AddComponent<AudioSource>();
        _bombExplosionObj.SetActive(false);
        _bombExplosionObj.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Art/Animations/Effects/Explosion");
        _bombExplosionObj.GetComponent<AudioSource>().volume = 0.5f;
        _bombExplosionObj.GetComponent<AudioSource>().playOnAwake = true;
        _bombExplosionObj.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Audio/SFX/BombExplosion");

        _index = 0;
    }

    // Update is called once per frame
    void Update () {
        // Team effects
		if(_teamEffectObj.activeSelf) {
            _teamEffectTimer += Time.deltaTime;
            if(_teamEffectTimer >= _teamEffectTime) {
                _teamEffectObj.SetActive(false);
            }
        }

        // Counter effects
        if(_counterEffectObj.activeSelf) {
            _counterEffectTimer += Time.deltaTime;
            if(_counterEffectTimer >= _counterEffectTime) {
                _counterEffectObj.SetActive(false);
            }
        }

        // Match combo effects
        if(_comboEffectObj.activeSelf) {
            _comboEffectTimer += Time.deltaTime;
            if(_comboEffectTimer >= _comboEffectTime) {
                _comboEffectObj.SetActive(false);
            }
        }

        // bomb effects
        foreach (GameObject bomb in _bombExplosionObjects) {
            if (bomb.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {
                _bombExplosionObjects.Remove(bomb);
                DestroyObject(bomb);
                break;
            }
        }
    }

    public void TeamComboEffect(Vector2 position)
    {
        if (!_teamEffectObj.activeSelf) {
            _teamEffectObj.GetComponent<SpriteRenderer>().sprite = _teamComboSprite;
            _teamEffectObj.transform.position = new Vector3(position.x, position.y, -6);
            _teamEffectObj.SetActive(true);

            _teamAudioSource.clip = _teamComboClip;
            _teamAudioSource.Play();

            _teamEffectTimer = 0.0f;
        }
    }

    public void TeamDropEffect(Vector2 position)
    {
        if (!_teamEffectObj.activeSelf) {
            _teamEffectObj.GetComponent<SpriteRenderer>().sprite = _teamDropSprite;
            _teamEffectObj.transform.position = new Vector3(position.x, position.y, -6);
            _teamEffectObj.SetActive(true);

            _teamAudioSource.clip = _teamDropClip;
            _teamAudioSource.Play();

            _teamEffectTimer = 0.0f;
        }
    }

    public void CounterMatchEffect(Vector2 position) {
        if (!_counterEffectObj.activeSelf) {
            _counterEffectObj.GetComponent<SpriteRenderer>().sprite = _counterMatchSprite;
            _counterEffectObj.transform.position = new Vector3(position.x, position.y, -6);
            _counterEffectObj.SetActive(true);

            _teamAudioSource.clip = _teamComboClip;
            _teamAudioSource.Play();

            _counterEffectTimer = 0.0f;
        }
    }

    public void CounterDropEffect(Vector2 pos) {
        if (!_counterEffectObj.activeSelf) {
            _counterEffectObj.GetComponent<SpriteRenderer>().sprite = _counterDropSprite;
            _counterEffectObj.transform.position = new Vector3(pos.x, pos.y, -6);
            _counterEffectObj.SetActive(true);

            _teamAudioSource.clip = _teamDropClip;
            _teamAudioSource.Play();

            _counterEffectTimer = 0.0f;
        }
    }

    public void MultiMatchEffect(Vector2 pos) {
        Vector3 position = new Vector3(pos.x, pos.y, -5);
        GameObject mMatch = GameObject.Instantiate(_multiMatchEffectObj, position, Quaternion.identity);
        mMatch.SetActive(true);

        _multiMatchEffectObj.GetComponent<AudioSource>().Play();
    }

    public void MatchComboEffect(Vector2 pos, int combo) {
        if(!_comboEffectObj.activeSelf && combo >= 0) {
            if(combo > _matchComboSprites.Length-1) {
                combo = _matchComboSprites.Length - 1;
            }
            _comboEffectObj.GetComponent<SpriteRenderer>().sprite = _matchComboSprites[combo];
            _comboEffectObj.transform.position = new Vector3(pos.x, pos.y, -6);
            _comboEffectObj.SetActive(true);

            _teamAudioSource.clip = _teamComboClip;
            _teamAudioSource.Play();

            _comboEffectTimer = 0.0f;
        }
    }

    // The 'z' of pos determines facing; -1: left, 1: right
    public void BankShot(Vector3 pos)
    {
        Vector3 position = new Vector3(pos.x, pos.y, -5);
        GameObject bank = GameObject.Instantiate(_bankEffectObj, position, Quaternion.identity);
        bank.SetActive(true);
        if (pos.z == 1) {
            bank.transform.localScale = new Vector3(-1, 1, 1);
        } else if (pos.z == -1) {
            bank.transform.localScale = new Vector3(1, 1, 1);
        }

        _bankEffectObj.GetComponent<AudioSource>().Play();
    }

    public void StockOrbEffect(int spawnAmount, Vector3 pos) {
        // Create new StockOrbGenerator
        GameObject stockOrbGenerator = GameObject.Instantiate(stockOrbGeneratorObj, pos, Quaternion.identity);
        StockOrbGenerator soGenerator = stockOrbGenerator.GetComponent<StockOrbGenerator>();
        soGenerator.team = transform.parent.GetComponent<BubbleManager>().team;
        soGenerator.bubbleEffects = this;
        if (_hamsterMeter == null && soGenerator.team == 0) {
            _hamsterMeter = GameObject.FindGameObjectWithTag("BubbleManager2").GetComponent<BubbleManager>().hamsterMeter;
        } else if (_hamsterMeter == null && soGenerator.team == 1) {
            _hamsterMeter = GameObject.FindGameObjectWithTag("BubbleManager1").GetComponent<BubbleManager>().hamsterMeter;
        }

        soGenerator.BeginSpawning(spawnAmount, pos);
    }

    public void BombBubbleExplosion(Vector3 pos) {
        Vector3 position = new Vector3(pos.x, pos.y, -5);
        GameObject explosion = GameObject.Instantiate(_bombExplosionObj, position, Quaternion.identity);
        explosion.SetActive(true);
        _bombExplosionObjects.Add(explosion);
    }

    public Transform GetNextTallyPosition() {
        Transform t;

        t = _hamsterMeter.StockTallies[_index];

        _index++;
        if (_index >= _hamsterMeter.StockTallies.Count) {
            _index = 0;
        }

        return t;
    }

}
