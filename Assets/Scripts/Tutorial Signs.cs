using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSigns : MonoBehaviour
{

    [SerializeField] GameObject tutorialText;


    void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            tutorialText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            tutorialText.SetActive(false);
        }
    }
}
