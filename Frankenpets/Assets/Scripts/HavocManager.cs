using UnityEngine;
using UnityEngine.UI;

public class HavocManager : MonoBehaviour
{
    public Slider havocMeter;
    private float currentPoints = 0f;
    private float maxPoints = 100f;
    private float transitionSpeed = 5f;
    
    void Start()
    {
        havocMeter.maxValue = maxPoints;
        havocMeter.value = currentPoints;
    }

    void Update()
    {
        havocMeter.value = Mathf.Lerp(havocMeter.value, currentPoints, transitionSpeed * Time.deltaTime);
    }

    public void collectHavocPoints(float points)
    {
        currentPoints += points;

        if (currentPoints > maxPoints)
        {
            currentPoints = maxPoints;
        }
    }
}