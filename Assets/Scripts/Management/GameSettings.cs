using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour {

    // Multiplayer
    public bool[] specialHamstersMultiplayer = new bool[4]; // Rainbow, Skull, Bomb, Plasma
    public bool aimAssistMultiplayer;
    public SPECIALSPAWNMETHOD specialSpawnMethod = SPECIALSPAWNMETHOD.BOTH;
    public bool AnySpecialsMultiplayer {
        get { return specialHamstersMultiplayer[0] || specialHamstersMultiplayer[1] || specialHamstersMultiplayer[2] || specialHamstersMultiplayer[3]; }
    }
    public bool SpecialBallsOn {
        get { return (specialSpawnMethod == SPECIALSPAWNMETHOD.BOTH || specialSpawnMethod == SPECIALSPAWNMETHOD.BALLS) && AnySpecialsMultiplayer; }
    }
    public bool SpecialPipeOn {
        get { return specialSpawnMethod == SPECIALSPAWNMETHOD.BOTH || specialSpawnMethod == SPECIALSPAWNMETHOD.PIPE; }
    }

    public int hamsterSpawnMaxMultiplayer = 8;
    public int HamsterSpawnMax {
        get { return hamsterSpawnMaxMultiplayer; }

        set {
            hamsterSpawnMaxMultiplayer = value;
            if (hamsterSpawnMaxMultiplayer < 4) {
                hamsterSpawnMaxMultiplayer = 4;
            } else if (hamsterSpawnMaxMultiplayer > 12) {
                hamsterSpawnMaxMultiplayer = 12;
            }
        }
    }

    // Singleplayer
    public bool[] specialHamstersSingleplayer = new bool[4]; // Rainbow, Skull, Bomb, Plasma
    public bool aimAssistSingleplayer;
    public AIMASSIST aimAssistSetting = AIMASSIST.AFTERLOSS;
    public int hamsterSpawnMaxSingleplayer = 8;


    // Start is called before the first frame update
    void Start() {
        
    }

    public void DefaultSettings() {
        aimAssistMultiplayer = false;
        hamsterSpawnMaxMultiplayer = 8;
    }

    // Update is called once per frame
    void Update() {
        
    }
}
