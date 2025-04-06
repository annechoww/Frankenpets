using UnityEngine;
using System.Collections.Generic;

public class AdvanceToBasement : MonoBehaviour
{
    public static AdvanceToBasement Instance { get; private set; }
    public bool isAtBasementDoor = false;
    public bool antiDogClub = false;
    private List<Task> livingRoomTasks;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        livingRoomTasks = TaskManager.GetAllTasksOfLevel(1);
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
        }

        if (TaskManager.CheckTaskCompletion(livingRoomTasks) && other.CompareTag("cat front"))
        {
            antiDogClub = false;
        }
    }

    public bool GetAntiDogClub()
    {
        return antiDogClub;
    }

    public bool GetIsAtBasementDoor()
    {
        return isAtBasementDoor;
    }
}
