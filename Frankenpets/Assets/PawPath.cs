using System.Collections;
using UnityEngine;

public class PawPath : MonoBehaviour
{
    public GameObject path;
    private SpriteRenderer[] pawPrints; 
    public AnimationCurve glowCurve; // controls fade-in and fade-out
    public float cycleDuration = 2.0f; 
    public float delayBetweenPrints = 0.4f; 
    private bool isActive = true;

    private void Start()
    {
        pawPrints = path.GetComponentsInChildren<SpriteRenderer>();
        StartCoroutine(PlayPawPath());
    }

    private IEnumerator PlayPawPath()
    {
        while (isActive)
        {
            foreach (SpriteRenderer paw in pawPrints)
            {
                StartCoroutine(FadePaws(paw)); // animate each paw print
                yield return new WaitForSeconds(delayBetweenPrints); // wait before the next paw print
            }
        }
    }

    private IEnumerator FadePaws(SpriteRenderer sprite)
    {
        float time = 0.0f;
        while (time < cycleDuration)
        {
            float glowIntensity = glowCurve.Evaluate(time / cycleDuration);
            sprite.color = new Color(1f, 1f, 1f, glowIntensity); // adjust transparency
            time += Time.deltaTime;
            yield return null;
        }

        sprite.color = new Color(1f, 1f, 1f, 0f); // fully invisible at the end
    }

    public void setActive(bool setting)
    {
        isActive = setting;
        path.SetActive(setting);
    }

    
}
