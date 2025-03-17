// using UnityEngine;
// using System;
// using System.Collections;
// using System.Diagnostics;

// public class TaskCompleteManager : MonoBehaviour
// {
//     public static TaskCompleteManager Instance { get; private set; }
//     public event Action OnTaskCompleted;
//     public AudioClip taskCompletedSound;

//     [SerializeField] private GameObject taskCompletedBanner;

//     private void Awake()
//     {
//         if (Instance == null)
//             Instance = this;
//         else
//             Destroy(gameObject);
//     }

//     private void Start()
//     {
//         taskCompletedBanner.SetActive(false);
//     }

//     public void CompleteTask()
//     {
//         OnTaskCompleted?.Invoke();
//         StartCoroutine(ShowBanner());
//     }

//     private IEnumerator ShowBanner()
//     {
//         taskCompletedBanner.SetActive(true);

//         if (taskCompletedSound != null)
//         {
//             audioSource.PlayOneShot(taskCompletedSound);
//         }

//         yield return new WaitForSeconds(2f); // Display banner for 2 seconds
//         taskCompletedBanner.SetActive(false);
//     }
// }
