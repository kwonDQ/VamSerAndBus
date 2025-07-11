using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnZone : MonoBehaviour
{
    public GameObject[] EnemyPrefabs;
    public float SpawnInterval;
    public float SpawnDistance;
    public float SpawnAngleStart; // 0도가 x+ 방향, 시계방향으로 커짐
    public float SpawnAngleEnd;
    public float DespawnDistance;

    private GameObject Player;
    private Coroutine SpawnRoutine;
    private List<EnemyInfo> ActiveEnemies = new List<EnemyInfo>();

    class EnemyInfo
    {
        public GameObject EnemyObject;
        public GameObject EnemyPrefab; // 어떤 프리팹으로 만들어졌는지 추적
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player = other.gameObject;
            SpawnRoutine = StartCoroutine(SpawnEnemies());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopCoroutine(SpawnRoutine);
            // DespawnAll();
            // Player = null;
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (EnemyPrefabs.Length > 0)
            {
                // 플레이어 기준 랜덤 방향에서 일정 거리 떨어진 위치
                float angle = Random.Range(SpawnAngleStart * Mathf.PI / 180, SpawnAngleEnd * Mathf.PI / 180);
                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * SpawnDistance;
                Vector2 spawnPos = (Vector2)Player.transform.position + offset;

                GameObject enemyPrefab = EnemyPrefabs[Random.Range(0, EnemyPrefabs.Length)];
                GameObject enemy = EnemyPoolManager.Instance.GetEnemy(enemyPrefab);

                enemy.transform.position = spawnPos;
                enemy.transform.rotation = Quaternion.identity;

                enemy.GetComponent<EnemyAIShooter>().SetTarget(Player.transform); // 적 플레이어 추적용

                ActiveEnemies.Add(new EnemyInfo { EnemyObject = enemy, EnemyPrefab = enemyPrefab });
            }
            yield return new WaitForSeconds(SpawnInterval);
        }
    }

    void Update()
    {
        for (int i = ActiveEnemies.Count - 1; i >= 0; i--)
        {
            var enemyInfo = ActiveEnemies[i];

            if (enemyInfo.EnemyObject == null || !enemyInfo.EnemyObject.activeInHierarchy)
            {
                ActiveEnemies.RemoveAt(i); // 체력이 없어 죽은 적 리스트에서 제거
                continue;
            }

            if (Vector2.Distance(Player.transform.position, enemyInfo.EnemyObject.transform.position) > DespawnDistance)
            {
                EnemyPoolManager.Instance.ReturnEnemy(enemyInfo.EnemyPrefab, enemyInfo.EnemyObject);
                ActiveEnemies.RemoveAt(i); // 너무 멀어진 적 리스트에서 제거
            }
        }
    }
    /**
    void DespawnAll()
    {
        foreach (var enemyInfo in ActiveEnemies)
        {
            EnemyPoolManager.Instance.ReturnEnemy(enemyInfo.EnemyPrefab, enemyInfo.EnemyObject);
        }
        ActiveEnemies.Clear();
    }
    */
}
