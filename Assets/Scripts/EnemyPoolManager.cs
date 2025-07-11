using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    [System.Serializable] // EnemyPool을 클래스 유니티 에디터 인스펙터 창에 보이도록
    public class EnemyPool // Enemy를 미리 생성해놓고 저장하는 클래스
    {
        public GameObject EnemyPrefab;
        public int PoolSize;
        [HideInInspector] public Queue<GameObject> PoolQueue = new Queue<GameObject>();
    }

    public static EnemyPoolManager Instance; // 싱글톤으로 전역 접근 << 다른 스크립트에서 이 스크립트 접근 위해서

    public EnemyPool[] EnemyPools; // 인스펙터창에서 구성요소 설정해야함 EnemyPrefab는 이 맵에서 쓸 적들 모두 넣고 PoolSize는 적당히 최적화되게

    private void Awake()
    {
        if (Instance == null) Instance = this; // 이게 붙은 오브젝트 EnemyPoolManager가 대표 인스턴스가 됨
        else Destroy(gameObject);

        InitializePools();
    }

    void InitializePools() // 미리 적을 소환해서 준비시키는 함수
    {
        foreach (var pool in EnemyPools) // EnemyPools에 있는 모든 프리펩을 PoolSize개수 만큼 EnemyPrefab의 클론 오브젠트 생성
        {
            for (int i = 0; i < pool.PoolSize; i++)
            {
                GameObject obj = Instantiate(pool.EnemyPrefab);
                obj.SetActive(false);
                pool.PoolQueue.Enqueue(obj);
            }
        }
    }

    public GameObject GetEnemy(GameObject prefab) // 풀의 적을 활성화하는 함수
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
            GameObject enemy = Instantiate(prefab); // 풀에 여유 없을 경우 생성 (옵션)
            return enemy;
        }
    }

    public void ReturnEnemy(GameObject prefab, GameObject enemy) // 풀의 적을 비활성화하는 함수
    {
        enemy.SetActive(false);
        EnemyPool pool = FindPool(prefab);
        if (pool != null)
            pool.PoolQueue.Enqueue(enemy);
        else
            Destroy(enemy); // 풀을 못 찾으면 파괴, 정상적인 프리펩이 넘어오지 않은 경우를 위한 예외처리 문구, 일반적으로 작동안함
    }

    private EnemyPool FindPool(GameObject prefab) // 이 프리펩이 어떤 풀에 속하는지 확인
    {
        foreach (var pool in EnemyPools) // EnemyPools에 있는 프리펩중에 준비되있는 풀 찾아서 있으면 반환 없으면 null
        {
            if (pool.EnemyPrefab.name == prefab.name)
                return pool;
        }
        return null;
    }
}
