using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControlsCornerUI : MonoBehaviour
{
    [Header("Corner/mini Controls")]
    public GameObject cornerControlsUIParent;
    private GameObject cornerControlsUI;
    private GameObject P1ControlsCF;
    private GameObject P1ControlsCB;
    private GameObject P2ControlsDF;
    private GameObject P2ControlsDB;

    [Header("Script references")]
    private ControllerAssignment controllerAssignment;
    public PlayerManager playerManager;

    [Header("P1 Front Control Icons")]
    public Image p1SwitchIcon;
    public Image p1ReconnectIcon;
    public Image p1FrontPawIcon;
    public Image p1MeowIcon;
    public Image p1ClimbIcon;

    [Header("P2 Front Control Icons")]
    public Image p2SwitchIcon;
    public Image p2ReconnectIcon;
    public Image p2FrontPawIcon;
    public Image p2BarkIcon;
    public Image p2GrabIcon;

    [Header("P1 Back Control Icons")]
    public Image p1BackSwitchIcon;
    public Image p1BackReconnectIcon;
    public Image p1JumpIcon;
    public Image p1TailIcon;
    public Image p1HighJumpIcon;
    
    [Header("P2 Back Control Icons")]
    public Image p2BackSwitchIcon;
    public Image p2BackReconnectIcon;
    public Image p2JumpIcon;
    public Image p2TailIcon;
    public Image p2DashIcon;

    [Header("Icon States")]
    public Sprite RTNormal;
    public Sprite RTPressed;
    public Sprite LTNormal;
    public Sprite LTPressed;
    public Sprite ANormal;
    public Sprite APressed;
    public Sprite XNormal;
    public Sprite XPressed;
    public Sprite YNormal;
    public Sprite YPressed;


    [Header("Show the controls corner?")]
    public bool show = false; 

    private bool initialized = false;

    void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controllerAssignment = FindFirstObjectByType<ControllerAssignment>();
        InitializeControlsUI();


        if (controllerAssignment.IsKeyboard()) cornerControlsUI = cornerControlsUIParent.transform.GetChild(0).gameObject;
        else cornerControlsUI = cornerControlsUIParent.transform.GetChild(1).gameObject;

        if (show) cornerControlsUI.SetActive(true);
        
        Transform P1Controls = cornerControlsUI.transform.GetChild(0).gameObject.transform;
        Transform P2Controls = cornerControlsUI.transform.GetChild(1).gameObject.transform;

        P1ControlsCF = P1Controls.GetChild(0).gameObject;
        P1ControlsCB = P1Controls.GetChild(1).gameObject;

        P2ControlsDF = P2Controls.GetChild(0).gameObject;
        P2ControlsDB = P2Controls.GetChild(1).gameObject;

    }

    // Update is called once per frame
    void Update()
    {   
        if (initialized && show)
        {
            cornerControlsUI.SetActive(true);
            UpdateControlsCornerUI();
            UpdateButtonHighlights();
        }
        else if (initialized && !show)
        {
            cornerControlsUI.SetActive(false);
        }
    }

    void InitializeControlsUI()
    {
            
        if (playerManager == null)
            playerManager = FindAnyObjectByType<PlayerManager>();
        
        // Ensure we have all the required references
        if (controllerAssignment == null || cornerControlsUIParent == null || playerManager == null)
            return;
        
        // Set up the proper UI based on input type
        cornerControlsUI = controllerAssignment.IsKeyboard() 
            ? cornerControlsUIParent.transform.GetChild(0).gameObject 
            : cornerControlsUIParent.transform.GetChild(1).gameObject;
        
        // Set up control references
        Transform P1Controls = cornerControlsUI.transform.GetChild(0);
        Transform P2Controls = cornerControlsUI.transform.GetChild(1);
        
        P1ControlsCF = P1Controls.GetChild(0).gameObject;
        P1ControlsCB = P1Controls.GetChild(1).gameObject;
        
        P2ControlsDF = P2Controls.GetChild(0).gameObject;
        P2ControlsDB = P2Controls.GetChild(1).gameObject;
        
        initialized = true;
        Debug.Log("Controls Corner UI successfully initialized");
    }

    public void UpdateControlsCornerUI()
    {
        // bool isP1Cat = playerManager.P1.Species == "cat";
        bool isP1Front = playerManager.P1.IsFront;

        P1ControlsCF.SetActive(isP1Front);
        P1ControlsCB.SetActive(!isP1Front);

        P2ControlsDF.SetActive(!isP1Front);
        P2ControlsDB.SetActive(isP1Front);
    }

    public void setShow(bool setting)
    {
        show = setting;
    }

    public void UpdateButtonHighlights() {
        InputHandler p1Input = playerManager.player1Input;
        InputHandler p2Input = playerManager.player2Input;

        if (P1ControlsCF.activeSelf) {
            p1SwitchIcon.sprite = p1Input.GetSwitchPressed() ? LTPressed : LTNormal;
            p1ReconnectIcon.sprite = p1Input.GetReconnectPressed() ? RTPressed : RTNormal;
            p1FrontPawIcon.sprite = p1Input.GetJumpPressed() ? APressed : ANormal;
            p1MeowIcon.sprite = p1Input.GetSoundTailPressed() ? YPressed : YNormal;
            p1ClimbIcon.sprite = p1Input.GetSpecialActionPressed() ? XPressed : XNormal;
        }
            
        if (P1ControlsCB.activeSelf) {
            p1BackSwitchIcon.sprite = p1Input.GetSwitchPressed() ? LTPressed : LTNormal;
            p1BackReconnectIcon.sprite = p1Input.GetReconnectPressed() ? RTPressed : RTNormal;
            p1JumpIcon.sprite = p1Input.GetJumpPressed() ? APressed : ANormal;
            p1TailIcon.sprite = p1Input.GetSoundTailPressed() ? YPressed : YNormal;
            p1HighJumpIcon.sprite = p1Input.GetSpecialActionPressed() ? XPressed : XNormal;
        }

        if (P2ControlsDF.activeSelf) {
            p2SwitchIcon.sprite = p2Input.GetSwitchPressed() ? LTPressed : LTNormal;
            p2ReconnectIcon.sprite = p2Input.GetReconnectPressed() ? RTPressed : RTNormal;
            p2FrontPawIcon.sprite = p2Input.GetJumpPressed() ? APressed : ANormal;
            p2BarkIcon.sprite = p2Input.GetSoundTailPressed() ? YPressed : YNormal;
            p2GrabIcon.sprite = p2Input.GetSpecialActionPressed() ? XPressed : XNormal;
        }

        if (P2ControlsDB.activeSelf) {
            p2BackSwitchIcon.sprite = p2Input.GetSwitchPressed() ? LTPressed : LTNormal;
            p2BackReconnectIcon.sprite = p2Input.GetReconnectPressed() ? RTPressed : RTNormal;
            p2JumpIcon.sprite = p2Input.GetJumpPressed() ? APressed : ANormal;
            p2TailIcon.sprite = p2Input.GetSoundTailPressed() ? YPressed : YNormal;
            p2DashIcon.sprite = p2Input.GetSpecialActionPressed() ? XPressed : XNormal;
        }
    }
}
