using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D; // SpriteShapeController

[ExecuteAlways]
[RequireComponent(typeof(SpriteShapeController))]
public class RoadPath : MonoBehaviour
{
    [Header("Sampling")]
    [Range(0.5f, 5f)] public float SampleStep = 1.0f;   // 포인트 간 간격(대충 이 정도 길이마다 한 점)
    public bool AutoRebuild = true;                     // 에디터에서 포인트 움직이면 자동 갱신

    [Header("Gizmos")]
    public bool DrawGizmos = true;
    public float TangentScale = 0.8f;
    public Color PointColor = Color.cyan;
    public Color TangentColor = Color.yellow;

    // 경로 데이터 -------------------------
    public List<Vector2> Points = new List<Vector2>();          // 경로 위의 포인트(월드 좌표)
    public List<float> CumulativeLengths = new List<float>();   // 각 포인트까지의 누적 거리
    public float TotalLength { get; private set; }              // 전체 경로 길이
    // ----------------------------------

    SpriteShapeController _spriteShape;

    void OnEnable()
    {
        _spriteShape = GetComponent<SpriteShapeController>();
        Rebuild();
    }

    void Update()
    {
        // 플레이 중이 아닐 때, 에디터에서 움직이면 자동 갱신
        if (!Application.isPlaying && AutoRebuild)
            Rebuild();
    }

    /// <summary>
    /// 스플라인 전체를 Bezier로 샘플링해서 Points / CumulativeLengths를 다시 만든다.
    /// </summary>
    public void Rebuild()
    {
        Points.Clear();
        CumulativeLengths.Clear();
        TotalLength = 0f;

        if (_spriteShape == null) _spriteShape = GetComponent<SpriteShapeController>();
        var spline = _spriteShape.spline;
        int knotCount = spline.GetPointCount();
        if (knotCount < 2) return;

        // 시작점 (로컬 → 월드)
        Vector2 prev = transform.TransformPoint(spline.GetPosition(0));
        Points.Add(prev);
        CumulativeLengths.Add(0f);

        for (int i = 0; i < knotCount - 1; i++)
        {
            // 세그먼트 i ~ i+1의 Bezier 제어점 (로컬 좌표 기준)
            Vector2 p0Local = spline.GetPosition(i);
            Vector2 p3Local = spline.GetPosition(i + 1);
            Vector2 r1Local = spline.GetRightTangent(i);      // p0에서 나가는 핸들 벡터
            Vector2 l2Local = spline.GetLeftTangent(i + 1);   // p3로 들어오는 핸들 벡터

            // Bezier 제어점 4개(로컬)
            Vector2 p0 = p0Local;
            Vector2 p1 = p0Local + r1Local;
            Vector2 p3 = p3Local;
            Vector2 p2 = p3Local + l2Local;

            // 이 구간의 대략적인 길이 계산 → 몇 번 샘플링할지 정하기
            float approx = ApproxBezierLength(p0, p1, p2, p3);
            int steps = Mathf.Max(2, Mathf.RoundToInt(approx / Mathf.Max(0.1f, SampleStep)));

            for (int s = 1; s <= steps; s++)
            {
                float t = (float)s / steps;
                Vector2 pLocal = EvalBezier(p0, p1, p2, p3, t);
                Vector2 pWorld = transform.TransformPoint(pLocal); // 로컬 → 월드

                Points.Add(pWorld);

                // 이전 포인트에서 현재 포인트까지 거리 누적
                TotalLength += Vector2.Distance(prev, pWorld);
                CumulativeLengths.Add(TotalLength);

                prev = pWorld;
            }
        }
    }

    // Cubic Bezier 평가: P(t) = (1-t)^3 p0 + 3(1-t)^2 t p1 + 3(1-t) t^2 p2 + t^3 p3
    Vector2 EvalBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float u = 1f - t;
        float uu = u * u;
        float uuu = uu * u;
        float tt = t * t;
        float ttt = tt * t;
        return uuu * p0 + 3f * uu * t * p1 + 3f * u * tt * p2 + ttt * p3;
    }

    // Bezier 길이 대략 계산 (N등분 폴리라인 길이)
    float ApproxBezierLength(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        const int N = 8;
        float len = 0f;
        Vector2 prev = p0;
        for (int i = 1; i <= N; i++)
        {
            float t = (float)i / N;
            Vector2 pt = EvalBezier(p0, p1, p2, p3, t);
            len += Vector2.Distance(prev, pt);
            prev = pt;
        }
        return len;
    }

    // 특정 "거리" 위치에서의 포인트를 Lerp로 얻기
    public Vector2 GetPositionAtDistance(float distance)
    {
        if (Points.Count == 0) return transform.position;
        if (TotalLength <= 0f) return Points[0];

        // 루프 도로 느낌 내고 싶으면 Repeat, 왕복/끝에서 멈추게 하고 싶으면 Clamp
        distance = Mathf.Repeat(distance, TotalLength);

        // 선형 탐색 (지금은 단순하게; 나중에 필요하면 이분 탐색도 가능)
        for (int i = 1; i < CumulativeLengths.Count; i++)
        {
            if (CumulativeLengths[i] >= distance)
            {
                float s0 = CumulativeLengths[i - 1];
                float s1 = CumulativeLengths[i];
                float t = (distance - s0) / Mathf.Max(0.0001f, s1 - s0);
                return Vector2.Lerp(Points[i - 1], Points[i], t);
            }
        }

        // 혹시 루프가 안 잡혔으면 마지막 포인트
        return Points[Points.Count - 1];
    }

    // 특정 거리 위치에서의 진행 방향(접선) 벡터
    public Vector2 GetTangentAtDistance(float distance)
    {
        float epsilon = 0.1f;
        Vector2 p0 = GetPositionAtDistance(distance - epsilon);
        Vector2 p1 = GetPositionAtDistance(distance + epsilon);
        Vector2 dir = (p1 - p0).normalized;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.up;
        return dir;
    }

    // 월드 좌표에서 가장 가까운 포인트를 찾아, 그 포인트까지의 "거리" 값을 반환
    public float GetClosestDistance(Vector2 worldPos)
    {
        if (Points.Count == 0) return 0f;

        int bestIndex = 0;
        float bestDist = float.MaxValue;

        // 가장 가까운 포인트 찾기
        for (int i = 0; i < Points.Count; i++)
        {
            float d = (worldPos - Points[i]).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                bestIndex = i;
            }
        }

        // 해당 포인트의 누적 거리를 distance로 사용
        if (bestIndex < CumulativeLengths.Count)
            return CumulativeLengths[bestIndex];
        else
            return 0f;
    }

    void OnDrawGizmos()
    {
        if (!DrawGizmos || Points.Count < 2) return;

        Gizmos.color = PointColor;
        foreach (var p in Points)
            Gizmos.DrawSphere(p, 0.06f);

        Gizmos.color = TangentColor;
        for (int i = 0; i < Points.Count - 1; i++)
        {
            Vector2 a = Points[i];
            Vector2 b = Points[i + 1];
            Vector2 dir = (b - a).normalized;
            Gizmos.DrawLine(a, a + dir * TangentScale);

            Vector2 left = new Vector2(-dir.y, dir.x) * (TangentScale * 0.25f);
            Gizmos.DrawLine(a + dir * TangentScale, a + dir * TangentScale - dir * 0.2f + left * 0.5f);
            Gizmos.DrawLine(a + dir * TangentScale, a + dir * TangentScale - dir * 0.2f - left * 0.5f);
        }
    }
}
