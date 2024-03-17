using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RedWitch : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] GameObject dashCooldownBar;
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject indicator;
    [SerializeField] FloatingJoystick movementJoystick;
    [SerializeField] FixedJoystick projectileJoystick;
    [SerializeField] GameObject defeatMenu;
    [SerializeField] GameObject virtualCamera;

    CinemachineVirtualCamera virtualCameraComponent;
    Animator animator;
    Rigidbody2D rb;
    Rigidbody2D projectileRigidBody;
    SpriteRenderer sr;
    SpriteRenderer projectileSpriteRenderer;

    Vector2 dir = Vector2.zero;
    Vector2 projectileWay = Vector2.zero;
    Vector2 ProjectilePos = Vector2.zero;
    Color currentColor;

    bool isMobile = false;
    bool canMove = true;
    bool isDashing = false;
    bool canDash = true;
    bool afterRelease;
    bool projectileReset = false;
    bool isCharging = false;
    public bool isFullPower;

    [Header("Player Movement")]
    [SerializeField] float playerSpeed = 3f;
    [SerializeField] float dashingCooldown = 2f;
    [SerializeField] float dashingTime = 0.3f;
    [SerializeField] float dashingPower = 10f;
    float dashBarIndicatorX = 0f;

    [Header("Projectile")]
    [SerializeField] float projectileStartingVelocity = 3f;
    [SerializeField] float projectilePower = 6f;
    [SerializeField] float projectileMaxVelocity = 10f;
    [SerializeField] float projectileSlowdown = 0.992f;

    float projectileVelocity = 3f;
    float indicatorAngle;
    float currentTime = 0f;
    float currentTime2 = 0f;
    float cameraSize;

    public int projectileForm = 0; // 0 = green, 1 = red, 2 = blue

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        virtualCameraComponent = virtualCamera.GetComponent<CinemachineVirtualCamera>();

        projectileSpriteRenderer = projectile.GetComponent<SpriteRenderer>();
        projectileRigidBody = projectile.GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        if (isDashing) return; //chargelarken topun hýzý azalmýyo, taþa çarptýðý sayýlmýyo

        if (!canMove)
        {
            currentTime2 += Time.deltaTime;
            float t = Mathf.Clamp01(currentTime2 / 1.5f);
            cameraSize = Mathf.Lerp(7f, 6.5f, t);

            virtualCameraComponent.m_Lens.OrthographicSize = cameraSize;

        }
        else
        {
            currentTime2 = 0f;
            virtualCameraComponent.m_Lens.OrthographicSize = 7f;
        }

        if (!canDash)
        {
            currentTime += Time.deltaTime;
            float t = Mathf.Clamp01(currentTime / dashingCooldown);
            dashCooldownBar.SetActive(true);
            dashBarIndicatorX = Mathf.Lerp(0.6f, 0f, t);

            dashCooldownBar.transform.localScale = new Vector2(dashBarIndicatorX, 0.05f);

            if(t >= 1)
            {
                currentTime = 0f;
            }
        }
        else
        {
            dashBarIndicatorX = 0;
            dashCooldownBar.transform.localScale = Vector2.zero;
            dashCooldownBar.SetActive(false);
        }

        if (isMobile)
        {
            MobileMovement();
            MobileProjectile();
        }
        else
        {
            KeyboardProjectile();
            KeyboardMovement();
        }



    }

    //PC CONTROLS
    void KeyboardMovement()
    {
        dir = Vector2.zero;
        if (Input.GetKey(KeyCode.LeftArrow) && canMove)
        {
            animator.SetInteger("xAxis", 1);
            sr.flipX = true;
            dir.x = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && canMove)
        {
            animator.SetInteger("xAxis", 1);
            sr.flipX = false;
            dir.x = 1;
        }

        if (Input.GetKey(KeyCode.UpArrow) && canMove)
        {
            animator.SetBool("yAxisMove", true);
            animator.SetInteger("yAxis", 1);
            dir.y = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && canMove)
        {
            animator.SetBool("yAxisMove", true);
            animator.SetInteger("yAxis", -1);
            dir.y = -1;
        }

        if (dir.y == 0)
            animator.SetBool("yAxisMove", false);

        if (dir.x == 0)
            animator.SetInteger("xAxis", 0);

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && canMove)
        {
            activateDash();
        }

        dir.Normalize();
        if (!isDashing)
            rb.velocity = playerSpeed * dir;

        //check if player is moving
        if (rb.velocity == Vector2.zero)
            animator.SetBool("isMoving", false);
        else
            animator.SetBool("isMoving", true);
    }

    void KeyboardProjectile()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            projectileForm = 0;
            projectile.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ResetProjectile();
            projectileForm++;
            if (projectileForm > 2)
            {
                projectileForm = 0;
            }
        }

        if (Input.GetKey(KeyCode.E))
        {
            afterRelease = false;
            canMove = false;
            animator.SetBool("isCharging", true);

            if(projectileForm == 1)
            projectile.GetComponent<CircleCollider2D>().enabled = false;

            SetKeyboardProjectileDirection();
            SetProjectileDirectionIndicator(projectileWay);
            SetProjectilePower();
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            ReleaseProjectile();
        }

        projectileRigidBody.velocity *= projectileSlowdown;
       
    }

    void SetKeyboardProjectileDirection()
    {
        projectileWay.x = Input.GetKey(KeyCode.RightArrow) ? 1 : (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0);
        projectileWay.y = Input.GetKey(KeyCode.UpArrow) ? 1 : (Input.GetKey(KeyCode.DownArrow) ? -1 : 0);

        float xOffset = Input.GetKey(KeyCode.LeftArrow) ? -0.5f : (Input.GetKey(KeyCode.RightArrow) ? 0.5f : 0f);
        float yOffset = Input.GetKey(KeyCode.DownArrow) ? -0.3f : (Input.GetKey(KeyCode.UpArrow) ? 1f : 0.4f);

        ProjectilePos = new Vector2(transform.position.x + xOffset, transform.position.y + yOffset);

        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            projectileWay.y = 1;
            ProjectilePos.y = transform.position.y + 1f;
        }

        if (projectileForm == 1)
        {
            projectile.transform.position = ProjectilePos;
        }
    }

    //MOBILE CONTROLS
    void MobileMovement()
    {
        dir = Vector2.zero;
        if (movementJoystick.Horizontal == -1 && canMove)
        {
            animator.SetInteger("xAxis", 1);
            sr.flipX = true;
            dir.x = -1;
        }
        else if (movementJoystick.Horizontal == 1 && canMove)
        {
            animator.SetInteger("xAxis", 1);
            sr.flipX = false;
            dir.x = 1;
        }

        if (movementJoystick.Vertical == 1 && canMove)
        {
            animator.SetBool("yAxisMove", true);
            animator.SetInteger("yAxis", 1);
            dir.y = 1;
        }
        else if (movementJoystick.Vertical == -1 && canMove)
        {
            animator.SetBool("yAxisMove", true);
            animator.SetInteger("yAxis", -1);
            dir.y = -1;
        }

        if (dir.y == 0)
            animator.SetBool("yAxisMove", false);

        if (dir.x == 0)
            animator.SetInteger("xAxis", 0);

        dir.Normalize();
        if (!isDashing)
            rb.velocity = playerSpeed * dir;

        //check if player is moving
        if (rb.velocity == Vector2.zero)
            animator.SetBool("isMoving", false);
        else
            animator.SetBool("isMoving", true);
    }

    void MobileProjectile()
    {
        if (projectileJoystick.Direction != new Vector2(0, 0) && !projectileReset)
        {
            ResetProjectile();
            projectileReset = true;
        }

        if (projectileJoystick.Direction != new Vector2(0, 0))
        {
            isCharging = true;
            afterRelease = false;
            canMove = false;
            animator.SetBool("isCharging", true);

            SetMobileProjectileDirection();
            SetProjectileDirectionIndicator(projectileWay);
            SetProjectilePower();
        }

        if (projectileJoystick.Direction == new Vector2(0, 0) && isCharging)
        {
            ReleaseProjectile();
            isCharging = false;
            projectileReset = false;
        }

        if (afterRelease)
        {
            projectileRigidBody.velocity *= 0.992f;
        }
    }

    void SetMobileProjectileDirection()
    {
        float xOffset;
        float yOffset;

        if (projectileJoystick.Horizontal == 1)
        {
            projectileWay.x = 1;
            xOffset = 0.5f;
        }
        else if (projectileJoystick.Horizontal == -1)
        {
            projectileWay.x = -1;
            xOffset = -0.5f;
        }
        else
        {
            projectileWay.x = 0;
            xOffset = 0f;
        }

        if (projectileJoystick.Vertical == 1)
        {
            projectileWay.y = 1;
            yOffset = 1f;
        }
        else if (projectileJoystick.Vertical == -1)
        {
            projectileWay.y = -1;
            yOffset = -0.3f;
        }
        else
        {
            projectileWay.y = 0;
            yOffset = 0.4f;
        }

        ProjectilePos = new Vector2(transform.position.x + xOffset, transform.position.y + yOffset);

        if (projectileForm == 0)
        {
            projectile.transform.position = ProjectilePos;
        }
    }

    //OTHER
    public void activateDash()
    {
        StartCoroutine(Dash(dir));
    }

    public IEnumerator Dash(Vector2 direction)
    {
        direction.Normalize();
        canDash = false;
        isDashing = true;
        animator.SetBool("isDashing", true);

        if (direction == Vector2.zero)
            rb.velocity = dashingPower * new Vector2(0f, -1f);
        else
            rb.velocity = dashingPower * direction;

        yield return new WaitForSeconds(dashingTime);

        animator.SetBool("isDashing", false);
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);

        canDash = true;
    }

    void SetProjectileDirectionIndicator(Vector2 projectileWay)
    {
        projectileWay.Normalize();
        indicator.transform.localPosition = projectileWay;

        indicatorAngle = Mathf.Atan2(projectileWay.y, projectileWay.x) * Mathf.Rad2Deg;
        indicator.transform.rotation = Quaternion.Euler(0, 0, indicatorAngle - 90);
    }

    void SetProjectilePower()
    {
        if (projectileVelocity <= projectileMaxVelocity)
        {
            isFullPower = false;
            projectileVelocity += Time.deltaTime * projectilePower;

        }
        else if (projectileVelocity >= projectileMaxVelocity)
            isFullPower = true;
    }
    
    void ReleaseProjectile()
    {
        isFullPower = false;
        projectile.GetComponent<CircleCollider2D>().enabled = true;
        indicator.SetActive(false);
        projectileRigidBody.velocity += projectileVelocity * projectileWay;
        afterRelease = true;
        canMove = true;
        animator.SetBool("isCharging", false);
        

    }

    public void ResetProjectile()
    {
        
        indicator.SetActive(true);
        //projectileRigidBody.velocity = Vector2.zero;
        projectileVelocity = projectileStartingVelocity;
        projectileSpriteRenderer.color = Color.white;
        
        if (projectileForm == 0)
        {
            
            projectileRigidBody.velocity = Vector2.zero;
            projectile.layer = gameObject.layer + 3;
            projectileSpriteRenderer.sortingLayerID = sr.sortingLayerID;
            projectileSpriteRenderer.sortingOrder = sr.sortingOrder;
        }

        projectile.SetActive(true);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Slime"))
        {
            defeatMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
