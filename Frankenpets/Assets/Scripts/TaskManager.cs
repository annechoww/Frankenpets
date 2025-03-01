using UnityEngine;
using System.Collections.Generic;

/* This file holds all functions pertaining to managing the tasks.
    Lists of Task Names Implemented
    - "Scatter Boxes" [0]
    - "Shatter Vase" [0]
    - "Move Rug" [0]
*/

public class TaskManager : MonoBehaviour
{
    private static List<Task> allTasks = new List<Task>();

    public static void RegisterTask(Task task)
    {
        if (!allTasks.Contains(task))
        {
            allTasks.Add(task);
        }
    }

    public static void UnregisterTask(Task task)
    {
        allTasks.Remove(task);
    }

    public static List<Task> GetAllTasks()
    {
        return allTasks;
    }

    public static Task FindTaskByName(string name)
    {
        return allTasks.Find(task => task.Name == name);
    }

    // Get all tasks of a specific level
    public static List<Task> GetAllTasksOfLevel(int level)
    {
        return allTasks.FindAll(task => task.Level == level);
    }
}