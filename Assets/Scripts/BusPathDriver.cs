using UnityEngine;

// 알고리즘 요약:
// 1) 시작할 때, 버스의 현재 위치에서 도로 상 "가장 가까운 거리"를 구해서 DistanceAlongPath에 저장.
// 2) 매 프레임마다 DistanceAlongPath에 속도 * dt를 더해서 앞으로 전진.
// 3) 그 거리에서의 위치/방향을 RoadPath에게 물어보고, Transform에 반영.
// 4) 이렇게 하면 버스는 항상 도로 위를 "레일 타듯" 움직인다.

public class BusPathDriver : MonoBehaviour
{
    public RoadPath Path;           // 따라갈 도로
    public float MoveSpeed = 5f;    // 초당 몇 유닛 전진할지
    public float DistanceAlongPath; // 도로 위에서 얼마나 진행했는지 (내부 상태)

    void Start()
    {
        if (Path == null || Path.TotalLength <= 0f) return;

        // 시작 위치를 도로 위 최근접 거리로 맞춰줌
        Vector2 startPos = transform.position;
        DistanceAlongPath = Path.GetClosestDistance(startPos);
    }

    void Update()
    {
        if (Path == null || Path.TotalLength <= 0f) return;

        // 1) 거리 증가 → 앞으로 전진
        DistanceAlongPath += MoveSpeed * Time.deltaTime;

        // 2) 해당 거리에서 위치, 방향 가져오기
        Vector2 pos = Path.GetPositionAtDistance(DistanceAlongPath);
        Vector2 tangent = Path.GetTangentAtDistance(DistanceAlongPath);

        // 3) 위치 적용
        transform.position = pos;

        // 4) 방향 적용
        //   - tangent는 "도로가 진행되는 방향"
        //   - 스프라이트가 "위쪽(↑)"을 앞이라고 가정하면, 회전을 이렇게 맞춘다.
        float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
