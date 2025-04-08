using UnityEngine;
using System.Collections.Generic;

public class AdvanceToBasement : MonoBehaviour
{
    public static AdvanceToBasement Instance { get; private set; }
    public GameObject basementBlocker;
    public bool isAtBasementDoor = false;
    public bool antiDogClub = false;
    private List<Task> livingRoomTasks;
    private LivingRoomText livingRoomManager;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        livingRoomManager = FindObjectOfType<LivingRoomText>();
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
            basementBlocker.SetActive(true);
            livingRoomManager.SetCurrAdvBasementStage(1);
        }

        if (TaskManager.CheckTaskCompletion(livingRoomTasks) && other.CompareTag("cat front"))
        {
            antiDogClub = false;
            basementBlocker.SetActive(false);
            livingRoomManager.SetCurrAdvBasementStage(2);
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
