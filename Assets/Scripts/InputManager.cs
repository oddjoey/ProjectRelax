using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;

    // Player Input actions
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction attackAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction useAction;
    private InputAction inventoryAction;
    private InputAction settingsAction;

    // UI Input Actions
    private InputAction navigateAction;
    private InputAction submitAction;
    private InputAction cancelAction;
    private InputAction pointAction;
    private InputAction leftClickAction;
    private InputAction scrollAction;
    private InputAction middleClickAction;
    private InputAction rightClickAction;

    // Vehicle Input Actions
    private InputAction engineAction;

    // Player Inputs
    [SerializeField] public Vector2 moveDelta;
    [SerializeField] public Vector2 lookDelta;
    [SerializeField] public bool action1;
    [SerializeField] public bool sprint;
    [SerializeField] public bool jump;
    [SerializeField] public bool use;
    [SerializeField] public bool inventory;
    [SerializeField] public bool settings;

    // UI Inputs
    [SerializeField] public Vector2 navigate;
    [SerializeField] public bool submit;
    [SerializeField] public bool cancel;
    [SerializeField] public Vector2 point;
    [SerializeField] public bool leftClick;
    [SerializeField] public Vector2 scroll;
    [SerializeField] public bool middleClick;
    [SerializeField] public bool rightClick;

    // Vehicle Inputs
    public bool engine; 
    
    // On Awake
    private void Awake()
    {
        moveAction = inputActions.FindActionMap("Player").FindAction("Move");
        lookAction = inputActions.FindActionMap("Player").FindAction("Look");
        attackAction = inputActions.FindActionMap("Player").FindAction("Attack");
        sprintAction = inputActions.FindActionMap("Player").FindAction("Sprint");
        jumpAction = inputActions.FindActionMap("Player").FindAction("Jump");
        useAction = inputActions.FindActionMap("Player").FindAction("Use");
        inventoryAction = inputActions.FindActionMap("Player").FindAction("Inventory");
        settingsAction = inputActions.FindActionMap("Player").FindAction("Settings");

        navigateAction = inputActions.FindActionMap("UI").FindAction("Navigate");
        submitAction = inputActions.FindActionMap("UI").FindAction("Submit");
        cancelAction = inputActions.FindActionMap("UI").FindAction("Cancel");
        pointAction = inputActions.FindActionMap("UI").FindAction("Point");
        leftClickAction = inputActions.FindActionMap("UI").FindAction("Click");
        scrollAction = inputActions.FindActionMap("UI").FindAction("ScrollWheel");
        middleClickAction = inputActions.FindActionMap("UI").FindAction("MiddleClick");
        rightClickAction = inputActions.FindActionMap("UI").FindAction("RightClick");
        
        engineAction = inputActions.FindActionMap("Vehicle").FindAction("Engine");

        moveAction.performed += context => moveDelta = context.ReadValue<Vector2>();
        moveAction.canceled += context => moveDelta = Vector2.zero;
        lookAction.performed += context => lookDelta = context.ReadValue<Vector2>();
        lookAction.canceled += context => lookDelta = Vector2.zero;
        attackAction.performed += context => action1 = true;
        attackAction.canceled += context => action1 = false;
        sprintAction.performed += context => sprint = true;
        sprintAction.canceled += context => sprint = false;
        jumpAction.performed += context => jump = true;
        jumpAction.canceled += context => jump = false;
        useAction.performed += context => use = true;
        useAction.canceled += context => use = false;
        inventoryAction.performed += context => inventory = true;
        inventoryAction.canceled += context => inventory = false;
        settingsAction.performed += context => settings = true;
        settingsAction.canceled += context => settings = false;

        navigateAction.performed += context => navigate = context.ReadValue<Vector2>();
        navigateAction.canceled += context => navigate = Vector2.zero;
        submitAction.performed += context => submit = true;
        submitAction.canceled += context => submit = false;
        cancelAction.performed += context => cancel = true;
        cancelAction.canceled += context => cancel = false;
        pointAction.performed += context => point = context.ReadValue<Vector2>();
        pointAction.canceled += context => point = Vector2.zero;
        leftClickAction.performed += context => leftClick = true;
        leftClickAction.canceled += context => leftClick = false;
        scrollAction.performed += context => scroll = context.ReadValue<Vector2>();
        scrollAction.canceled += context => scroll = Vector2.zero;;
        middleClickAction.performed += context => middleClick = true;
        middleClickAction.canceled += context => middleClick = false;
        rightClickAction.performed += context => rightClick = true;
        rightClickAction.canceled += context => rightClick = false;

        engineAction.performed += context => engine = true;
        engineAction.canceled += context => engine = false;
    }

    // On Enabled
    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        attackAction.Enable();
        sprintAction.Enable();
        jumpAction.Enable();
        useAction.Enable();
        inventoryAction.Enable();
        settingsAction.Enable();

        navigateAction.Enable();
        submitAction.Enable();
        cancelAction.Enable();
        pointAction.Enable();
        leftClickAction.Enable();
        scrollAction.Enable();
        middleClickAction.Enable();
        rightClickAction.Enable();

        engineAction.Enable();
    }
    
    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        attackAction.Disable();
        sprintAction.Disable();
        jumpAction.Disable();
        useAction.Disable();
        inventoryAction.Disable();
        settingsAction.Disable();

        navigateAction.Disable();
        submitAction.Disable();
        cancelAction.Disable();
        pointAction.Disable();
        leftClickAction.Disable();
        scrollAction.Disable();
        middleClickAction.Disable();
        rightClickAction.Disable();

        engineAction.Disable();
    }
}
