using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

 public class AttackCooldown : MonoBehaviour
 {
     public Animator attack_anim;
     public Button attack_btn;

     // Start is called before the first frame update
     void Start(){
         attack_btn.interactable = true;
     }

     void Awake()
     {
         attack_btn.onClick.AddListener(Button_pressed);
     }

     void Button_pressed(){
         StartCoroutine(AttackCooldownStart());
     }

     IEnumerator AttackCooldownStart(){
       attack_anim.SetTrigger("AttackCooldown");
       attack_btn.interactable = false;
       yield return new WaitForSecondsRealtime(1);
       Debug.Log("1 sec");
       attack_btn.interactable = true;
     }
 }