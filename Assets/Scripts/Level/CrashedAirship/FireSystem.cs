using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSystem : MonoBehaviour {

    public FireButton fireButton;
    public FireHitbox fireHitbox;
    public ParticleSystem fireEffect;

    float _fireStartTime = 0.5f;
    float _fireStartTimer = 0f;
    bool _fireActive;
    float _fireTime = 2.0f;
    float _fireTimer = 0f;


    private void Awake() {
        fireButton = GetComponentInChildren<FireButton>();
        fireHitbox = GetComponentInChildren<FireHitbox>();
        fireEffect = GetComponentInChildren<ParticleSystem>();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (_fireActive) {
            _fireTimer += Time.deltaTime;
            if (_fireTimer >= _fireTime) {
                StopFire();
            }
        }
    }

    public void StartFire() {
        _fireActive = true;
        _fireTimer = 0f;

        fireEffect.Play();
        fireHitbox.FireStart();
    }
    void StopFire() {
        _fireActive = false;

        fireHitbox.FireEnd();
        fireButton.pressed = false;
    }

}
