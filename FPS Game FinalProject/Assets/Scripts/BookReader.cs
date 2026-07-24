using UnityEngine;
using TMPro;

public class BookReader : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject bookPanel;
    public GameObject bookPrompt;

    public TMP_Text titleText;
    public TMP_Text storyText;


    [Header("Book Content")]
    public string bookTitle = "The Whispering Garden";

    [TextArea(5, 10)]
    public string story =
        "Long ago, beyond the forests of Wonderland, there was a mysterious garden hidden from ordinary visitors.\n\n" +
        "The flowers growing inside the garden were unlike any others. " +
        "They could whisper forgotten memories and secrets to those who listened carefully.\n\n" +
        "Many travelers searched for this magical place, but only people with a kind heart could hear the gentle voices of the flowers.\n\n" +
        "It was said that the garden did not reveal its secrets to the strongest person, but to the one who truly understood the wonders of Wonderland.";


    private bool playerNearby = false;
    private bool reading = false;


    private void Start()
    {
        bookPanel.SetActive(false);
        bookPrompt.SetActive(false);
    }


    private void Update()
    {
        if (playerNearby && !reading)
        {
            bookPrompt.SetActive(true);

            if (Input.GetKeyDown(KeyCode.R))
            {
                OpenBook();
            }
        }


        else if (reading)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                CloseBook();
            }
        }
    }


    private void OpenBook()
    {
        reading = true;

        bookPrompt.SetActive(false);
        bookPanel.SetActive(true);

        titleText.text = bookTitle;
        storyText.text = story;
    }


    private void CloseBook()
    {
        reading = false;

        bookPanel.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            playerNearby = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            playerNearby = false;

            bookPrompt.SetActive(false);
        }
    }
}