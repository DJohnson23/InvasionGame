using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class GameManager : MonoBehaviour
{
    public static bool paused { get; private set; } = false;

    public GameObject pauseUI;
    public GameObject gameUI;
    public Animator fadeAnimator;

    bool loadingScene = false;
    AsyncOperation loadOperation;

    Health health;

    private void Awake()
    {
        GameManager[] gms = FindObjectsOfType<GameManager>();

        if(gms.Length > 1)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (pauseUI)
        {
            pauseUI.SetActive(false);
        }

        paused = false;
        Time.timeScale = 1;
        health = GetComponent<Health>();
        fadeAnimator.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandlePause();
    }

    public void DamagePlayer(float damage)
    {
        health.TakeDamage(damage);
    }

    public void OnDeath()
    {
        //Time.timeScale = 0;
        fadeAnimator.gameObject.SetActive(true);
        fadeAnimator.SetTrigger("transition");
        StartCoroutine(DelayRestart(1));
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
    }
}
