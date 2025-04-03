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
    - "Break Glass Doors" [1]
    - "Throw Away Shoes" [1]
    - "Find Golden Bone" [1]
    - "Free the Pets" [2]
*/

public class TaskManager : MonoBehaviour
{
    private static List<Task> allTasks = new List<Task>();

    public static TaskManager Instance { get; private set; }
    public event Action OnTaskCompleted;

    [Header("Task Banner variables")]
    [SerializeField] private GameObject taskCompletedBanner;
    private Vector3 targetPosition = new Vector3(0, 423, 0);
    private Vector3 originalPosition = new Vector3(0, 723, 0);
    private RectTransform bannerChildRectTransform; 
    private RectTransform bannerTextChildRectTransform; 
    private float moveSpeed = 5.0f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        bannerChildRectTransform = taskCompletedBanner.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        bannerTextChildRectTransform = taskCompletedBanner.transform.GetChild(1).gameObject.GetComponent<RectTransform>();

    }

    private void Start()
    {
        taskCompletedBanner.SetActive(false);
        bannerChildRectTransform.anchoredPosition = originalPosition;
        bannerTextChildRectTransform.anchoredPosition = originalPosition + new Vector3(0, 723 + 197, 0);
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
        AudioManager.Instance.PlayTaskCompletionSound();

        taskCompletedBanner.SetActive(true);
        StartCoroutine(MoveToPosition(targetPosition));

        yield return new WaitForSeconds(4.0f); // Display banner for 3 seconds

        StartCoroutine(MoveToPosition(originalPosition));
        yield return new WaitForSeconds(3.0f);
        taskCompletedBanner.SetActive(false);
    }

    private IEnumerator MoveToPosition(Vector2 target)
    {
        while (Vector2.Distance(bannerChildRectTransform.anchoredPosition, target) > 1f)
        {
            bannerChildRectTransform.anchoredPosition = Vector2.Lerp(bannerChildRectTransform.anchoredPosition, target, moveSpeed * Time.deltaTime);
            bannerTextChildRectTransform.anchoredPosition = Vector2.Lerp(bannerTextChildRectTransform.anchoredPosition, target + new Vector2(0, 197), moveSpeed * Time.deltaTime);

            yield return null;
        }
    }
}