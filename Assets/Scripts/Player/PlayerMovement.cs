using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Photon.Pun;
using System.Threading;
using Photon.Pun.UtilityScripts;
using System.Numerics;
using System.Runtime.InteropServices;

public class PlayerMovement : MonoBehaviour, IPunObservable
{
    PhotonView view;

    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public GameObject attackPoint;

    public Animator animator;

    private float horizontal;
    private float speed = 8f;
    public float jumpingPower = 10f;
    public Joystick joystick;

    private UnityEngine.Vector3 smoothMove;

    private int extraJumps;
    public int extraJumpValue = 2;
    public int attackNum = 1;

    public SpriteRenderer spriteRenderer;

    private InputActionAsset inputAsset;
    private InputActionMap player;

    void Start()
    {
        extraJumps = extraJumpValue;
        joystick = GameObject.Find("Floating Joystick").GetComponent<Joystick>();
        view = GetComponent<PhotonView>();

        //Camera.main.GetComponent<CameraMotion>().setTarget(gameObject.transform);
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
        player.Enable();
    }

    private void OnDisable()
    {
        //Eltavolitja
        player.FindAction("Jump").started -= Jump;
        player.Disable();
    }
    
    void Update()
    {
        //Ha mi iranyitjuk a karaktert
        if(view.IsMine)
        {
            //Karakter megforitasa
            if (this.enabled == true && horizontal > 0f)
            {
                spriteRenderer.flipX = false;
                attackPoint.transform.position = new UnityEngine.Vector2(this.transform.position.x + 1f, this.transform.position.y);
                view.RPC("OnDirectionChange_RIGHT", RpcTarget.Others);
            }
            else if (this.enabled == true && horizontal < 0f)
            {
                spriteRenderer.flipX = true;
                attackPoint.transform.position = new UnityEngine.Vector2(this.transform.position.x - 1f, this.transform.position.y);
                view.RPC("OnDirectionChange_LEFT", RpcTarget.Others);
            }

            //Jump animacio
            if (this.enabled == true && !IsGrounded())
            {
                animator.SetBool("isJumping", true);
            } else
            {
                animator.SetBool("isJumping", false);
            }

            if(this.enabled == true)
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
        attackPoint.transform.position = new UnityEngine.Vector2(this.transform.position.x - 1f, this.transform.position.y);
    }
    [PunRPC]
    void OnDirectionChange_RIGHT()
    {
        spriteRenderer.flipX = false;
        attackPoint.transform.position = new UnityEngine.Vector2(this.transform.position.x + 1f, this.transform.position.y);
    }  

    public void Jump(InputAction.CallbackContext context)
    {
        if(view.IsMine)
        {
            if(this.enabled == true && IsGrounded())
            {
                extraJumps = extraJumpValue;
            }

            if(this.enabled == true && context.performed && extraJumps > 0)
            {
                rb.velocity = UnityEngine.Vector2.up * jumpingPower;
                extraJumps--;
            }else if(this.enabled == true && context.performed && extraJumps == 0 && IsGrounded())
            {
                rb.velocity = UnityEngine.Vector2.up * jumpingPower;
            }
        }
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        
    }

    //Elkuldi a poziciot a serverre
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(this.enabled);
        }else if(stream.IsReading)
        {
            smoothMove = (UnityEngine.Vector3)stream.ReceiveNext();
            this.enabled = (bool)stream.ReceiveNext();
        }
    }
}