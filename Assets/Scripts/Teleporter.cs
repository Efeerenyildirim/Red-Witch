using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Teleporter : MonoBehaviour
{

    public bool playerInside = false;
    [SerializeField] string sceneToLoad;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInside = true;
            StartCoroutine(TeleportAfterDelay());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = false;
            StopAllCoroutines();
        }
    }

    private IEnumerator TeleportAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        if (playerInside)
        {
            PlayerPrefs.SetInt("isPlayerClearedTutorial", 1);
            PlayerPrefs.Save();

            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
