using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerOpeningScript : ScriptingController {
    PlayerController _player;
    Hamster _hamster;
    Camera _camera;

    protected override void Awake() {
        base.Awake();
    }
    // Start is called before the first frame update
    protected override void Start() {
        //base.Start();
    }

    public override void Setup() {
        base.Setup();

        // Spawn forest board
        //Instantiate(Resources.Load("Prefabs/Level/Boards/Multiplayer/ForestBoard"));

        // Spawn players
        GameObject player = Instantiate(Resources.Load("Prefabs/Entities/Player")) as GameObject;
        player.transform.position = new Vector3(-3.5f, -5.5f, -0.75f);
        CharaInfo playerInfo = new CharaInfo();
        playerInfo.name = CHARACTERS.BOY;
        playerInfo.color = 1;
        playerInfo.team = 0;
        player.GetComponent<PlayerController>().SetCharacterInfo(playerInfo);
        _player = player.GetComponent<PlayerController>();
        _player.gameObject.AddComponent<PlayerMoveUp>();

        GameObject player2 = Instantiate(Resources.Load("Prefabs/Entities/Player")) as GameObject;
        player2.transform.position = new Vector3(8f, -4f, -0.75f);
        CharaInfo playerInfo2 = new CharaInfo();
        playerInfo2.name = CHARACTERS.ROOSTER;
        playerInfo2.color = 1;
        playerInfo2.team = 1;
        player2.GetComponent<PlayerController>().SetCharacterInfo(playerInfo2);


        // Setup camera
        _camera = FindObjectOfType<CameraExpand>().GetComponent<Camera>();
        _camera.transform.position = new Vector3(-6.05f, -3.51f, -25f);
        _camera.orthographicSize = 3.5f;
        _camera.rect = new Rect(0, 0, 1, 1);

        // Setup Bubble Managers
        BubbleInfo[] bubbles = new BubbleInfo[125];
        for(int i = 0; i < 34; ++i) {
            bubbles[i].isSet = true;
            bubbles[i].type = (HAMSTER_TYPES)Random.Range(0, (int)HAMSTER_TYPES.NUM_NORM_TYPES);
        }

        bubbles[35].isSet = true;
        bubbles[35].type = HAMSTER_TYPES.GREEN;
        bubbles[36].isSet = true;
        bubbles[36].type = HAMSTER_TYPES.PINK;
        bubbles[37].isSet = true;
        bubbles[37].type = HAMSTER_TYPES.GRAY;
        bubbles[38].isSet = true;
        bubbles[38].type = HAMSTER_TYPES.BLUE;
        bubbles[39].isSet = true;
        bubbles[39].type = HAMSTER_TYPES.YELLOW;
        bubbles[40].isSet = true;
        bubbles[40].type = HAMSTER_TYPES.PURPLE;
        bubbles[41].isSet = true;
        bubbles[41].type = HAMSTER_TYPES.RED;
        bubbles[42].isSet = true;
        bubbles[42].type = HAMSTER_TYPES.YELLOW;
        bubbles[43].isSet = true;
        bubbles[43].type = HAMSTER_TYPES.PINK;
        bubbles[44].isSet = true;
        bubbles[44].type = HAMSTER_TYPES.GREEN;
        bubbles[45].isSet = true;
        bubbles[45].type = HAMSTER_TYPES.RED;

        BubbleManager.startingBubbleInfo = bubbles;
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if(Input.GetKeyDown(KeyCode.Return)) {
            Begin();
        }
    }

    public override void Begin() {
        base.Begin();

        // Spawn first hamster
        GameObject hamster = Instantiate(Resources.Load("Prefabs/Entities/Hamster")) as GameObject;
        hamster.transform.position = new Vector3(-13.8f, -6.72f, 0.5f);
        _hamster = hamster.GetComponent<Hamster>();
        _hamster.FaceRight();
        _hamster.Initialize(0);
        _hamster.SetType(1);

        Wait1();
    }

    void Wait1() {
        nextAction = HamsterTurnUp;
        nextTime = 0.68f;
    }

    void HamsterTurnUp() {
        _hamster.FaceUp();
        nextAction = BeginPlayerMovement;
        nextTime = 0.5f;
    }

    void BeginPlayerMovement() {
        _player.GetComponent<PlayerMoveUp>().Begin();
        nextAction = null;
        nextTime = 0f;
    }

    public void SpawnHamster2() {
        GameObject hamster = Instantiate(Resources.Load("Prefabs/Entities/Hamster")) as GameObject;
        hamster.transform.position = new Vector3(-13.8f, -6.72f, 0.5f);
        _hamster = hamster.GetComponent<Hamster>();
        _hamster.FaceRight();
        _hamster.Initialize(0);
        _hamster.SetType(1);

        nextTime = 0.68f;
        nextAction = HamsterTurnUp2;
    }

    void HamsterTurnUp2() {
        _hamster.FaceUp();
        nextTime = 1f;
        nextAction = null;
    }

    public void End() {
        FindObjectOfType<GameCountdown>().StartCountdown();
        began = false;
    }
}
