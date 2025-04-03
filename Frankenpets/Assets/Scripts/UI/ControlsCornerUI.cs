using UnityEngine;
using System.Collections;

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

    [Header("Show the controls corner?")]
    public bool show = false; 

    private bool initialized = false;

    void Awake()
    {
        StartCoroutine(WaitForControllerAssignment());
    }

    private IEnumerator WaitForControllerAssignment() {
        // Wait for the ControllerAssignment singleton to exist
        while (ControllerAssignment.Instance == null)
            yield return null;
            
        // Wait for it to be fully initialized
        while (!ControllerAssignment.Instance.IsInitialized())
            yield return null;
            
        // Now it's safe to use
        controllerAssignment = ControllerAssignment.Instance;
        
        // Initialize your UI
        InitializeControlsUI();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controllerAssignment = ControllerAssignment.Instance;


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
            updateControlsCornerUI();
        }
        else if (initialized && !show)
        {
            cornerControlsUI.SetActive(false);
        }
    }

    void InitializeControlsUI()
    {
        // Get controller assignment if needed
        if (controllerAssignment == null)
            controllerAssignment = ControllerAssignment.Instance;
            
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

    public void updateControlsCornerUI()
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
}
