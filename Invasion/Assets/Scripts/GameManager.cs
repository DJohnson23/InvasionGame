using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class GameManager : MonoBehaviour
{
    public static int enemiesLeft { get; private set; } = 0;
    public static float mouseSensitivity { get; private set; } = 2.5f;
    public static bool paused { get; private set; } = false;
    public static GameManager instance { get; private set; }

    public GameObject pauseUI;
    public GameObject gameUI;
    public Animator fadeAnimator;

    bool loadingScene = false;
    AsyncOperation loadOperation;

    public Health health;
    public Health shield;
    public Text ammoText;
    public Text reloadText;
    public Text enemiesLeftText;
    public Slider mouseSensitivitySlider;


    private void Awake()
    {
        GameManager[] gms = FindObjectsOfType<GameManager>();

        if(gms.Length > 1)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (pauseUI)
        {
            pauseUI.SetActive(false);
        }

        reloadText.gameObject.SetActive(false);
        paused = false;
        Time.timeScale = 1;
        health = GetComponent<Health>();
        fadeAnimator.gameObject.SetActive(false);
        mouseSensitivity = mouseSensitivitySlider.value;
    }

    private void Update()
    {
        HandlePause();
    }

    public void StartReloading()
    {
        reloadText.gameObject.SetActive(true);
    }

    public void StopReloading()
    {
        reloadText.gameObject.SetActive(false);
    }

    public void UpdateAmmo(int ammo)
    {
        ammoText.text = "x " + ammo;
    }

    public bool HealPlayer(int amount)
    {
        if(health.health == health.MaxHealth)
        {
            return false;
        }

        health.Heal(amount);
        return true;
    }

    public void OnMouseSensitivityChanged()
    {
        mouseSensitivity = mouseSensitivitySlider.value;
    }

    public bool AddShield(int amount)
    {
        if(shield.health == shield.MaxHealth)
        {
            return false;
        }

        shield.Heal(amount);
        return true;
    }

    public void DamagePlayer(float damage)
    {
        float damageLeft = damage;

        if(shield.health > 0)
        {
            damageLeft = damage - shield.health;
            shield.TakeDamage(damage);
        }

        if(damageLeft > 0)
        {
            health.TakeDamage(damageLeft);
        }

        if(health.health <= 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        //Time.timeScale = 0;
        fadeAnimator.gameObject.SetActive(true);
        fadeAnimator.SetTrigger("transition");
        StartCoroutine(DelayRestart(1));
    }

    public static void AddEnemy()
    {
        enemiesLeft++;
        instance.enemiesLeftText.text = "x " + enemiesLeft;
    }

    public static void RemoveEnemy()
    {
        enemiesLeft--;
        instance.enemiesLeftText.text = "x " + enemiesLeft;
    }
    
    IEnumerator DelayRestart(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        loadOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        loadOperation.completed += DeathRestart;
    }

    IEnumerator HideAfterFade(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        fadeAnimator.gameObject.SetActive(false);
    }

    void DeathRestart(AsyncOperation operation)
    {
        fadeAnimator.SetTrigger("transition");
        health.SetHealth(health.MaxHealth);
        shield.SetHealth(0);
        StartCoroutine(HideAfterFade(1));
    }

    public void SetGameUIActive(bool active)
    {
        if(gameUI)
        {
            gameUI.SetActive(active);
        }
    }

    void HandlePause()
    {
        if(loadOperation != null && !loadOperation.isDone)
        {
            return;
        }

        int curScene = SceneManager.GetActiveScene().buildIndex;

        if (curScene > 0 && (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        paused = !paused;

        Time.timeScale = paused ? 0 : 1;
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;

        if (pauseUI)
        {
            pauseUI.SetActive(paused);
        }
    }

    public void RestartLevel()
    {
        if(paused)
        {
            TogglePause();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        if(paused)
        {
            TogglePause();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ReturnToMain()
    {
        if(paused)
        {
            TogglePause();
        }
        SceneManager.LoadScene(0);
        health.SetHealth(health.MaxHealth);
        shield.SetHealth(0);
    }
}
