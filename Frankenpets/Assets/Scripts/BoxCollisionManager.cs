using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BoxCollisionManager : MonoBehaviour
{
    public static BoxCollisionManager Instance { get; private set; }

    public Image taskItem;
    public Color completedColor;

    public static HashSet<GameObject> collidedBoxes = new HashSet<GameObject>(); 
    private static int totalObjects = 4; 

    // awake singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // register collision
    public static void RegisterCollision(GameObject obj)
    {
        if (!collidedBoxes.Contains(obj))
        {
            collidedBoxes.Add(obj);
            Debug.Log($"{obj.name} collided. Total: {collidedBoxes.Count}");

            if (collidedBoxes.Count == totalObjects)
            {
                Instance.FinishTask();
            }
        }
    }

    private void FinishTask(){
        taskItem.color = completedColor;
    }
}
