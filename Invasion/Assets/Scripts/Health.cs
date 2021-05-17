using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField]
    int maxHealth = 100;

    public int MaxHealth
    {
        get
        {
            return maxHealth;
        }
    }

    public Slider healthbar;
    public Gradient colorGradient;
    public bool showAlways = false;
    public bool startAtMax = true;
    public float showCooldown = 1f;

    Image healthbarFill;
    public int health { get; private set; }
    public bool isDead
    {
        get
        {
            return health <= 0;
        }
    }

    Coroutine showHealthbarCooldown;

    // Start is called before the first frame update
    void Start()
    {
        if(startAtMax)
        {
            health = maxHealth;
        }
        InitHealthbar();
    }

    public void SetHealthbar(Slider newBar)
    {
        healthbar = newBar;
        InitHealthbar();
        
    }

    void InitHealthbar()
    {
        if (healthbar != null)
        {
            healthbarFill = healthbar.fillRect.GetComponent<Image>();
            if (!showAlways)
            {
                healthbar.gameObject.SetActive(false);
            }

            healthbarFill.color = colorGradient.Evaluate(Mathf.InverseLerp(0f, maxHealth, health));
            UpdateBar();
        }
    }

    public void TakeDamage(float damage)
    {
        TakeDamage((int)damage);
    }

    public void TakeDamage(int damage)
    {
        if(isDead)
        {
            return;
        }

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        float perc = Mathf.InverseLerp(0f, maxHealth, health);

        if(healthbar != null)
        {
            UpdateBar(perc);

            if(!showAlways)
            {
                if (showHealthbarCooldown != null)
                {
                    StopCoroutine(showHealthbarCooldown);
                }
                showHealthbarCooldown = StartCoroutine(ShowBarCooldown());
            }
        }
    }

    void UpdateBar(float t)
    {
        healthbar.value = t;
        healthbarFill.color = colorGradient.Evaluate(t);
        healthbarFill.gameObject.SetActive(health > 0);
    }

    void UpdateBar()
    {
        UpdateBar(Mathf.InverseLerp(0f, maxHealth, health));
    }

    IEnumerator ShowBarCooldown()
    {
        healthbar.gameObject.SetActive(true);
        yield return new WaitForSeconds(showCooldown);

        healthbar.gameObject.SetActive(false);
    }

    public void SetMaxHealth(int newMax, bool scaleHealth = false)
    {
        if(scaleHealth)
        {
            float perc = Mathf.InverseLerp(0f, maxHealth, health);
            maxHealth = newMax;
            health = Mathf.RoundToInt(perc * maxHealth);
        }
        else
        {
            maxHealth = newMax;
            UpdateBar();
        }
    }

    public void SetHealth(int newHealth)
    {
        health = Mathf.Clamp(newHealth, 0, maxHealth);

        UpdateBar();
    }

    public void Heal(int amount)
    {
        SetHealth(health + amount);
    }
}
