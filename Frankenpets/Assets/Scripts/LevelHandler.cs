using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class LevelHandler : MonoBehaviour
{
    public LevelLoader levelLoader;
    private bool levelComplete = false;
    private List<Task> tasks;

    // Update is called once per frame
    void Update()
    {
        tasks = TaskManager.GetAllTasksOfLevel(1);
        if (TaskManager.CheckTaskCompletion(tasks))
        {
            if (levelLoader != null) 
            {
                levelLoader.LoadNextLevel();
            }
        }
    }
}
