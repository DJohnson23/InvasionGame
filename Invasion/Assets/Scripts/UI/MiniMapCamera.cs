using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MiniMapCamera : MonoBehaviour
{
    public Vector3 offset;
    public GameObject enemyIcon;
    public RectTransform miniMap;
    public Vector2 iconSize = new Vector2(0.06f, 0.06f);

    GameObject player;
    Camera cam;
    List<GameObject> enemyIcons;

    float iconWidth;
    float iconHeight;

    private void Start()
    {
        Refresh();
    }

    void Refresh()
    {
        cam = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemyIcons = new List<GameObject>();
        iconWidth = miniMap.rect.width * iconSize.x;
        iconHeight = miniMap.rect.height * iconSize.x;
        if(player != null)
        {
            transform.position = player.transform.position + offset;
        }
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");

            if(player == null)
            {
                return;
            }
        }

        transform.position = player.transform.position + offset;

        DrawIcons();
    }

    void DrawIcons()
    {
        DrawEnemies();
    }

    void DrawEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float cameraSize = cam.orthographicSize;

        for(int i = 0; i < enemyIcons.Count; i++)
        {
            Destroy(enemyIcons[i].gameObject);
        }

        enemyIcons.Clear();

        for(int i = 0; i < enemies.Length; i++)
        {
            GameObject enemy = enemies[i];
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if(enemyController != null && enemyController.IsDead)
            {
                continue;
            }

            float distance = FlatDistance(transform.position, enemy.transform.position);

            if (FlatDistance(transform.position, enemy.transform.position) < cameraSize * 1.25f)
            {
                GameObject icon = Instantiate(enemyIcon, miniMap);
                enemyIcons.Add(icon);

                float x = (enemy.transform.position.x - transform.position.x) / cameraSize * (miniMap.rect.width / 2);
                float y = (enemy.transform.position.z - transform.position.z) / cameraSize * (miniMap.rect.height / 2);

                RectTransform iconTransform = (RectTransform)icon.transform;
                iconTransform.anchoredPosition = new Vector3(x, y, 0);
                iconTransform.localRotation = Quaternion.Euler(0, 0, -enemy.transform.eulerAngles.y);
            }
        }
    }

    float FlatDistance(Vector3 a, Vector3 b)
    {
        Vector3 vecBtw = b - a;
        vecBtw.y = 0;

        return vecBtw.magnitude;
    }

    private void OnValidate()
    {
        Refresh();
    }
}
