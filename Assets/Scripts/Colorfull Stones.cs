using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorfullStones : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject rock;

    RedWitch playerScript;
    int projectileForm;
    [SerializeField] int stoneColor;

    void Start()
    {
        playerScript = player.GetComponent<RedWitch>();
        projectileForm = playerScript.projectileForm;
    }

    void Update()
    {
        projectileForm = playerScript.projectileForm;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            if(projectileForm == stoneColor)
            {
                rock.SetActive(false);
            }
        }
    }
}
