using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class LevelHandler : MonoBehaviour
{
    public LevelLoader levelLoader;
    public GameObject closedBasementDoors;
    public GameObject openBasementDoors;
    public GameObject basementDoorArrow;
    private List<Task> tasks;
    private Collider basementTrigger;

    private void Awake()
    {
        basementTrigger = GetComponent<Collider>();
        if (basementTrigger != null)
        {
            basementTrigger.enabled = false;
        }
        closedBasementDoors.SetActive(true);
        openBasementDoors.SetActive(false);
    }

    void Update()
    {
        tasks = TaskManager.GetAllTasksOfLevel(1);
        if (TaskManager.CheckTaskCompletion(tasks))
        {
            UnlockNextLevel();
        }
    }

    void UnlockNextLevel()
    {
        if (basementTrigger != null)
        {
            basementTrigger.enabled = true;
        }
        closedBasementDoors.SetActive(false);
        openBasementDoors.SetActive(true);
        basementDoorArrow.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dog front") || other.CompareTag("cat front"))
        {
            if (levelLoader != null)
            {
                levelLoader.LoadNextLevel();
            }
        }
    }
}
