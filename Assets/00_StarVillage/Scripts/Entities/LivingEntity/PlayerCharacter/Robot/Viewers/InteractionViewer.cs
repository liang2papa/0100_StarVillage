using TMPro;
using UnityEngine;

/// <summary>
/// [Lv 4] 플레이어가 상호작용 중인 객체의 메뉴를 시각적으로 표시하는 뷰어
/// (World Space Canvas에 배치된 독립 오브젝트)
/// </summary>
public class InteractionViewer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 m_offset = new Vector3(1.5f, 2.0f, 0); // 대상 우측 상단 오프셋

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI m_interactionTxt;

    private Transform m_cameraTransform;
    private Transform m_targetTransform;

    private bool m_isInitialized = false;

    public void InitClass(Transform cameraTransform)
    {
        m_interactionTxt = GetComponentInChildren<TextMeshProUGUI>();
        m_cameraTransform = cameraTransform;

        m_isInitialized = true;

        // 시작 시 비활성화
        gameObject.SetActive(false);
    }

    // 매 프레임 호출되어도 부하가 없도록 최적화
    public void DisplayUI(Transform targetPos, string message)
    {
        if (!m_isInitialized) return;

        m_targetTransform = targetPos;
        m_interactionTxt.text = message; // 텍스트만 갱신

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

    }
    public void Hide()
    {
        if (gameObject.activeSelf)
        {
            m_interactionTxt.text = "";
            gameObject.SetActive(false);
        }
    }
    private void LateUpdate()
    {
        if (!gameObject.activeSelf || m_targetTransform == null) return;

        // 1. 위치 동기화
        transform.position = m_targetTransform.position + m_offset;

        // 2. 빌보드 처리
        if (m_cameraTransform != null)
        {
            transform.rotation = m_cameraTransform.rotation;
        }
    }


}