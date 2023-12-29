using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Photon.Pun;
using System.Threading;
using Photon.Pun.UtilityScripts;
using System.Numerics;

public class PlayerMovement : MonoBehaviour, IPunObservable
{
    PhotonView view;

    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Animator animator;
    private float horizontal;
    private float speed = 8f;
    public float jumpingPower = 10f;
    private UnityEngine.Vector3 smoothMove;

    bool isDead = false;

    public int maxHealth = 100;
	public int currentHealth;
    private int extraJumps;
    public int extraJumpValue = 2;

    public int attackNum = 1;

    public HealthBar healthBar;
    
    public Joystick joystick;

    public SpriteRenderer spriteRenderer;

    private InputActionAsset inputAsset;
    private InputActionMap player;

    void Start()
    {
		currentHealth = maxHealth;
		healthBar.SetMaxHealth(maxHealth);
        extraJumps = extraJumpValue;
        joystick = GameObject.Find("Floating Joystick").GetComponent<Joystick>();
        view = GetComponent<PhotonView>();
    }

    private void Awake()
    {
        //Megkeresi a sajat action mapjet
        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("Player");
    }

    private void OnEnable()
    {
        //Inditaskor hozzaadja
        player.FindAction("Jump").started += Jump;
        player.FindAction("Attack").started += Attack;
        player.Enable();
    }

    private void OnDisable()
    {
        //Eltavolitja
        player.FindAction("Jump").started -= Jump;
        player.FindAction("Attack").started -= Attack;
        player.Disable();
    }
    
    void Update()
    {
        //Ha mi iranyitjuk a karaktert
        if(view.IsMine)
        {
            //Karakter megforitasa
            if (!isDead && horizontal > 0f)
            {
                spriteRenderer.flipX = false;
                view.RPC("OnDirectionChange_RIGHT", RpcTarget.Others);
            }
            else if (!isDead && horizontal < 0f)
            {
                spriteRenderer.flipX = true;
                view.RPC("OnDirectionChange_LEFT", RpcTarget.Others);
            }

            //Jump animacio
            if (!isDead && !IsGrounded())
            {
                animator.SetBool("isJumping", true);
            } else
            {
                animator.SetBool("isJumping", false);
            }

            //Halal animacio es bool beallitasa
            if(currentHealth <= 0)
            {
                animator.SetBool("isDead", true);
                isDead = true;
                horizontal = 0;
                rb.velocity = new UnityEngine.Vector2(horizontal, rb.velocity.y);
            }

            if(!isDead)
            {
                //Joystick csak ebben a tartomanyban eszelel es nem noveli a sebesseget
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
                rb.velocity = new UnityEngine.Vector2(horizontal, rb.velocity.y);

                animator.SetFloat("speed", Math.Abs(horizontal));
            }
        }else
        {
            //Ha nem mi iranyitunk, akkor smooth movemenet
            SmoothSyncMovement();
        }     
    }

    private void SmoothSyncMovement()
    {
        transform.position = UnityEngine.Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 10);
    }

    [PunRPC]
    void OnDirectionChange_LEFT()
    {
        spriteRenderer.flipX = true;
    }
    [PunRPC]
    void OnDirectionChange_RIGHT()
    {
        spriteRenderer.flipX = false;
    }  

    public void Jump(InputAction.CallbackContext context)
    {
        if(view.IsMine)
        {
            if(!isDead && IsGrounded())
            {
                extraJumps = extraJumpValue;
            }

            if(!isDead && context.performed && extraJumps > 0)
            {
                rb.velocity = UnityEngine.Vector2.up * jumpingPower;
                extraJumps--;
            }else if(!isDead && context.performed && extraJumps == 0 && IsGrounded())
            {
                rb.velocity = UnityEngine.Vector2.up * jumpingPower;
            }
        }
    }
    
    public void Attack(InputAction.CallbackContext context)
    {
        if(!isDead && context.performed && view.IsMine)
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

    //Elkuldi a pozicionkat a serverre
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }else if(stream.IsReading)
        {
            smoothMove = (UnityEngine.Vector3)stream.ReceiveNext();
        }
    }
}