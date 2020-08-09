﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bubble : MonoBehaviour {
	public HAMSTER_TYPES type;

	// Array of 6 adjacent bubbles.
	// 0 - top left, increases clockwise.
	public Bubble[] adjBubbles;
    public Node[] adjNodes;

    public List<Bubble> matches;
    public int numMatches;
	public int node = -1;
	public bool locked;
    public bool wasThrown;
	public bool popped;
	public bool isPlasma;
    public bool isIce;
    public SpriteRenderer iceSprite;

    public LayerMask checkMask;
    public bool canBeHit;
    public bool canCombo = true; // if this bubble can be part of a team combo

    public bool checkedForMatches;
	public bool checkedForAnchor;
	public bool foundAnchor = false;

    bool _adjBubbleCheck;

    public int team; // -1 = no team, 0 = left team, 1 = right team
    public int dropPotential; // A count of how many bubbles will be dropped if this bubble is matched

    public int is13Edge = 0; // If this bubble is on the edges of a 13 row. -1 = left; 0 = not; 1 = right

    public Animator bubbleAnimator;
    public Animator hamsterAnimator;

    private float _deltaX;
    private float _deltaY;

    private bool _dropCombo;
    private Vector3 _bankedPos; // position where this bubble banked off a wall
    BubblePopAnimation _popAnimation;

    Vector2 _velocity;
    float _airTime = 0f; // Time the bubble is in the air before hitting the board

    bool _popping = false;
    float _popTimer = 0f;
    int _popIndex = 0; // When in the match this bubble will pop
    float _popDelay = 0f; // Time to wait before popping
    int _popType; // Type of pop to do (normal, bomb, ice)
    public bool Popping {
        get { return _popping; }
    }

    public PlasmaEffect plasmaEffect;

    public FMOD.Studio.EventInstance BallBreakEvent;

    protected bool _petrified; // Only used during game end sequence
    public bool petrifying; // If this bubble is running the petrify coroutine

    BubbleManager _homeBubbleManager;
    GameManager _gameManager;
    Rigidbody2D _rigidbody;

    AudioSource _audioSource;
    AudioClip _iceClip;

    bool _destroy = false;
    bool _boardChanged = false;

	public FMOD.Studio.EventInstance BubbleDropEvent;
	public FMOD.Studio.EventInstance PetrifyEvent;
	public FMOD.Studio.EventInstance PetrifyBubbleEvent;

    PlayerController _playerController;
    public PlayerController PlayerController {
        get { return _playerController; }
        set { _playerController = value; }
    }

    public BubbleManager HomeBubbleManager {
        get { return _homeBubbleManager; }
        set { _homeBubbleManager = value; }
    }

    public bool Petrified {
        get { return _petrified; }
    }

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
        plasmaEffect = GetComponentInChildren<PlasmaEffect>();
    }
    // Use this for initialization
    void Start () {
		BubbleDropEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.BubbleDrop);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(BubbleDropEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());

        BallBreakEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.BallBreak);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(BallBreakEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }

    public void Initialize(HAMSTER_TYPES inType) {
		adjBubbles = new Bubble[6];
        adjNodes = new Node[6];
		
        _popAnimation = GetComponent<BubblePopAnimation>();
        _popAnimation.LoadPieces(inType);

        _gameManager = FindObjectOfType<GameManager>();

        _audioSource = GetComponent<AudioSource>();
        _iceClip = Resources.Load<AudioClip>("Audio/SFX/IceBreak");

        // If the type says this should be a gravity
        if ((int)inType >= (int)HAMSTER_TYPES.PLASMA) {
            type = inType - (int)HAMSTER_TYPES.PLASMA;
            SetPlasma(true);
        } else {
		    type = inType;
        }
        locked = false;
        wasThrown = false;
		popped = false;
		checkedForMatches = false;
		checkedForAnchor = false;
		foundAnchor = false;
        canBeHit = false;
        canCombo = true;
        _dropCombo = false;
        _bankedPos = Vector3.zero;

        SetType((int)type);

	}

    public void SetType(int inType) {
        type = (HAMSTER_TYPES)inType;
        bubbleAnimator.SetInteger("Type", inType);
        hamsterAnimator.SetInteger("Type", inType);
    }

    public void SetPlasma(bool plasma) {
        if (plasma && !isPlasma) {
            isPlasma = true;
            bubbleAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Art/Animations/Hamsters/AnimationObjects/Plasma/Plasma_Bubble");
            //GameObject spiralEffectInstance = Instantiate(spiralEffectObj, transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
            //spiralEffectInstance.transform.parent = transform;
            //spiralEffectInstance.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
        } else {
            isPlasma = false;
            bubbleAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Art/Animations/Hamsters/AnimationObjects/Bubble");
        }
    }

    public void SetIce(bool iced) {
        isIce = iced;
        iceSprite.enabled = iced;
    }

    // Update is called once per frame
    void Update () {

        if(_destroy && !_audioSource.isPlaying) {
            Destroy(gameObject);
        }

        if(_popping) {
            _popTimer += Time.deltaTime;
            if(_popTimer >= _popDelay) {
                switch(_popType) {
                    case 0:
                        Pop();
                        break;
                    case 1:
                        BombExplode();
                        break;
                    case 2:
                        SpecialPop();
                        break;
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.U)) {
            BoardChanged();
        }

    }

    private void FixedUpdate() {
        if (!locked) {
            // Move
            _deltaX = _velocity.x * Time.deltaTime;
            _deltaY = _velocity.y * Time.deltaTime;
            transform.Translate(_deltaX, _deltaY, 0.0f);
        }
    }

    private void LateUpdate() {
        if (locked && !(_popping || popped)) {
            // TODO: This really doesn't need to be happening every frame
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;

            //if (!foundAnchor && _homeBubbleManager.BoardIsStable) {
            //    Drop();
            //}

            //GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            checkedForMatches = false;
            checkedForAnchor = false;
            //foundAnchor = false;

            // Double check for adjBubbles, making sure it's never empty
            // (except for gravity because it's possible for them to be floating separately)
            if (!isPlasma) {
                _adjBubbleCheck = false;
                foreach (Bubble b in adjBubbles) {
                    if (b != null) {
                        _adjBubbleCheck = true;
                        break;
                    }
                }
                // If we somehow don't have ANY adj bubbles
                if (_adjBubbleCheck == false && _homeBubbleManager != null) {
                    _homeBubbleManager.AssignAdjBubbles(this, node);
                }
            }

            // Sync drop potential between matched bubbles
            if (_boardChanged) {
                foreach (Bubble b in matches) {
                    if (b != null && b.dropPotential > dropPotential) {
                        dropPotential = b.dropPotential;
                    }
                }
                _boardChanged = false;
            }
        } else {
            // Count time from throw to land
            if (wasThrown) {
                _airTime += Time.deltaTime;
            }
        }
    }

    // Started by anchor bubbles that tell all connected bubbles that they have an anchor
    // Runs recursively through adjBubbles
    public void SetAnchors() {
        foundAnchor = true;

        if(plasmaEffect.active) {
            plasmaEffect.Deactivate();
        }

        foreach(Bubble bub in adjBubbles) {
            if(bub != null && !bub.IsAnchorPoint() && !bub.foundAnchor) {
                bub.SetAnchors();
            }
        }
    }

    // This does the same as above but also turns on the plasma effect
    public void PlasmaAnchor(int type) {
        plasmaEffect.Activate(type);
        foundAnchor = true;

        foreach(Bubble bub in adjBubbles) {
            if(bub != null && !bub.foundAnchor) {
                bub.PlasmaAnchor(type);
            }
        }
    }

    public void DropCheck() {
        if(!foundAnchor) {
            Drop();
        }
    }

    /* Check for anchor old
    public void CheckForAnchor(List<Bubble> bubbles) {
		for (int i = 0; i < 6; ++i) {
			if(adjBubbles[i] != null && !adjBubbles[i].popped) {
				if(!adjBubbles[i].checkedForAnchor && !adjBubbles[i].foundAnchor) {
					adjBubbles[i].checkedForAnchor = true;
					bubbles.Add(adjBubbles[i]);
					if(adjBubbles[i].IsAnchorPoint()) {
                        // Plasmas should ignore other plasmas when searching for an anchor
                        if(isPlasma && adjBubbles[i].isPlasma) {
                            continue;
                        }

						foundAnchor = true;
						foreach(Bubble b in bubbles) {
							b.foundAnchor = true;
						}
					} else {
						adjBubbles[i].CheckForAnchor(bubbles);
					}
				} else if(adjBubbles[i].foundAnchor) {
					foundAnchor = true;
					foreach(Bubble b in bubbles) {
						b.foundAnchor = true;
					}
				}
			}
		}

        // If we're a plasma and haven't found a ceiling anchor
        if(isPlasma && !foundAnchor) {
            // Then we are floating and should turn on the plasma effect for all bubbles connected to us
            plasmaEffect.PlasmaEffectStart();
        }

		// way too many extra checks to super omega make sure we don't drop when we shouldn't
		if (IsAnchorPoint ()) {
			foundAnchor = true;
			foreach(Bubble b in bubbles) {
				b.foundAnchor = true;
			}
		} else if (foundAnchor) {
			foreach(Bubble b in bubbles) {
				b.foundAnchor = true;
			}
		} else {
			foreach(Bubble b in bubbles) {
				if(b.foundAnchor) {
					foundAnchor = true;
				}
			}

			if(foundAnchor) {
				foreach(Bubble b in bubbles) {
					b.foundAnchor = true;
				}
			}
		}

		if (!foundAnchor) {
			foreach(Bubble b in bubbles) {
				b.Drop();
			}
			Drop ();
		}
	}
    */

    // This overload is used to check whether or not this bubble can be dropped by popping the excludeBubble
    public bool CheckForAnchor(List<Bubble> bubbles, List<Bubble> excludedBubbles) {
        for (int i = 0; i < 6; ++i) {
            if (adjBubbles[i] != null && !adjBubbles[i].popped && !excludedBubbles.Contains(adjBubbles[i])) {
                if (!adjBubbles[i].checkedForAnchor && !adjBubbles[i].foundAnchor) {
                    adjBubbles[i].checkedForAnchor = true;
                    bubbles.Add(adjBubbles[i]);
                    if (adjBubbles[i].IsAnchorPoint()) {
                        foundAnchor = true;
                        foreach (Bubble b in bubbles) {
                            b.foundAnchor = true;
                        }
                    } else {
                        adjBubbles[i].CheckForAnchor(bubbles, excludedBubbles);
                    }
                } else if (adjBubbles[i].foundAnchor) {
                    foundAnchor = true;
                    foreach (Bubble b in bubbles) {
                        b.foundAnchor = true;
                    }
                }
            }
        }

        return foundAnchor;
    }

    public void ClearAdjBubbles() {
		for(int i = 0; i < 6; ++i) {
			adjBubbles[i] = null;
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Wall" && Mathf.Abs(_velocity.x) > 0.1f) {
            // Make sure bubbles only bounce off of walls they are moving towards
            if(_velocity.x > 0 && other.transform.position.x > transform.position.x || 
                _velocity.x < 0 && other.transform.position.x < transform.position.x) {
                // Bounce the bubble the opposite direction
                //BOUNCE AUDIO
                _velocity = new Vector2(_velocity.x * -1, _velocity.y);

				FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.WallBounce);
                // Banked combo effect stuff
                _bankedPos = transform.position;
                // Check which side we collided on
                if (other.transform.position.x < transform.position.x) {
                    // Left side
                    _bankedPos.z = -1;
                } else /*if(other.transform.position.x > transform.position.x)*/ {
                    // Right side
                    _bankedPos.z = 1;
                }

                // Earn some points for a wall bounce
                _playerController.HomeBubbleManager.IncreaseScore(20);
            }
        }
		if (other.gameObject.tag == "Bubble") {
			if(!locked && other.transform.GetComponent<Bubble>().locked) {
                if (!PhotonNetwork.connectedAndReady || (GetComponent<PhotonView>().owner == PhotonNetwork.player)) {
                    CollisionWithBoard(other.transform.GetComponent<Bubble>()._homeBubbleManager);
                } else {
                    // Stop moving and sit in place.
                    _velocity = Vector2.zero;
                    locked = true;
                }
            }
		}
        if(other.gameObject.tag == "Ceiling" && !locked) {
            CollisionWithBoard(other.transform.GetComponent<Ceiling>().bubbleManager);
        }
        if (other.gameObject.tag == "Bottom") {
            if (type == HAMSTER_TYPES.SKULL) {
                int inc = 1 * (_dropCombo ? 3 : 1);

                // Calculate score before the margin multiplier
                int incScore = inc * 100;
                _homeBubbleManager.IncreaseScore(incScore);

                // If we are not playing single player or team survival
                if (!_gameManager.isSinglePlayer && _gameManager.gameMode != GAME_MODE.TEAMSURVIVAL) {
                    GenerateDropJunk(inc);
                }
            } else {
                int inc = 2 * (_dropCombo ? 3 : 1);
                
                // Calculate score before the margin multiplier
                int incScore = inc * 100;
                _homeBubbleManager.IncreaseScore(incScore);

                // If we are not playing single player or team survival
                if (!_gameManager.isSinglePlayer && _gameManager.gameMode != GAME_MODE.TEAMSURVIVAL) {
                    GenerateDropJunk(inc);
                }
            }

            // Remove self from bubble manager
            //_homeBubbleManager.RemoveBubble(node);

            _destroy = true;
		}
	}

    void GenerateDropJunk(int amount) {
        // Multiply by the Margin Multiplier
        amount = (int)(amount * GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().marginMultiplier);

        if (_playerController != null) {
            // Add the player's atk modifier
            amount = (amount + _playerController.atkModifier);
        }

        // Start stock orb effect
        _homeBubbleManager.BubbleEffects.StockOrbEffect(amount, transform.position);
    }

    public void CollisionWithBoard(BubbleManager bubbleManager) {
        // Stop moving and sit in place.
        _velocity = Vector2.zero;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        gameObject.layer = LayerMask.NameToLayer("SolidBubble");
        locked = true;

        //Debug.Log("connect with board");

        // Make super sure the bubble is visible
        DisplaySprites();

        // Remove the held bubble of the player controller
        if (_playerController != null) {
            _playerController.heldBall = null;
        }

        // Set our team and home bubble manager
        _homeBubbleManager = bubbleManager;
        team = _homeBubbleManager.team;

        _homeBubbleManager.boardChangedEvent.AddListener(BoardChanged);

        if (!PhotonNetwork.connectedAndReady) {
            _homeBubbleManager.AddBubble(this);
        } else if (GetComponent<PhotonView>().owner == PhotonNetwork.player) { // if we own this bubble
            _homeBubbleManager.AddBubble(this);
            GetComponent<PhotonView>().RPC("AddToBoard", PhotonTargets.Others, team, node);
        }
     
        // Loop through adjbubbles to see if we connected with a matching color
        bool sameType = false;
        foreach (Bubble bub in adjBubbles) {
            if(bub != null && bub.type == type) {
                // Play same color connect sound
                sameType = true;
                break;
            }
        }

        if (sameType) {
            // Play same color connect sound
			FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.HamsterConnectSameColor);
		} else if(type == HAMSTER_TYPES.RAINBOW){
			FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.HamsterConnectRainbow);
		} else if(type == HAMSTER_TYPES.SKULL){
			FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.HamsterConnectSkull);
		}
		else {
            // Play normal connect sound
            FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.HamsterConnect);
        }

        // Check if was a LongShot
        //Debug.Log(_airTime);
        if (_airTime > 0.76f) {
            // Increase score
            _playerController.HomeBubbleManager.IncreaseScore(200);

            // Show longshot effect
        }

        // Check Matches
        if (type == HAMSTER_TYPES.RAINBOW) {
            DoCrazyRainbowMatches();
            type = HAMSTER_TYPES.RAINBOW;
			FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.HamsterConnectRainbow);
			Debug.Log("Rainbow Hamster Connect");
        } else if (type == HAMSTER_TYPES.BOMB) {
            // Explode
            BombExplode();
            
        } else {
            matches = new List<Bubble>();
            matches = CheckMatches(matches);
            numMatches = matches.Count;

            // After matches are calculated, but before anything is popped,
            // check if any adjBubbles are a bomb
            for(int i = 0; i < 6; ++i) {
                if(adjBubbles[i] != null && adjBubbles[i].type == HAMSTER_TYPES.BOMB && !adjBubbles[i].isIce) {
                    adjBubbles[i].BombExplode();
                }
            }

            if (matches.Count >= 3) {
                //_homeBubbleManager.IncreaseComboCounter(transform.position);
                HandleMatch(matches);
                _homeBubbleManager.matchCount++;
                Pop();
            }
        }

        // Set 3d attributes for sound effects
        BubbleDropEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, _rigidbody));
        BallBreakEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, _rigidbody));

        _homeBubbleManager.LastBubbleAdded = this;
    }

    void DoCrazyRainbowMatches() {
		// Do crazy raindbow checking for each color.
		List<Bubble> greenMatches = new List<Bubble>();
		List<Bubble> redMatches = new List<Bubble>();
		List<Bubble> orangeMatches = new List<Bubble>();
		List<Bubble> grayMatches = new List<Bubble>();
		List<Bubble> blueMatches = new List<Bubble>();
		List<Bubble> pinkMatches = new List<Bubble>();
        List<Bubble> purpleMatches = new List<Bubble>();

        // Green checks
        type = HAMSTER_TYPES.GREEN;
		greenMatches = CheckMatches(greenMatches);
		checkedForMatches = false;
		_homeBubbleManager.RefreshRainbowBubbles ();
		// Red checks
		type = HAMSTER_TYPES.RED;
		redMatches = CheckMatches(redMatches);
		checkedForMatches = false;
		_homeBubbleManager.RefreshRainbowBubbles ();
		// Orange checks
		type = HAMSTER_TYPES.YELLOW;
		orangeMatches = CheckMatches(orangeMatches);
		checkedForMatches = false;
		_homeBubbleManager.RefreshRainbowBubbles ();
		// Gray checks
		type = HAMSTER_TYPES.GRAY;
		grayMatches = CheckMatches(grayMatches);
		checkedForMatches = false;
		_homeBubbleManager.RefreshRainbowBubbles ();
		// Blue checks
		type = HAMSTER_TYPES.BLUE;
		blueMatches = CheckMatches(blueMatches);
		checkedForMatches = false;
		_homeBubbleManager.RefreshRainbowBubbles ();
		// Pink checks
		type = HAMSTER_TYPES.PINK;
		pinkMatches = CheckMatches(pinkMatches);
		checkedForMatches = false;
		_homeBubbleManager.RefreshRainbowBubbles ();
        // Purple checks
        type = HAMSTER_TYPES.PURPLE;
        purpleMatches = CheckMatches(purpleMatches);
        checkedForMatches = false;
        _homeBubbleManager.RefreshRainbowBubbles();

        bool matched = false;
        List<int> colorMatches = new List<int>();
		if (CheckRainbowMatchCount (greenMatches)) {
			matched = true;
            colorMatches.Add(greenMatches.Count);
		}
		if (CheckRainbowMatchCount (redMatches)) {
			matched = true;
            colorMatches.Add(redMatches.Count);
        }
        if (CheckRainbowMatchCount (orangeMatches)) {
			matched = true;
            colorMatches.Add(orangeMatches.Count);
        }
        if (CheckRainbowMatchCount (grayMatches)) {
			matched = true;
            colorMatches.Add(grayMatches.Count);
        }
        if (CheckRainbowMatchCount (blueMatches)) {
			matched = true;
            colorMatches.Add(blueMatches.Count);
        }
        if (CheckRainbowMatchCount (pinkMatches)) {
			matched = true;
            colorMatches.Add(pinkMatches.Count);
        }
        if (CheckRainbowMatchCount (purpleMatches)) {
            matched = true;
            colorMatches.Add(purpleMatches.Count);
        }

        if(colorMatches.Count > 1) {
            // We have a multi-match!
            // Find the smallest match
            int smallestMatch = 100;
            foreach(int count in colorMatches) {
                if (count < smallestMatch) {
                    smallestMatch = count;
                }
            }

            // Double the garbage generated by the smallest match
            GenerateGarbage(smallestMatch, 0);

            // Show multi-match effect
            _homeBubbleManager.BubbleEffects.MultiMatchEffect(transform.position);
        }

        if (matched) {
			Pop();
		}
	}

	public List<Bubble> CheckMatches(List<Bubble> matches) {
		for (int i = 0; i < 6; ++i) {
			if(adjBubbles[i] != null && adjBubbles[i].type != HAMSTER_TYPES.SKULL && adjBubbles[i].type != HAMSTER_TYPES.BOMB && !adjBubbles[i].isIce) {
                if (adjBubbles[i].type == type) {
					if(!adjBubbles[i].checkedForMatches) {
						adjBubbles[i].checkedForMatches = true;
						matches.Add(adjBubbles[i]);
						adjBubbles[i].CheckMatches(matches);
					}
				} else if(adjBubbles[i].type == HAMSTER_TYPES.RAINBOW) {
					adjBubbles[i].type = type;
					if(!adjBubbles[i].checkedForMatches) {
						adjBubbles[i].checkedForMatches = true;
						matches.Add(adjBubbles[i]);
						adjBubbles[i].CheckMatches(matches);
					}
					adjBubbles[i].type = HAMSTER_TYPES.RAINBOW;
				}
			}
		}

        //numMatches = matches.Count;

		return matches;
	}

	bool CheckRainbowMatchCount(List<Bubble> matches) {
		bool matched = false;

		if(matches.Count >= 3) {
			matched = true;
            HandleMatch(matches);
		}

		return matched;
	}

    void HandleMatch(List<Bubble> matches) {
        int comboBonus = CheckCombos(matches);

        GenerateGarbage(matches.Count, comboBonus);

        // Pop matches
        float pDelay = 0.1f;
        int pIndex = 1;
        foreach (Bubble b in matches) {
            if (b != this) {
                if (b.type == HAMSTER_TYPES.BOMB && !b.popped) {
                    b.StartPop(pIndex, pDelay, 1);
                } else {
                    b.StartPop(pIndex, pDelay, 0);
                }
            }
            pDelay += 0.1f;
            pIndex += 1;
        }
    }

    void GenerateGarbage(int matchCount, int comboBonus) {
        // Calculate amount of garbage to generate
        //int garbageCount = (int)Mathf.Pow((matchCount - 2), 2); // 3 = 1, 4 = 4, 5 = 9, 6 = 16, 7 = 25, 8 = 36
        int garbageCount = 3;
        for (int i = 0; i <= matchCount-3; ++i) {
            garbageCount += (3 * i) + (i - 1); // 3 = 2, 4 = 5, 5 = 12, 6 = 23, 7 = 38, 8 = 57
        }
        
        // Calculate score before the margin multiplier
        int incScore = (garbageCount + comboBonus) * 100;
        _homeBubbleManager.IncreaseScore(incScore);

        // Multiply by the Margin Multiplier
        garbageCount = (int)((garbageCount + comboBonus) * GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().marginMultiplier);

        // If this bubble was thrown by a player
        if (_playerController != null) {
            // Multiply by the player's atk multiplier
            garbageCount = (garbageCount + _playerController.atkModifier);
        }

        // If the combo count is high enough
        // TODO: Although I like it as an idea, this combo stuff seems way too powerful and easy to do.
        //if (_homeBubbleManager.ComboCount > 0) {
        // Multiply based on the combo
        //garbageCount += 2 * _homeBubbleManager.ComboCount;
        //Debug.Log("Comboed " + _homeBubbleManager.ComboCount);
        //}

        // If we are not playing single player or team survival
        if (!_gameManager.isSinglePlayer && _gameManager.gameMode != GAME_MODE.TEAMSURVIVAL) {
            // Start stock orb effect
            _homeBubbleManager.BubbleEffects.StockOrbEffect(garbageCount, transform.position);
        }
    }

    int CheckCombos(List<Bubble> matches) {
        int comboBonus = 0;
        // Check combo stuff
        foreach (Bubble b in matches) {
            // if either bubble is missing it's player controller, move on to the next one.
            if(_playerController == null || b.PlayerController == null) {
                continue;
            }

            // If the last bubble added has matched and wasn't thrown by our thrower
            if (_homeBubbleManager.team == _playerController.team && b.canCombo && b == _homeBubbleManager.LastBubbleAdded && b.PlayerController != _playerController) {
                // If a different team threw the bubble
                if (b.PlayerController.team != _playerController.team) {
                    // Then it's a counter!
                    comboBonus += 5;
                    _homeBubbleManager.BubbleEffects.CounterMatchEffect(transform.position);
                } else {
                    // Team combo!
                    comboBonus += 4;
                    _homeBubbleManager.BubbleEffects.TeamComboEffect(transform.position);
                }
            }
        }

        // If this bubble had banked off a wall before matching
        if(_bankedPos.x != 0 && _bankedPos.y != 0) {
            comboBonus += 2;
            _homeBubbleManager.BubbleEffects.BankShot(_bankedPos);
			FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.WallBounceSuccess);
        }

        return comboBonus;
    }

    public void StartPop(int pIndex, float pDelay, int pType) {
        _popIndex = pIndex;
        _popDelay = pDelay;
        _popType = pType;
        _popping = true;
    }

    public void Pop() {
		_homeBubbleManager.RemoveBubble (node);
		popped = true;
        _popping = false;

        if (_popIndex <= 10){
        	BallBreakEvent.setParameterValue("PoppedCount", _popIndex);
        } else {
        	BallBreakEvent.setParameterValue("PoppedCount", 10);
        }

        if (_popIndex > 7)
        {
            FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.CrowdLarge1);
        }
        else if (_popIndex <= 7 && _popIndex >= 4)
        {
            FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.CrowdMedium1);
        }
        else if (_popIndex < 4)
        {
            FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.CrowdSmall1);
        }

        //Debug.Log("PopIndex " + _popIndex);
        BallBreakEvent.start();
		BallBreakEvent.release();

		//Debug.Log("Pop");
        // Instaed of destroying, do a nice animation of the bubble opening.
        _popAnimation.Pop(false);

        if(plasmaEffect.active) {
            plasmaEffect.Deactivate();
        }

        // Run through adjBubbles
        foreach(Bubble b in adjBubbles) {
            if (b != null) {
                // Remove self from their adjBubbles
                for(int i = 0; i < 6; ++i) {
                    if(b.adjBubbles[i] == this) {
                        b.adjBubbles[i] = null;
                    }
                }

                // If iced, break
                if (b.isIce) {
                    b.BreakIce();
                }
                // If special, pop if
                if (b.type == HAMSTER_TYPES.SPECIAL) {
                    b.StartPop(0, 0, 2);
                }
            }
        }
	}

    public void SpecialPop() {
        _homeBubbleManager.RemoveBubble(node);
        popped = true;
        _popping = false;

        BallBreakEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.BallBreak);
        BallBreakEvent.start();
        BallBreakEvent.release();

        Debug.Log("Special Pop!");

        // Instead of destroying, do a nice animation of the bubble opening.
        _popAnimation.Pop(true);
    }

    public void Drop() {
        if(IsAnchorPoint()) {
            Debug.LogError("Tried to drop anchor bubble, bail");
            return;
        }

		locked = false;
        _velocity = new Vector2 (0.0f, -10f);
		gameObject.layer = 15; // GhostBubble

        if (plasmaEffect.active) {
            plasmaEffect.Deactivate();
        }

        // If both bubbles player controllers exist
        if (_playerController != null && _homeBubbleManager.LastBubbleAdded.PlayerController != null) {
            // Check for drop combos
            // If this bubble can combo, a local player threw the bubble, the bubble causing the drop was the last bubble thrown, and it was thrown by a different player
            if (canCombo && _homeBubbleManager.LastBubbleAdded.PlayerController.team == _homeBubbleManager.team && _homeBubbleManager.LastBubbleAdded != null &&
                _homeBubbleManager.LastBubbleAdded.popped && _playerController != null && _homeBubbleManager.LastBubbleAdded.PlayerController != _playerController) {
                // If this bubble was thrown by an opponent
                if (_playerController.team != _homeBubbleManager.team) {
                    // Then it's a counter!
                    _homeBubbleManager.BubbleEffects.CounterDropEffect(transform.position);
                    _dropCombo = true;
                } else {
                    // It's a combo
                    _homeBubbleManager.BubbleEffects.TeamDropEffect(transform.position);
                    _dropCombo = true;
                }
            }
        }

        // Remove self from bubble manager
        _homeBubbleManager.RemoveBubble(node);
    }

    public bool IsAnchorPoint() {
		if (node < _homeBubbleManager.TopLineLength) {
			return true;
		}

		return false;
	}

    public void BombExplode() {
        // Add score for how many bubbles were blown up
        _homeBubbleManager.IncreaseScore(adjBubbles.Length * 120);
        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.HamsterConnectBomb);
        // Get the iced data from the adjBubbles before exploding
        bool[] icedBubbles = new bool[6];
        for(int i = 0; i < 6; ++i) {
            if(adjBubbles[i] != null) {
                icedBubbles[i] = adjBubbles[i].isIce;
            }
        }

        // Pop self and all adjBubbles
        Pop();

        for (int i = 0; i < 6; ++i) {
            if (adjBubbles[i] != null) {
                // If this bubble was iced before this
                if (icedBubbles[i]) {
                    // Break the ice
                    BreakIce();
                } else if (adjBubbles[i].type == HAMSTER_TYPES.BOMB && !adjBubbles[i].popped) {
                    adjBubbles[i].StartPop(i, 0.1f, 1);
                } else {
                    adjBubbles[i].Pop();
                }
            }
        }


        _homeBubbleManager.BubbleEffects.BombBubbleExplosion(transform.position);
    }

    public void BreakIce() {
        if (isIce) {
            // TODO: Some ice breaking animation

            // Play sound effect
            PlayIceClip();

            SetIce(false);
        }
    }

    // Is called when the board is changed
    void BoardChanged() {
        dropPotential = 0;

        // If we aren't the last bubble added
        if(_homeBubbleManager != null && this != _homeBubbleManager.LastBubbleAdded) {
            canCombo = false;
        }

        UpdateRigidbodyStatus();

        _boardChanged = true;
    }

    void UpdateRigidbodyStatus() {
        // We don't need to update rigidbodies on bubbles that are impossible to hit
        // So If we have no open adj nodes set rigidbody to kinematic
        foreach (Node node in adjNodes) {
            if (node != null && node.bubble != null) {
                // We have an open node so turn on rigidbody
                _rigidbody.isKinematic = false;
                return;
            }
        }

        // We had no open nodes so sleep the rigidbody
        _rigidbody.isKinematic = true;
    }

    // This function scans around the bubble to check whether or not it's possible to be hit by the player.
    bool CanBeHit() {
        canBeHit = false;

        RaycastHit2D hit;
        Vector2 rayDir;

        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        int hitCount = 0;
        Vector2 origin;
        for (int i = 0; i < 4; ++i) {
            hitCount = 0;

            for (float j = -0.2f; j < 0.4f; j += 0.2f) {
                rayDir = new Vector2(-1 + (i * 0.45f)+j/2, -1);
                origin = new Vector2(transform.position.x + j, transform.position.y);
                hit = Physics2D.Raycast(origin, rayDir, 10f, checkMask);
                if (hit && hit.transform.tag == "Platform") {
                    hitCount++;
                    //Debug.DrawRay(origin, rayDir * hit.distance);
                }
            }

            if(hitCount > 2) {
                canBeHit = true;
                break;
            }
        }

        gameObject.layer = LayerMask.NameToLayer("SolidBubble");

        return canBeHit;
    }

    // This is just used to reduce the number of bubbles that go through the dropPotential check,
    // it's not designed to be perfectly accurate
    public bool CouldMaybeBeHit() {
        if(adjBubbles[2] == null || adjBubbles[3] == null || adjBubbles[4] == null || adjBubbles[5] == null) {
            return true;
        }

        return false;
    }

    public void CheckDropPotential() {
        dropPotential = 0;

        // Make sure matches at least contains self
        if (matches.Count == 0) {
            matches.Add(this);
        }

        // save the anchor data
        bool[] anchorData = new bool[_homeBubbleManager.Bubbles.Length];
        for(int i = 0; i < _homeBubbleManager.Bubbles.Length; ++i) {
            if(_homeBubbleManager.Bubbles[i] != null) {
                anchorData[i] = _homeBubbleManager.Bubbles[i].foundAnchor;
            }
        }

        // go through each adjBubble, checking to see if this bubble would drop without it
        List<Bubble> bubbles = new List<Bubble>();
        foreach (Bubble b in adjBubbles) {
            // Reset variables for next bubble
            // TODO: This is super inefficient, but so far is the only way to be 100% accurate
            foreach (Bubble lB in _homeBubbleManager.Bubbles) {
                if (lB != null) {
                    lB.checkedForAnchor = false;
                    lB.foundAnchor = false;
                    lB.checkedForMatches = false;
                }
            }
            bubbles.Clear();

            // If a bubble can't find an anchor without the matchedBubbles
            // matches aren't set up for inital bubbles
            if (b != null && !matches.Contains(b)) {
                if (!b.CheckForAnchor(bubbles, matches)) {
                    // Then add weight based on how many bubbles would be dropped
                    dropPotential = DropCount(b, matches);
                }
            }
        }

        // return bubbles to original anchor data
        for(int i = 0; i < _homeBubbleManager.Bubbles.Length; ++i) {
            if(_homeBubbleManager.Bubbles[i] != null) {
                _homeBubbleManager.Bubbles[i].foundAnchor = anchorData[i];
            }
        }
    }

    // Counts how many bubbles will be dropped along with the inBubble
    int DropCount(Bubble bub, List<Bubble> matchedBubbles) {
        int dropCount = 1;
        bub.checkedForMatches = true;

        foreach (Bubble b in bub.adjBubbles) {
            if (b != null && !b.checkedForMatches && !matchedBubbles.Contains(b)) {
                //dropCount++;
                b.checkedForMatches = true;
                dropCount += DropCount(b, matchedBubbles);
            }
        }

        return dropCount;
    }

    // Petrification sequence happens when a player loses a round, bubbles are petrified starting from the bubble(s) that lost the round
    public IEnumerator Petrify() {
        petrifying = true;
        _petrified = true;

        // Set the type to Gray
        bubbleAnimator.SetInteger("Type", 3);

        // Darken bubble and hamster sprite 
        bubbleAnimator.GetComponent<SpriteRenderer>().color = new Color(0.63f, 0.63f, 0.63f);
        hamsterAnimator.GetComponent<SpriteRenderer>().color = new Color(0.63f, 0.63f, 0.63f);

		PetrifyBubbleEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.PetrifyBubble);
		PetrifyBubbleEvent.start();
		PetrifyBubbleEvent.release();

		PetrifyEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.Petrify);
		PetrifyEvent.start();
		//PetrifyEvent.release();

        yield return new WaitForSeconds(0.2f);

        bool foundUnpetrifiedBubble = false;
        // Petrify all adjacent bubbles
        foreach(Bubble bub in adjBubbles) {
            if(bub != null && !bub._petrified) {
                StartCoroutine(bub.Petrify());
                foundUnpetrifiedBubble = true;
            }
        }

        // If we didn't find any unpetrified bubbles
        if (!foundUnpetrifiedBubble) {
            // If this bubble is a ceiling bubble, make sure to check for other ceiling bubbles (and plasmas)
            if (IsAnchorPoint()) {
                // find all other anchor points and petrify them
                foreach(Bubble bubble in _homeBubbleManager.Bubbles) {
                    if(bubble != null && !bubble._petrified && bubble.IsAnchorPoint()) {
                        StartCoroutine(bubble.Petrify());
                    }
                }
            }
        }

        petrifying = false;
    }

    // Miscelleanous functions

    void PlayIceClip() {
        if (!_audioSource.isPlaying) {
            _audioSource.clip = _iceClip;
            _audioSource.Play();
        }
    }

    public void Throw(float speed, Vector2 dir) {
        _velocity = new Vector2(speed * dir.x, speed * dir.y);
        GetComponent<CircleCollider2D>().enabled = true;
        wasThrown = true;
    }

    public void HideSprites() {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites) {
            sprite.enabled = false;
        }
    }
    public void DisplaySprites() {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer sprite in sprites) {
            sprite.enabled = true;
        }

        // Except for the ice sprite
        iceSprite.enabled = false;
    }

    public bool IsSpecialType() {
        if(type == HAMSTER_TYPES.SKULL || type == HAMSTER_TYPES.RAINBOW) {
            return true;
        }

        return false;
    }

    public void AddForce(Vector2 force) {
        _velocity += force;
    }
}