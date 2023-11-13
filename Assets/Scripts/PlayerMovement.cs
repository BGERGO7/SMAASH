using System;
using Unity.VisualScripting;
using UnityEngine;
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
    private float jumpingPower = 16f;
    private bool isFacingRight = true;
    bool isDead = false;

    public int maxHealth = 100;
	public int currentHealth;


    void Start()
    {
		currentHealth = maxHealth;
		healthBar.SetMaxHealth(maxHealth);
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

        if (Input.GetKeyDown(KeyCode.H))
		{
			TakeDamage(10);
		}

        if (Input.GetKeyDown(KeyCode.Space))
		{
			AddHealth(10);
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
        if(collider.gameObject.name == "dmg")
        {
            TakeDamage(10);
        }else if(collider.gameObject.name == "heal")
        {
            AddHealth(10);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!isDead && context.performed && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (!isDead && context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
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