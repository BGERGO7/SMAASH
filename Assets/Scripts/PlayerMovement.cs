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
    
    public Joystick joystick;

    private float horizontal;
    private float speed = 8f;
    public float jumpingPower = 10f;
    private bool isFacingRight = true;
    bool isDead = false;

    public int maxHealth = 100;
	public int currentHealth;
    private int extraJumps;
    public int extraJumpValue = 2;

    public int attackNum = 1;


    void Start()
    {
		currentHealth = maxHealth;
		healthBar.SetMaxHealth(maxHealth);
        extraJumps = extraJumpValue;
    }
    
    void Update()
    {
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
            rb.velocity = new Vector2(horizontal, rb.velocity.y);
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

    private void OnTriggerEnter2D(Collider2D collider)
    {
        TakeDamage(10);
    }

    public void Jump(InputAction.CallbackContext context)
    {

        if(!isDead && IsGrounded())
        {
            extraJumps = extraJumpValue;
        }

        if(!isDead && context.performed && extraJumps > 0)
        {
            rb.velocity = Vector2.up * jumpingPower;
            extraJumps--;
        }else if(!isDead && context.performed && extraJumps == 0 && IsGrounded())
        {
            rb.velocity = Vector2.up * jumpingPower;
        }

    }

    public void Attack(InputAction.CallbackContext context)
    {
        if(!isDead && context.performed)
        {
            if(attackNum == 1)
            {
                animator.SetTrigger("Attack1");
                attackNum = 2;
            }else if(attackNum == 2)
            {
                animator.SetTrigger("Attack2");
                attackNum = 1;
            }
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

    public void Move(InputAction.CallbackContext context)
    {
        if (!isDead)
        {
            horizontal = context.ReadValue<Vector2>().x;
        }
    }

    void TakeDamage(int damage)
	{
        if(currentHealth <= maxHealth && currentHealth > 0)
        {
            currentHealth -= damage;

		    healthBar.SetHealth(currentHealth);
        }
	}

    void AddHealth(int amount)
    {
        if(currentHealth < maxHealth && currentHealth >= 0)
        {
            currentHealth += amount;

            healthBar.SetHealth(currentHealth);
        }
    }
}