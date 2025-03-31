using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class LevelHandler : MonoBehaviour
{
    public LevelLoader levelLoader;
    private List<Task> tasks;
    private Collider basementTrigger;

    private void Awake()
    {
        basementTrigger = GetComponent<Collider>();
        if (basementTrigger != null)
        {
            basementTrigger.enabled = false;
        }
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
