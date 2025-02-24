using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BoxCollisionManager : MonoBehaviour
{
    public static BoxCollisionManager Instance { get; private set; }

    public Image taskItem;
    public Color completedColor;

    public static int collidedBoxes = 0;
    //public static HashSet<GameObject> frontBoxes = new HashSet<GameObject>(); 
    //public static HashSet<GameObject> backBoxes = new HashSet<GameObject>(); 
    private static bool frontBoxes = false;
    private static bool backBoxes = false;
    
    private HavocManager havocMeter; 
    private TutorialText tutorialText;
    private bool isFirstCollision = true;


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

        tutorialText = GameObject.Find("TutorialTextManager").GetComponent<TutorialText>();
    }
    
    // register collision
    public static void RegisterCollision(GameObject obj, string tag)
    {
        if (tag == "front")
        {
            // if (!frontBoxes.Contains(obj)) {
            //     frontBoxes.Add(obj);
            //     Debug.Log($"{obj.name} collided. Front Total: {frontBoxes.Count}");
            // }
            frontBoxes = true;

            if (frontBoxes && backBoxes)
            {
                Instance.FinishTask();
            }

        } else if (tag == "back")
        {
            // if (!backBoxes.Contains(obj)) {
            //     backBoxes.Add(obj);
            //     Debug.Log($"{obj.name} collided. Back Total: {backBoxes.Count}");
            // }
            backBoxes = true;
            //if (frontBoxes.Count + backBoxes.Count == totalBoxes)
            if (frontBoxes && backBoxes)
            {
                Instance.FinishTask();
            }
        }
    }

    private void FinishTask(){
        taskItem.color = completedColor;
        if (isFirstCollision && (tutorialText.getCurrTutorialStage() == TutorialText.tutScatterBoxes))
        {
            tutorialText.advanceTutorialStage();
            isFirstCollision = false;
            havocMeter = GameObject.Find("HavocManager").GetComponent<HavocManager>();
            havocMeter.collectHavocPoints(5);
        }
    }
}
