using UnityEngine;
using System.Collections.Generic;

public class Bubble : MonoBehaviour {
	public HAMSTER_TYPES type;

	// Array of 6 adjacent bubbles.
	// 0 - top left, increases clockwise.
	public Bubble[] adjBubbles;

    public List<Bubble> matches;
    public int numMatches;
	public int node = -1;
	public bool locked;
    public bool wasThrown;
	public bool popped;
	public bool isGravity;

    public LayerMask checkMask;
    public bool canBeHit;

	public bool checkedForMatches;
	public bool checkedForAnchor;
	public bool foundAnchor = false;

    public int team; // -1 = no team, 0 = left team, 1 = right team

    public int is13Edge = 0; // If this bubble is on the edges of a 13 row. -1 = left; 0 = not; 1 = right

    private float _deltaX;
    private float _deltaY;

    private bool _dropCombo;
    private Vector3 _bankedPos; // position where this bubble banked off a wall
    BubblePopAnimation _popAnimation;

	BubbleManager _homeBubbleManager;
    Rigidbody2D _rigibody2D;

    AudioSource _audioSource;
    AudioClip _dropClip;
    bool _destroy = false;

    PlayerController _playerController;
    public PlayerController PlayerController {
        get { return _playerController; }
        set { _playerController = value; }
    }

    public BubbleManager HomeBubbleManager {
        get { return _homeBubbleManager; }
    }

    // Use this for initialization
    void Start () {
	}
	
	public void Initialize(HAMSTER_TYPES inType) {
		adjBubbles = new Bubble[6];

        _popAnimation = GetComponent<BubblePopAnimation>();
        _popAnimation.LoadPieces(inType);

		if(team == 0) {
			_homeBubbleManager = GameObject.FindGameObjectWithTag ("BubbleManager1").GetComponent<BubbleManager>();
		} else if (team == 1) {
			_homeBubbleManager = GameObject.FindGameObjectWithTag ("BubbleManager2").GetComponent<BubbleManager>();
		}

        _rigibody2D = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        _dropClip = Resources.Load<AudioClip>("Audio/SFX/Hamster_Fall2");

		type = inType;
		locked = false;
        wasThrown = false;
		popped = false;
		checkedForMatches = false;
		checkedForAnchor = false;
		foundAnchor = false;
        canBeHit = false;
        _dropCombo = false;
        _bankedPos = Vector3.zero;

        SetType((int)type);
	}

    public void SetType(int inType) {
        type = (HAMSTER_TYPES)inType;
        GetComponent<Animator>().SetInteger("Type", inType);
        transform.GetChild(0).GetComponent<Animator>().SetInteger("Type", inType);
    }

    // Update is called once per frame
    void Update () {
        if(_destroy && !_audioSource.isPlaying) {
            DestroyObject(this.gameObject);
        }

        if (locked) {
			GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			checkedForMatches = false;
			checkedForAnchor = false;
			foundAnchor = false;
            // TODO: Only run this when the board changes.
            // CanBeHit();
		} else {
            // Move
            //Debug.Log(_rigibody2D.velocity);
            if (_rigibody2D != null) {
                _deltaX = _rigibody2D.velocity.x * Time.deltaTime;
                _deltaY = _rigibody2D.velocity.y * Time.deltaTime;
                transform.Translate(_deltaX, _deltaY, 0.0f);
            } else {
                _rigibody2D = GetComponent<Rigidbody2D>();
            }
        }
	}

	public void CheckForAnchor(List<Bubble> bubbles) {
		for (int i = 0; i < 6; ++i) {
			if(adjBubbles[i] != null && !adjBubbles[i].popped) {
				if(!adjBubbles[i].checkedForAnchor && !adjBubbles[i].foundAnchor) {
					adjBubbles[i].checkedForAnchor = true;
					bubbles.Add(adjBubbles[i]);
					if(adjBubbles[i].IsAnchorPoint()) {
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
                        adjBubbles[i].CheckForAnchor(bubbles);
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

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Wall" && Mathf.Abs(_rigibody2D.velocity.x) > 0.1f) {
            _rigibody2D.velocity = new Vector2(_rigibody2D.velocity.x * -1,
                                               _rigibody2D.velocity.y);

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
		}
		if (other.tag == "Bubble") {
			if(!locked && other.GetComponent<Bubble>().locked) {
                if (!PhotonNetwork.connectedAndReady || (GetComponent<PhotonView>().owner == PhotonNetwork.player)) {
                    CollisionWithBoard();
                } else {
                    // Stop moving and sit in place.
                    _rigibody2D.velocity = Vector2.zero;
                    locked = true;
                }
            }
		}
        if(other.tag == "Ceiling") {
            CollisionWithBoard();
        }
        if (other.tag == "Bottom") {
            if(type == HAMSTER_TYPES.DEAD) {
                int inc = 1;

                // Calculate score before the margin multiplier
                int incScore = inc * 100;
                _homeBubbleManager.IncreaseScore(incScore);

                // If we are not playing single player
                if (!GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().isSinglePlayer) {
                    // Multiply by the Margin Multiplier
                    inc = (int)(inc * GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().marginMultiplier);

                    // Start stock orb effect
                    _homeBubbleManager.BubbleEffects.StockOrbEffect(inc, transform.position);
                }
            } else {
                int inc = 3 * (_dropCombo ? 2 : 1);
                
                // Calculate score before the margin multiplier
                int incScore = inc * 100;
                _homeBubbleManager.IncreaseScore(incScore);

                // If we are not playing single player
                if (!GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().isSinglePlayer) {
                    // Multiply by the Margin Multiplier
                    inc = (int)(inc * GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().marginMultiplier);

                    // Start stock orb effect
                    _homeBubbleManager.BubbleEffects.StockOrbEffect(inc, transform.position);
                }
            }
            _homeBubbleManager.RemoveBubble(node);
            _audioSource.clip = _dropClip;
            _audioSource.Play();

            _destroy = true;
		}
	}

    public void CollisionWithBoard() {
        // Stop moving and sit in place.
        _rigibody2D.velocity = Vector2.zero;
        locked = true;

        if (!PhotonNetwork.connectedAndReady) {
            _homeBubbleManager.AddBubble(this);
        } else if (GetComponent<PhotonView>().owner == PhotonNetwork.player) { // if we own this bubble
            _homeBubbleManager.AddBubble(this);
            GetComponent<PhotonView>().RPC("AddToBoard", PhotonTargets.Others, node);
        }

        // Check Matches
        if (type == HAMSTER_TYPES.RAINBOW) {
            DoCrazyRainbowMatches();
            type = HAMSTER_TYPES.RAINBOW;
        } else if (type == HAMSTER_TYPES.BOMB) {
            // Pop self and all adjBubbles
            foreach(Bubble b in adjBubbles) {
                if (b != null) {
                    b.Pop();
                }
            }
            Pop();

            _homeBubbleManager.BubbleEffects.BombBubbleExplosion(transform.position);
        } else {
            matches = new List<Bubble>();
            matches = CheckMatches(matches);
            numMatches = matches.Count;

            if (matches.Count >= 3) {
                //_homeBubbleManager.IncreaseComboCounter(transform.position);
                HandleMatch(matches);
                Pop();
            } else {
                _homeBubbleManager.ResetComboCounter();
            }
        }

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
		type = HAMSTER_TYPES.ORANGE;
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
		if (CheckRainbowMatchCount (greenMatches)) {
			matched = true;
		}
		if (CheckRainbowMatchCount (redMatches)) {
			matched = true;
		}
		if (CheckRainbowMatchCount (orangeMatches)) {
			matched = true;
		}
		if (CheckRainbowMatchCount (grayMatches)) {
			matched = true;
		}
		if (CheckRainbowMatchCount (blueMatches)) {
			matched = true;
		}
		if (CheckRainbowMatchCount (pinkMatches)) {
			matched = true;
		}
        if(CheckRainbowMatchCount (purpleMatches)) {
            matched = true;
        }

		if(matched) {
			Pop();
		}
	}

	public List<Bubble> CheckMatches(List<Bubble> matches) {
		for (int i = 0; i < 6; ++i) {
			if(adjBubbles[i] != null && adjBubbles[i].type != HAMSTER_TYPES.DEAD) {
				if(adjBubbles[i].type == type) {
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

        // Calculate amount of garbage to generate
        int inc = (int)Mathf.Pow((matches.Count - 2), 2); // 3 = 1, 4 = 4, 5 = 9, 6 = 16, 7 = 25, 8 = 36

        // Calculate score before the margin multiplier
        int incScore = inc * 100;
        _homeBubbleManager.IncreaseScore(incScore);

        // Multiply by the Margin Multiplier
        inc = (int)(inc * GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().marginMultiplier);

        // If the combo count is high enough
        // TODO: Although I like it as an idea, this combo stuff seems way too powerful and easy to do.
        if (_homeBubbleManager.ComboCount > 0) {
            // Multiply based on the combo
            //inc += 2 * _homeBubbleManager.ComboCount;
            //Debug.Log("Comboed " + _homeBubbleManager.ComboCount);
        }

        // If we are not playing single player
        if (!GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().isSinglePlayer) {
            // Start stock orb effect
            _homeBubbleManager.BubbleEffects.StockOrbEffect(inc + comboBonus, transform.position);
        }

        // Pop matches
        foreach (Bubble b in matches) {
            if (b != this) {
                b.Pop();
            }
        }
    }

    int CheckCombos(List<Bubble> matches) {
        int comboBonus = 0;
        // Check combo stuff
        foreach (Bubble b in matches) {
            // If the last bubble added has matched and isn't one we threw
            if (b == _homeBubbleManager.LastBubbleAdded && b.PlayerController != _playerController) {
                // Team combo!
                comboBonus += 3;
                _homeBubbleManager.BubbleEffects.TeamComboEffect(transform.position);
            }
        }

        // If this bubble had banked off a wall before matching
        if(_bankedPos.x != 0 && _bankedPos.y != 0) {
            comboBonus += 2;
            _homeBubbleManager.BubbleEffects.BankShot(_bankedPos);
        }

        return comboBonus;
    }

    public void Pop() {
		_homeBubbleManager.RemoveBubble (node);
		popped = true;

        // Instaed of destroying, do a nice animation of the bubble opening.
        _popAnimation.Pop();
		//DestroyObject (this.gameObject);
	}

	public void Drop() {
		locked = false;
        _rigibody2D.velocity = new Vector2 (0.0f, -5f);
		gameObject.layer = 15; // GhostBubble

        // Check for team drop bonus
        if(_homeBubbleManager.LastBubbleAdded != null && _homeBubbleManager.LastBubbleAdded.popped && 
            _playerController != null && _homeBubbleManager.LastBubbleAdded.PlayerController != _playerController) {
            _homeBubbleManager.BubbleEffects.TeamDropEffect(transform.position);
            _dropCombo = true;
        }
	}

	public bool IsAnchorPoint() {
		if (node < _homeBubbleManager.TopLineLength || isGravity) {
			return true;
		}

		return false;
	}

	public void SwitchTeams() {
        if(team == 0) {
            team = 1;
        } else if(team == 1) {
            team = 0;
        }

		if(team == 0) {
			_homeBubbleManager = GameObject.FindGameObjectWithTag ("BubbleManager1").GetComponent<BubbleManager>();
		} else if(team == 1) {
			_homeBubbleManager = GameObject.FindGameObjectWithTag ("BubbleManager2").GetComponent<BubbleManager>();
		}
	}

    // This function scans around the bubble to check whether or not it's possible to be hit by the player.
    void CanBeHit() {
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
    }

    public bool IsSpecialType() {
        if(type == HAMSTER_TYPES.DEAD || type == HAMSTER_TYPES.RAINBOW) {
            return true;
        }

        return false;
    }
}
