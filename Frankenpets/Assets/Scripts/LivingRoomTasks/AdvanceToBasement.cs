using UnityEngine;
using System.Collections.Generic;

public class AdvanceToBasement : MonoBehaviour
{
    public static AdvanceToBasement Instance { get; private set; }
    public bool isAtBasementDoor = false;
    public bool antiDogClub = false;
    public bool hideAntiDogClub = true;
    private List<Task> livingRoomTasks;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        livingRoomTasks = TaskManager.GetAllTasksOfLevel(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (TaskManager.CheckTaskCompletion(livingRoomTasks) && (other.CompareTag("dog front") || other.CompareTag("cat front") || other.CompareTag("dog back") || other.CompareTag("dog back")))
        {
            isAtBasementDoor = true;
        }

        if (TaskManager.CheckTaskCompletion(livingRoomTasks) && other.CompareTag("dog front"))
        {
            antiDogClub = true;
            hideAntiDogClub = false;
        }

        if (TaskManager.CheckTaskCompletion(livingRoomTasks) && other.CompareTag("cat front"))
        {
            antiDogClub = false;
            hideAntiDogClub = true;
        }
    }
}
