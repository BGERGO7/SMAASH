using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using System.Collections;

public class MeleeAttack : MonoBehaviour
{
    PhotonView view;

    public Animator animator;

    //public Animator attack_anim;
    public GameObject attackBtn;

    public Transform attackPoint;
    public Transform attackPointOpposite;
    public float attackRange = 1f;
    public LayerMask enemyLayer;
    public SpriteRenderer spriteRenderer;
    
    public int attackDamage = 40;
    public int attackNum;

    private InputActionAsset inputAsset;
    private InputActionMap player;

    int damage = 20;
    bool canAttack = true;

    public void Start()
    {
        attackNum = 1;
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
        player.FindAction("Attack").started += Attack;
        player.Enable();
    }

    private void OnDisable()
    {
        //Eltavolitja
        player.FindAction("Attack").started -= Attack;
        player.Disable();
    }


    [PunRPC]
    public void Attack(InputAction.CallbackContext context)
    {
        if(this.enabled == true && context.performed && view.IsMine && canAttack == true)
        {
            animator.SetTrigger("Attack1");

            //Valtozoba tarolja azt a collidert (masik jatekost), ami a koron belul van
            if(spriteRenderer.flipX == true){
                Collider2D hitEnemyOpposite = Physics2D.OverlapCircle(attackPointOpposite.position, attackRange, enemyLayer);
                hitEnemyOpposite.GetComponent<TakeDmg>().TakeDamageCaller(damage);

            }else if(spriteRenderer.flipX == false){
                Collider2D hitEnemy = Physics2D.OverlapCircle(attackPoint.position, attackRange, enemyLayer);
                hitEnemy.GetComponent<TakeDmg>().TakeDamageCaller(damage);
            }

        }
    }


    //Lerajzolja a kort a jobb lathatosagert az editorban
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        if (attackPointOpposite == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawWireSphere(attackPointOpposite.position, attackRange);
    }
}
