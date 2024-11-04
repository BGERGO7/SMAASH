using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCooldowns : MonoBehaviour
{

    public Animator attack_anim;
    public Button attack_btn;

    public Animator jump_anim;
    public Button jump_btn;

    public bool isDead = false;


    // Start is called before the first frame update
    void Start()
    {
        attack_btn.interactable = true;
        jump_btn.interactable = true;
    }

    void Awake()
    {
        attack_btn.onClick.AddListener(Attack_Button_pressed);
        jump_btn.onClick.AddListener(Jump_Button_pressed);
    }

    void Attack_Button_pressed(){
        if(isDead == false){
            StartCoroutine(AttackCooldownStart());
        }
    }

    void Jump_Button_pressed(){
        if(isDead == false){
            StartCoroutine(JumpCooldownStart());
        }
    }

    IEnumerator AttackCooldownStart(){
       attack_anim.SetTrigger("AttackCooldown");
       attack_btn.interactable = false;
       yield return new WaitForSecondsRealtime(1);
       
       attack_btn.interactable = true;
    }

    IEnumerator JumpCooldownStart(){
       jump_anim.SetTrigger("isJumping");
       jump_btn.interactable = false;
       yield return new WaitForSecondsRealtime(.5f);
       jump_btn.interactable = true;
     }

}
