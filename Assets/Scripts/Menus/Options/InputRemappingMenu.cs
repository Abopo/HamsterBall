using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class InputRemappingMenu : MonoBehaviour {

    public bool unsavedChanges = false;

    InputMapper _inputMapper;
    InputMapper.Context _context;

    Player _player;

    Menu _controlMapMenu;

    public InputMapper InputMapper { get => _inputMapper; }

    void Awake() {
        _inputMapper = new InputMapper();
    }
    // Start is called before the first frame update
    void Start() {
        _context = new InputMapper.Context();
        _controlMapMenu = transform.GetComponentInParent<Menu>();

        _player = ReInput.players.GetPlayer(0);

        InitializeMapper();
    }

    void InitializeMapper() {
        _inputMapper.options.allowAxes = false;
        _inputMapper.options.allowKeyboardKeysWithModifiers = false;
        _inputMapper.options.allowKeyboardModifierKeyAsPrimary = true;
        _inputMapper.options.ignoreMouseXAxis = true;
        _inputMapper.options.ignoreMouseYAxis = true;

        _inputMapper.InputMappedEvent += OnInputMapped;
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void StartJumpMap() {
        // Lose focus on menu
        _controlMapMenu.LoseFocus();

        // Setup context
        Controller controller = _player.controllers.Keyboard;
        ActionElementMap aeMap = _player.controllers.maps.GetFirstButtonMapWithAction(2, true);
        _context.actionId = 2;
        _context.controllerMap = _player.controllers.maps.GetMap(controller, 0/*Default*/, 3/*Player1*/);
        _context.actionRange = AxisRange.Positive;
        _context.actionElementMapToReplace = aeMap;

        // Start input mapper
        _inputMapper.Start(_context);
    }

    public void StartCatchMap() {
        // Lose focus on menu
        _controlMapMenu.LoseFocus();

        // Setup context
        Controller controller = _player.controllers.Keyboard;
        ActionElementMap aeMap = _player.controllers.maps.GetFirstButtonMapWithAction(3, true);
        _context.actionId = 3;
        _context.controllerMap = _player.controllers.maps.GetMap(controller, 0/*Default*/, 3/*Player1*/);
        _context.actionRange = AxisRange.Positive;
        _context.actionElementMapToReplace = aeMap;

        // Start input mapper
        _inputMapper.Start(_context);
    }

    public void StartAttackMap() {
        // Lose focus on menu
        _controlMapMenu.LoseFocus();

        // Setup context
        Controller controller = _player.controllers.Keyboard;
        ActionElementMap aeMap = _player.controllers.maps.GetFirstButtonMapWithAction(4, true);
        _context.actionId = 4;
        _context.controllerMap = _player.controllers.maps.GetMap(controller, 0/*Default*/, 3/*Player1*/);
        _context.actionRange = AxisRange.Positive;
        _context.actionElementMapToReplace = aeMap;

        // Start input mapper
        _inputMapper.Start(_context);
    }

    public void StartSwapMap() {
        // Lose focus on menu
        _controlMapMenu.LoseFocus();

        // Setup context
        Controller controller = _player.controllers.Keyboard;
        ActionElementMap aeMap = _player.controllers.maps.GetFirstButtonMapWithAction(5, true);
        _context.actionId = 5;
        _context.controllerMap = _player.controllers.maps.GetMap(controller, 0/*Default*/, 3/*Player1*/);
        _context.actionRange = AxisRange.Positive;
        _context.actionElementMapToReplace = aeMap;

        // Start input mapper
        _inputMapper.Start(_context);
    }

    void OnInputMapped(InputMapper.InputMappedEventData mapData) {
        _controlMapMenu.TakeFocus();

        unsavedChanges = true;
    }

    public void RestoreDefaults() {
        ActionElementMap aeMap = _player.controllers.maps.GetFirstButtonMapWithAction(2, true);
        aeMap.elementIdentifierId = 23; // W for jump

        aeMap = _player.controllers.maps.GetFirstButtonMapWithAction(3, true);
        aeMap.elementIdentifierId = 10; // J for catch

        aeMap = _player.controllers.maps.GetFirstButtonMapWithAction(4, true);
        aeMap.elementIdentifierId = 11; // K for attack

        aeMap = _player.controllers.maps.GetFirstButtonMapWithAction(5, true);
        aeMap.elementIdentifierId = 12; // L for swap

        RemapButton[] remapButtons = GetComponentsInChildren<RemapButton>();
        foreach(RemapButton remapButton in remapButtons) {
            remapButton.UpdateButtonText();
        }
    }

    public void SaveChanges() {
        unsavedChanges = false;

        ReInput.userDataStore.Save();
    }

    public void CheckSaveState() {
        if(unsavedChanges) {
            // Show warning of unsaved changes?

            // Restore to defaults if not saved
            RestoreDefaults();
        }
    }
}
