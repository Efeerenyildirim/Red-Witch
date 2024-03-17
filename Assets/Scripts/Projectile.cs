using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject player;
    RedWitch playerScript;
    bool isTouchingWall;
    Animator animator;

    void Start()
    {
        playerScript = player.GetComponent<RedWitch>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        animator.SetInteger("ProjectileForm", playerScript.projectileForm);
        animator.SetBool("isFullPower", playerScript.isFullPower);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Slime"))
        {
            playerScript.projectileForm = 0;
            playerScript.ResetProjectile();
            gameObject.SetActive(false);
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = true;
            Debug.Log("se");
            StartCoroutine(TeleportAfterDelay());
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = false;
            StopAllCoroutines();
        }
    }

    private IEnumerator TeleportAfterDelay()
    {
        yield return new WaitForSeconds(0.3f);

        if (isTouchingWall)
        {
            playerScript.projectileForm = 0;
            playerScript.ResetProjectile();
            gameObject.SetActive(false);
        }
    }
}
