using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    [System.Serializable] // EnemyPool�� Ŭ���� ����Ƽ ������ �ν����� â�� ���̵���
    public class EnemyPool // Enemy�� �̸� �����س��� �����ϴ� Ŭ����
    {
        public GameObject EnemyPrefab;
        public int PoolSize;
        [HideInInspector] public Queue<GameObject> PoolQueue = new Queue<GameObject>();
    }

    public static EnemyPoolManager Instance; // �̱������� ���� ���� << �ٸ� ��ũ��Ʈ���� �� ��ũ��Ʈ ���� ���ؼ�

    public EnemyPool[] EnemyPools; // �ν�����â���� ������� �����ؾ��� EnemyPrefab�� �� �ʿ��� �� ���� ��� �ְ� PoolSize�� ������ ����ȭ�ǰ�

    private void Awake()
    {
        if (Instance == null) Instance = this; // �̰� ���� ������Ʈ EnemyPoolManager�� ��ǥ �ν��Ͻ��� ��
        else Destroy(gameObject);

        InitializePools();
    }

    void InitializePools() // �̸� ���� ��ȯ�ؼ� �غ��Ű�� �Լ�
    {
        foreach (var pool in EnemyPools) // EnemyPools�� �ִ� ��� �������� PoolSize���� ��ŭ EnemyPrefab�� Ŭ�� ������Ʈ ����
        {
            for (int i = 0; i < pool.PoolSize; i++)
            {
                GameObject obj = Instantiate(pool.EnemyPrefab);
                obj.SetActive(false);
                pool.PoolQueue.Enqueue(obj);
            }
        }
    }

    public GameObject GetEnemy(GameObject prefab) // Ǯ�� ���� Ȱ��ȭ�ϴ� �Լ�
    {
        EnemyPool pool = FindPool(prefab);
        if (pool == null) return null;

        if (pool.PoolQueue.Count > 0)
        {
            GameObject enemy = pool.PoolQueue.Dequeue();
            enemy.SetActive(true);
            return enemy;
        }
        else
        {
            GameObject enemy = Instantiate(prefab); // Ǯ�� ���� ���� ��� ���� (�ɼ�)
            return enemy;
        }
    }

    public void ReturnEnemy(GameObject prefab, GameObject enemy) // Ǯ�� ���� ��Ȱ��ȭ�ϴ� �Լ�
    {
        enemy.SetActive(false);
        EnemyPool pool = FindPool(prefab);
        if (pool != null)
            pool.PoolQueue.Enqueue(enemy);
        else
            Destroy(enemy); // Ǯ�� �� ã���� �ı�, �������� �������� �Ѿ���� ���� ��츦 ���� ����ó�� ����, �Ϲ������� �۵�����
    }

    private EnemyPool FindPool(GameObject prefab) // �� �������� � Ǯ�� ���ϴ��� Ȯ��
    {
        foreach (var pool in EnemyPools) // EnemyPools�� �ִ� �������߿� �غ���ִ� Ǯ ã�Ƽ� ������ ��ȯ ������ null
        {
            if (pool.EnemyPrefab.name == prefab.name)
                return pool;
        }
        return null;
    }
}
