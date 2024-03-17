using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;

    public float speed = 3f;
    bool canMove = true;

    bool isDashing = false;
    bool canDash = true;
    float dashingCooldown = 2f;
    float dashingTime = 0.15f;
    float dashingPower = 10f;

    public GameObject projectile;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }


    private void Update()
    {
        if (isDashing) return;

        // PROJECTILE CHARGE BEGIN
        if (Input.GetKeyDown(KeyCode.E))
        {
            canMove = false;
            animator.SetBool("Charging", true);
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            canMove = true;
            animator.SetBool("Charging", false);
        }
        // PROJECTILE CHARGE END


        // PLAYER MOVEMENT BEGIN
        Vector2 dir = Vector2.zero;
        if (Input.GetKey(KeyCode.A) && canMove)
        {
            animator.SetBool("IsMoving", true);
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            dir.x = -1;
        }
        else if (Input.GetKey(KeyCode.D) && canMove)
        {
            animator.SetBool("IsMoving", true);
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            dir.x = 1;
        }

        if (Input.GetKey(KeyCode.W) && canMove)
        {
            animator.SetBool("IsMoving", true);
            dir.y = 1;
        }
        else if (Input.GetKey(KeyCode.S) && canMove)
        {
            animator.SetBool("IsMoving", true);
            dir.y = -1;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && canMove)
        StartCoroutine(Dash(dir));

        dir.Normalize();
        if (!isDashing)
        rb.velocity = speed * dir;

        //check if player is moving
        if (rb.velocity == Vector2.zero)
        animator.SetBool("IsMoving", false);
        else
        animator.SetBool("IsMoving", true);

        // PLAYER MOVEMENT END



        
    }

    public IEnumerator Dash(Vector2 direction)
    {

        canDash = false;
        isDashing = true;
        animator.SetBool("Dashing", true);

        if (direction == Vector2.zero)
        rb.velocity = dashingPower * new Vector2(0f, -1f);
        else
        rb.velocity = dashingPower * direction;

        yield return new WaitForSeconds(dashingTime);

        animator.SetBool("Dashing", false);
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);

        canDash = true;
    }


    //eski hareket þemasý (Red Witch)
    /*
    void SetProjectileDirectionn()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            projectileWay.x = -1;
            ProjectilePos.x = transform.position.x - 0.5f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            projectileWay.x = 1;
            ProjectilePos.x = transform.position.x + 0.5f;
        }
        else
        {
            projectileWay.x = 0;
            ProjectilePos.x = transform.position.x;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            projectileWay.y = 1;
            ProjectilePos.y = transform.position.y + 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            projectileWay.y = -1;
            ProjectilePos.y = transform.position.y - 0.3f;
        }
        else
        {
            projectileWay.y = 0;
            ProjectilePos.y = transform.position.y + 0.4f;
        }

        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            projectileWay.y = 1;
            ProjectilePos.y = transform.position.y + 1f;
        }

        if (projectileForm == 0)
        {
            projectile.transform.position = ProjectilePos;
        }
    }
    */
}
