using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndComic : MonoBehaviour, IComicFlow
{
    [Header("Comic Variables")]
    public ComicManager comicManager;
    public GameObject comicPanel;
    public GameObject comicCanvas;
    [Header("Fade Variables")]
    public Image fadeOverlay;

    [Header("Player Inputs")]
    public InputHandler player1Input;
    public InputHandler player2Input;

    [Header("Reference to Basement Logic Handler")]
    public BasementText basementText;
    private bool isEnding = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Make sure comic elements are disabled initially
        if (comicCanvas != null)
        {
            comicCanvas.SetActive(false);
        }
        if (comicPanel != null)
        {
            comicPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (isEnding) return;
        
        if (other.GetComponent<Collider>().CompareTag("dog front") || other.GetComponent<Collider>().CompareTag("cat front") ||
            other.GetComponent<Collider>().CompareTag("dog back") || other.GetComponent<Collider>().CompareTag("cat back"))
        {
            isEnding = true;
            StartCoroutine(TransitionToComic());
        }
    }

    public IEnumerator TransitionToComic()
    {
        // Activate comic canvas and panel
        if (comicCanvas != null)
        {
            comicCanvas.SetActive(true);
        }
        if (comicPanel != null)
        {
            comicPanel.SetActive(true);
        }

        // not working properly >:(
        // yield return StartCoroutine(CustomFadeToBlack());

        // Start the comic sequence with the input handlers
        // Player 1 (with Player 1 light) is always Cat
        // Player 2 (with Player 2 light) is always Dog
        comicManager.StartComic(player1Input, player2Input, this);
        
        yield return null;
    }

    public IEnumerator CustomFadeToBlack()
    {
        float fadeToBlackTime = 6f;
        // Set the fade overlay color to black
        fadeOverlay.color = Color.black;
        CanvasGroup fadeGroup = fadeOverlay.GetComponent<CanvasGroup>();

        // Start with overlay invisible
        fadeGroup.alpha = 0f;

        // Fade to black
        float timer = 0f;
        while (timer < fadeToBlackTime)
        {
            fadeGroup.alpha = timer / fadeToBlackTime;
            timer += Time.deltaTime;

        }

        // Ensure overlay is fully black
        fadeGroup.alpha = 1f;
        yield return null;
    }

    private IEnumerator UnfadeAndStopComic()
    {
        // eff this i cant get it to work
        // yield return comicManager.UnfadeFromBlack();

        if (basementText != null)
        {
            basementText.PlayEndOverlaySequence();

            if (comicCanvas != null)
            {
                comicCanvas.SetActive(false);
            }
            if (comicPanel != null)
            {
                comicPanel.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("BasementText not assigned to EndComic.cs!");
        }
        
        yield return null;
    }

    public void OnComicComplete()
    {
        StartCoroutine(UnfadeAndStopComic());
    }
}
