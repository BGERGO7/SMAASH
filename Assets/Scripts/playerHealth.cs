using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

	public int maxHealth = 100;
	public int currentHealth;

	public HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
		currentHealth = maxHealth;
		healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			TakeDamage(10);
		}

        if (Input.GetKeyDown(KeyCode.H))
		{
			AddHealth(10);
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