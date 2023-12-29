using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System;

public class MeleeAttack : MonoBehaviour
{
    PhotonView view;

    public Animator animator;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayer;
    public int attackDamage = 40;
    public bool isDead;
    public int attackNum;

    private InputActionAsset inputAsset;
    private InputActionMap player;

    int damage = 20;

    public void Start()
    {
        isDead = false;
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


    public void Attack(InputAction.CallbackContext context)
    {
        if(!isDead && context.performed && view.IsMine)
        {
            /*
            if(attackNum == 1)
            {
                animator.SetTrigger("Attack1");
                attackNum = 2;
            }else if(attackNum == 2)
            {
                animator.SetTrigger("Attack2");
                attackNum = 1;
            }
            */

            animator.SetTrigger("Attack1");

            Collider2D hitEnemy = Physics2D.OverlapCircle(attackPoint.position, attackRange, enemyLayer);

            Debug.Log("We hit " + hitEnemy.name);
            hitEnemy.GetComponent<TakeDmg>().TakeDamage(damage);
            
            /*
            foreach(Collider2D enemy in hitEnemy)
            {
                Debug.Log("We hit " + enemy.name);
                enemy.GetComponent<TakeDmg>().TakeDamage(damage);
            }
            */
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
