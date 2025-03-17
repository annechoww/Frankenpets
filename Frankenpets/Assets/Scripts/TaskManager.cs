using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

/* This file holds all functions pertaining to managing the tasks.
    Lists of Task Names Implemented
    - "Scatter Boxes" [0]
    - "Shatter Vase" [0]
    - "Move Rug" [0]

*/

public class TaskManager : MonoBehaviour
{
    private static List<Task> allTasks = new List<Task>();

    public static TaskManager Instance { get; private set; }
    public event Action OnTaskCompleted;
    public AudioClip taskCompletedSound;
    public AudioSource audioSource;

    [SerializeField] private GameObject taskCompletedBanner;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        taskCompletedBanner.SetActive(false);
    }

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

    public static List<Task> FindTasksByName(string name)
    {
        return allTasks.FindAll(task => task.Name == name);
    }

    // Get all tasks of a specific level
    public static List<Task> GetAllTasksOfLevel(int level)
    {
        return allTasks.FindAll(task => task.Level == level);
    }

    public static bool CheckTaskCompletion(List<Task> tasks)
    {
        return tasks.All(task => task.IsComplete);

    }




    public void CompleteTask()
    {
        OnTaskCompleted?.Invoke();
        StartCoroutine(ShowBanner());
    }

    private IEnumerator ShowBanner()
    {
        taskCompletedBanner.SetActive(true);

        if (taskCompletedSound != null)
        {
            AudioManager.Instance.PlayTaskCompletionSound();
        }

        yield return new WaitForSeconds(2.0f); // Display banner for 2 seconds
        taskCompletedBanner.SetActive(false);
    }
}