using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Animator animator;
    public HealthBar healthBar;
    public int attackNum = 1;
    public Joystick joystick;

    private float horizontal;
    private float speed = 8f;
    public float jumpingPower = 10f;
    private bool isFacingRight = true;
    bool isDead = false;

    public int maxHealth = 100;
	public int currentHealth;
    private int jumpNumber;
    public int jumpNumberValue = 2;


    void Start()
    {
		currentHealth = maxHealth;
		healthBar.SetMaxHealth(maxHealth);
        jumpNumber = jumpNumberValue;
    }
    
    void Update()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

        if (!isDead && !isFacingRight && horizontal > 0f)
        {
            Flip();
        }
        else if (!isDead && isFacingRight && horizontal < 0f)
        {
            Flip();
        }

        if (!isDead && !IsGrounded())
        {
            animator.SetBool("isJumping", true);
        } else
        {
            animator.SetBool("isJumping", false);
        }

        if(currentHealth <= 0)
        {
            animator.SetBool("isDead", true);
            isDead = true;
            horizontal = 0;
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
        
    }
    private void FixedUpdate()
    {
        if(!isDead)
        {
            if(joystick.Horizontal >= .2f)
            {
                horizontal = speed;
            }else if(joystick.Horizontal <= -.2f)
            {
                horizontal = -speed;
            }else
            {
                horizontal = 0f;
            }
            rb.velocity = new Vector2(horizontal, rb.velocity.y);

            animator.SetFloat("speed", Math.Abs(horizontal));
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(!isDead && IsGrounded())
        {
            jumpNumber = jumpNumberValue;
        }

        if(!isDead && context.performed && jumpNumber > 0)
        {
            rb.velocity = Vector2.up * jumpingPower;
            jumpNumber--;
        }else if(!isDead && context.performed && jumpNumber == 0 && IsGrounded())
        {
            rb.velocity = Vector2.up * jumpingPower;
        }

    }
    public void Attack(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
           animator.SetTrigger("isAttacking");
        }
    }
    

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        
    }

    private void Flip()
    {
        if (!isDead)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}