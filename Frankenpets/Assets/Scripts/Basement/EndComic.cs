using System.Collections;
using UnityEngine;

public class EndComic : MonoBehaviour, IComicFlow
{
    [Header("Comic Variables")]
    public ComicManager comicManager;
    public GameObject comicPanel;
    public GameObject comicCanvas;

    [Header("Player Inputs")]
    public InputHandler player1Input;
    public InputHandler player2Input;

    [Header("Reference to Basement Logic Handler")]
    public BasementText basementText;

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
        if (other.GetComponent<Collider>().CompareTag("dog front") || other.GetComponent<Collider>().CompareTag("cat front") ||
            other.GetComponent<Collider>().CompareTag("dog back") || other.GetComponent<Collider>().CompareTag("cat back"))
        {
            StartCoroutine(TransitionToComic());
        }
    }

    public IEnumerator TransitionToComic()
    {
        yield return StartCoroutine(comicManager.FadeToBlack());

        // Activate comic canvas and panel
        if (comicCanvas != null)
        {
            comicCanvas.SetActive(true);
        }
        if (comicPanel != null)
        {
            comicPanel.SetActive(true);
        }
    
        // Start the comic sequence with the input handlers
        // Player 1 (with Player 1 light) is always Cat
        // Player 2 (with Player 2 light) is always Dog
        comicManager.StartComic(player1Input, player2Input, this);
    }
    
    public void OnComicComplete()
    {
        StartCoroutine(comicManager.UnfadeFromBlack());

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
    }
}
