using UnityEngine;

public class Startup : MonoBehaviour
{
    [Header("Controller Input")]
    public ControllerAssignment controllerAssignment;
    public InputHandler player1Input;
    public InputHandler player2Input;

    public GameObject startText;
    public LevelLoader levelLoader;

    void Update()
    {
        if (controllerAssignment.IsKeyboard() && Input.GetKey(KeyCode.Space))
        {
            if (levelLoader != null) 
            {
                levelLoader.LoadNextLevel();
            }
        } else if (!controllerAssignment.IsKeyboard() && (player1Input.GetJumpPressed() || player2Input.GetJumpPressed()))
        {
            if (levelLoader != null) 
            {
                levelLoader.LoadNextLevel();
            }
        }
    }

    void Awake()
    {
        if (controllerAssignment.IsKeyboard())
        {
            startText.transform.GetChild(0).gameObject.SetActive(true);
            startText.transform.GetChild(2).gameObject.SetActive(true);
        }
        else {
            startText.transform.GetChild(1).gameObject.SetActive(true);
            startText.transform.GetChild(3).gameObject.SetActive(true);
        }
        
        // Flip the text to unmirror it
        // climbText.transform.rotation = Quaternion.Euler(0, climbText.transform.rotation.eulerAngles.y + 180, 0);
    }
}