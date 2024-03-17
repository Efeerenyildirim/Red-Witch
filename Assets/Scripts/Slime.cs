using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{

    [SerializeField] GameObject slime;
    [SerializeField] GameObject player;
    [SerializeField] GameObject slimeBody;
    [SerializeField] GameObject slimeShadow;

    CapsuleCollider2D slimeCollider;
    Rigidbody2D rb;
    Animator animator;
    RedWitch playerScript;

    float slimeMoveSpeed = 3f;
    float jumpDistanceMin = 0f;
    float jumpDistanceMax = 5f;
    float jumpDuration = 1f;
    float holdDuration = 0.2f;
    float slimeYAxis = 0f;
    float timer = 0f;
    float jumpTimer = 0f;
    float timeBetweenJumps = 0.5f;

    bool isJumping = false;
    bool isHolding = false;
    bool isGoingDown = false;
    bool isTouchingDown = true;
    bool isjumpedOnce = false;
    bool splitOnce = true;

    float slimeShadowSize = 0f;
    float slimeShadowNormalSizeX = 5f;
    float slimeShadowJumpingSizeX = 3f;

    float temp;
    float temp2;
    
    int slimeForm = 0; // 0 = green, 1 = red, 2 = blue
    int projectileForm = 0;

    float splitForce = 50000f;

    void Start()
    {
        animator = slimeBody.GetComponent<Animator>();
        slimeCollider = slimeShadow.GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        playerScript = player.GetComponent<RedWitch>();
    }

    void Update()
    {
        projectileForm = playerScript.projectileForm;
        Vector2 direction = (player.transform.position - transform.position).normalized;

        if (isTouchingDown)
        jumpTimer += Time.deltaTime;

        if (jumpTimer >= timeBetweenJumps) // yada oyuncu charge yapmaya baþladýðýnda
        {
            slimeForm++; // belirli alanlara deðdiðinde renk deðiþecek
            if(slimeForm > 2)
            slimeForm = 0;

            timeBetweenJumps = Random.Range(1f, 2.5f);
            jumpTimer = 0;
            isTouchingDown = false;
            isGoingDown = false;
            isJumping = true;
            isjumpedOnce = true;

            animator.SetInteger("SlimeForm", slimeForm);
            animator.SetTrigger("isJumped");
        }

        if (isJumping)
        {
            Jump();
        }
        else if (isHolding && !isGoingDown)
        {
            Hold();
        }

        if(isjumpedOnce)
        UpdateSlimePosition(direction);

        HandleSlimeCollision();

    }

    void Jump()
    {
        isTouchingDown = false;
        timer += Time.deltaTime;

        float t = Mathf.SmoothStep(0f, 1f, timer / jumpDuration);
        slimeYAxis = Mathf.Lerp(jumpDistanceMin, jumpDistanceMax, t);
        slimeShadowSize = Mathf.Lerp(slimeShadowNormalSizeX, slimeShadowJumpingSizeX, t);

        if (timer >= jumpDuration)
        {
            isJumping = false;
            timer = 0f;
            isHolding = true;

            if (isGoingDown)
            {
                SwapJumpValues();
                isTouchingDown = true;
            }
        }
    }

    void Hold()
    {
        timer += Time.deltaTime;

        if (timer >= holdDuration)
        {
            isHolding = false;
            timer = 0f;
            isJumping = true;
            isGoingDown = true;

            SwapJumpValues();
        }
    }

    void SwapJumpValues()
    {
        temp = jumpDistanceMin;
        jumpDistanceMin = jumpDistanceMax;
        jumpDistanceMax = temp;

        temp2 = slimeShadowNormalSizeX;
        slimeShadowNormalSizeX = slimeShadowJumpingSizeX;
        slimeShadowJumpingSizeX = temp2;
    }

    void UpdateSlimePosition(Vector2 direction)
    {
        slimeBody.transform.localPosition = new Vector2(0, slimeYAxis);
        rb.velocity = isTouchingDown ? Vector2.zero : direction * slimeMoveSpeed;
        slimeShadow.transform.localScale = new Vector2(slimeShadowSize, slimeShadow.transform.localScale.y);
    }

    void HandleSlimeCollision()
    {
        if (isTouchingDown)
        {
            slimeCollider.enabled = true;
        }
        else
        {
            slimeCollider.enabled = false;
        }
    }

    void Split()
    {
            float randomX = Random.Range(-1f, 1f);
            float randomY = Random.Range(-1f, 1f);
            float randomForce = Random.Range(0.5f, 2f);

            Vector2 splitDirectionLeft = new Vector2(randomX, randomY);
            Vector2 splitDirectionRight = new Vector2(-randomX, -randomY);

            GameObject splitPartLeft = Instantiate(slime, transform.position, Quaternion.identity);
            GameObject splitPartRight = Instantiate(slime, transform.position, Quaternion.identity);

            splitPartLeft.transform.localScale = new Vector2(transform.localScale.x - 0.5f, transform.localScale.y - 0.5f);
            splitPartRight.transform.localScale = new Vector2(transform.localScale.x - 0.5f, transform.localScale.y - 0.5f);

            if (splitPartRight.transform.localScale.x <= 0.1f)
            {
                Destroy(splitPartLeft);
                Destroy(splitPartRight);
            }

            Slime leftSlime = splitPartLeft.GetComponent<Slime>();
            Slime rightSlime = splitPartRight.GetComponent<Slime>();

            leftSlime.jumpDuration = Random.Range(0.5f, 1.5f);
            rightSlime.jumpDuration = Random.Range(0.5f, 1.5f);

            leftSlime.slimeMoveSpeed = Random.Range(2f, 3f);
            rightSlime.slimeMoveSpeed = Random.Range(2f, 3f);

            leftSlime.slimeForm = Random.Range(0, 2);
            rightSlime.slimeForm = Random.Range(0, 2);

            splitPartLeft.GetComponent<Rigidbody2D>().AddForce(splitDirectionLeft * splitForce * randomForce);
            splitPartRight.GetComponent<Rigidbody2D>().AddForce(splitDirectionRight * splitForce * randomForce);

            Destroy(gameObject);
    }



    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            if(slimeForm == projectileForm)
            {
                if(splitOnce)
                {
                    Invoke("Split", 0.05f);
                    splitOnce = false;
                }
                
                Debug.Log("slime damaged");
            }
        }
    }
}
