using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    PhotonView photonView;

    public Animator animator;

    public PlayerMovement playerMovement;
    public MeleeAttack meleeAttack;
    public JumpCooldown jumpCooldown;
    public AttackCooldown attackCooldown;
    public ButtonCooldowns buttonCooldowns;

    public HealthBar healthBar;
    public int maxHealth = 100;
    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {

        photonView = GetComponent<PhotonView>();
        if(photonView.Owner.ActorNumber == 1){
            healthBar = GameObject.Find("HealthBar1").GetComponent<HealthBar>();
        }else if(photonView.Owner.ActorNumber == 2){
            healthBar = GameObject.Find("HealthBar2").GetComponent<HealthBar>();
        }
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);    
    
    }

    public void TakeDamageCaller(int damage)
    {
        photonView.RPC("TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (!photonView.IsMine) return;

        currentHealth -= damage;
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            healthBar.SetHealth(currentHealth);
            Die();
        }

        // Frissítjük a saját healthbarunkat
        healthBar.SetHealth(currentHealth);

        // Szinkronizáljuk a sebzést a többiekkel
        photonView.RPC("UpdateHealth", RpcTarget.Others, currentHealth);
    }

    //Halal animacio
    void Die()
    {
        animator.SetBool("isDead", true);
        photonView.RPC("UpdateHealth", RpcTarget.Others, currentHealth);
        playerMovement.isDead = true;
        meleeAttack.isDead = true;
        
        jumpCooldown.enabled = false;
        buttonCooldowns.enabled = false;
        attackCooldown.enabled = false;
    }

    // Ezt a funkciót a többiek healthbarjának frissítéséhez használjuk
    [PunRPC]
    void UpdateHealth(int updatedHealth)
    {
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            healthBar.SetHealth(currentHealth);
        }else
        {
           currentHealth = updatedHealth;
            healthBar.SetHealth(currentHealth); 
        }
    }

    // Új játékos csatlakozásakor frissítjük a health értékeket
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("UpdateHealth", newPlayer, currentHealth);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
