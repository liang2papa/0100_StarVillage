using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [Lv 4] 실행 모듈
/// </summary>
public class InteractionModule : MonoBehaviour
{
    [Header("감지 관련 설정")]
    [SerializeField] private GameObject m_entityChecker;
    [SerializeField] private LayerMask m_interactiveLayer;
    [SerializeField] private Collider[] m_colliderBuffer = new Collider[10];
    [field: SerializeField] public IInteractable CurrentTarget { get; private set; }

    private const float SCORE_UNCHECKED_BONUS = 1000f; // 미확인 컨테이너 가중치
    private const float SCORE_EMPTY_PENALTY = 500f; // 빈 컨테이너 감점
    private const float SCORE_DISTANCE_PENALTY= 10f; // 거리 1m당 감점

    private void Update()
    {
        // [추가] 매 프레임 버퍼를 깨끗하게 비움 (잔여 데이터 제거)
        // Array.Clear(배열, 시작인덱스, 길이)
        Array.Clear(m_colliderBuffer, 0, m_colliderBuffer.Length);

        // 박스 사이즈 계산 (필요하다면 캐싱 고려 가능)
        Vector3 boxSize = transform.localScale * 3f;

        // [수정 2] 반환값(hitCount)을 받아서 그만큼만 반복
        int hitCount = Physics.OverlapBoxNonAlloc(
            m_entityChecker.transform.position,
            boxSize * 0.5f,
            m_colliderBuffer,
            Quaternion.identity,
            m_interactiveLayer
        );

        IInteractable bestTarget = null;
        float bestScore = float.MinValue; // 점수는 높을수록 좋으므로 최솟값부터 시작

        for (int i = 0; i < hitCount; i++)
        {
            Collider col = m_colliderBuffer[i];
            if (col.gameObject == gameObject) continue;

            if (col.TryGetComponent<IInteractable>(out var target))
            {
                // 점수 계산 시작
                float currentScore = 0f;

                // 1. 거리 페널티 (거리가 멀수록 점수 하락)
                // sqrMagnitude를 쓰면 값이 너무 커지므로, 정확한 비교를 위해 Vector3.Distance 권장
                // 혹은 단순 비교용이라면 sqr도 괜찮지만, 선형적인 감점을 위해 Distance 사용
                float dist = Vector3.Distance(transform.position, col.transform.position);
                currentScore -= (dist * SCORE_DISTANCE_PENALTY);

                // 2. 방문 여부 보너스 (핵심 로직)
                // 타겟이 인터렉티브 엔티티인 경우
                if (target is LootableEntity)
                {
                    var lootable = target as LootableEntity;
                    if (lootable != null)
                    {
                        if (!lootable.IsChecked)
                        {
                            currentScore += SCORE_UNCHECKED_BONUS;
                        }
                        if (lootable.IsEmpty)
                        {
                            currentScore -= SCORE_EMPTY_PENALTY;
                        }
                    }
                }
                    

                // 3. (선택사항) 각도 보너스: 내 정면에 있을수록 가산점
                // 카메라나 캐릭터가 바라보는 방향과의 내적(Dot Product) 이용
                /*
                Vector3 dirToTarget = (col.transform.position - transform.position).normalized;
                float dot = Vector3.Dot(transform.forward, dirToTarget);
                if (dot > 0.5f) // 전방 60도 이내
                {
                    currentScore += (dot * 50f); // 정면일수록 최대 50점 추가
                }
                */

                // 최고 점수 갱신
                if (currentScore > bestScore)
                {
                    bestScore = currentScore;
                    bestTarget = target;
                }
            }
        }
        // [수정 4] 루프가 다 끝난 뒤에 최종 결과를 할당
        // 루프 안에서 할당하면, 가까운 걸 찾았다가 먼 걸 검사할 때 덮어씌워질 수 있음
        // 만약 hitCount가 0이거나 유효한 엔티티가 없으면 nearestEntity는 null이 되므로 자연스럽게 초기화됨
        CurrentTarget = bestTarget;
    }
    private void OnDrawGizmos()
    {
        // Vector3 boxSize = new Vector3(transform.lossyScale.x * 3, transform.lossyScale.y * 3, transform.lossyScale.z * 3);
        Vector3 boxSize = transform.localScale * 3;
        Gizmos.color = Color.azure;
        Gizmos.DrawWireCube(m_entityChecker.transform.position, boxSize);
    }
}
