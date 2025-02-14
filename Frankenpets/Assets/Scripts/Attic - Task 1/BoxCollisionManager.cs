using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BoxCollisionManager : MonoBehaviour
{
    public static BoxCollisionManager Instance { get; private set; }

    public Image taskItem;
    public Color completedColor;

    //public static HashSet<GameObject> collidedBoxes = new HashSet<GameObject>(); 
    public static int collidedBoxes = 0;
    public static HashSet<GameObject> frontBoxes = new HashSet<GameObject>(); 
    public static HashSet<GameObject> backBoxes = new HashSet<GameObject>(); 
    private static int totalBoxes = 4; 

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
    public static void RegisterCollision(GameObject obj, string tag)
    {
        if (tag == "front")
        {
            if (!frontBoxes.Contains(obj)) {
                frontBoxes.Add(obj);
                Debug.Log($"{obj.name} collided. Front Total: {frontBoxes.Count}");
            }

            if (frontBoxes.Count + backBoxes.Count == totalBoxes)
            {
                Instance.FinishTask();
            }

        } else if (tag == "back")
        {
            if (!backBoxes.Contains(obj)) {
                backBoxes.Add(obj);
                Debug.Log($"{obj.name} collided. Back Total: {backBoxes.Count}");
            }
            if (frontBoxes.Count + backBoxes.Count == totalBoxes)
            {
                Instance.FinishTask();
            }
        }
    }

    private void FinishTask(){
        taskItem.color = completedColor;
    }
}
