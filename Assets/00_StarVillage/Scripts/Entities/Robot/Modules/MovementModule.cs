using UnityEngine;

/// <summary>
/// [Lv 4] 이동 모듈, 카메라를 기준으로 움직임 방향 계산
/// </summary>
public class MovementModule : MonoBehaviour
{
    [SerializeField] private Rigidbody m_rb;
    [SerializeField] private RobotDataSO m_robotData;
    [SerializeField] private Transform m_cameraTransform;
    [SerializeField] private Camera m_mainCamera;


    [Header("Ground Settings")]
    [SerializeField] private LayerMask m_groundLayer;

    [Header("GroundChecker")]
    [SerializeField] private Transform m_groundChecker; // 바닥 체크용 트랜스폼, 발판에 붙어있음

    [Header("Slope Settings")]
    private RaycastHit m_slopeHit;
    private const float SRayDistance = 3f; // 슬로프 체크를 위한 레이의 길이
    private const float SMaxSlopeAngle = 45f; // 등반할 수 있는 최대각도

    [Header("Wall Settings")]
    [SerializeField] private LayerMask m_wallLayer; // Wall 레이어 (없으면 Default나 Obstacle)
    [SerializeField] private float m_wallCheckDist = 0.7f; // 캡슐 반지름 + 약간

    [SerializeField] private bool isGrounded = false;

    public void InitClass(Rigidbody rb, RobotDataSO data, Transform cameraTransform)
    {
        m_rb = rb;
        m_robotData = data;
        m_cameraTransform = cameraTransform;
        cameraTransform.TryGetComponent<Camera>(out m_mainCamera);
    }
    public void MovementFixed(Vector2 inputDir)
    {
        // 1. 입력이 없으면 정지
        if (inputDir.sqrMagnitude < 0.01f)
        {
            if (CheckIsGrounded()) m_rb.linearVelocity = Vector3.zero;
            else m_rb.linearVelocity = new Vector3(0, m_rb.linearVelocity.y, 0);

            m_rb.angularVelocity = Vector3.zero;
            return;
        }

        // 2. 방향 계산
        Vector3 camForward = m_cameraTransform.forward;
        Vector3 camRight = m_cameraTransform.right;
        camForward.y = 0; camRight.y = 0;
        camForward.Normalize(); camRight.Normalize();

        Vector3 moveDir = (camForward * inputDir.y + camRight * inputDir.x).normalized;

        // ---------------------------------------------------------
        // [추가] 벽 타기 (Wall Sliding) 로직
        // ---------------------------------------------------------
        // 로봇 허리춤에서 진행 방향으로 레이를 쏴서 벽이 있는지 확인
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, moveDir, out RaycastHit wallHit, m_wallCheckDist))
        {
            // 벽이 있다면, 이동 방향을 벽면의 기울기(Normal)에 맞춰서 미끄러지게 꺾어줌
            // ProjectOnPlane: 벡터를 평면에 투영 (벽을 뚫으려는 힘을 제거)
            moveDir = Vector3.ProjectOnPlane(moveDir, wallHit.normal).normalized;
        }
        // ---------------------------------------------------------

        // 3. 속도 적용 (경사면 로직과 결합)
        if (IsOnSlope())
        {
            m_rb.linearVelocity = AdjustDirectionToSlope(moveDir) * m_robotData.MovementSpeed;
        }
        else
        {
            m_rb.linearVelocity = new Vector3(moveDir.x * m_robotData.MovementSpeed, m_rb.linearVelocity.y, moveDir.z * m_robotData.MovementSpeed);
        }

        // 4. [중요] 물리 회전력 완전 제거
        m_rb.angularVelocity = Vector3.zero;
    }
    public void RotationFixed(Vector2 lookInput, Vector2 mousePos, EControlScheme controlScheme)
    {
        Vector3 lookDirection = Vector3.zero;

        // =========================================================
        // [핵심] 조작 모드에 따라 로직을 완전히 분리 (if-else)
        // =========================================================

        // CASE A: 게임패드 모드
        if (controlScheme == EControlScheme.Gamepad)
        {
            // 입력값이 데드존보다 클 때만 계산
            if (lookInput.sqrMagnitude > 0.05f)
            {
                Vector3 camForward = m_cameraTransform.forward;
                Vector3 camRight = m_cameraTransform.right;
                camForward.y = 0; camRight.y = 0;
                camForward.Normalize(); camRight.Normalize();

                lookDirection = (camForward * lookInput.y + camRight * lookInput.x).normalized;
            }
        }
        // CASE B: 키보드/마우스 모드
        else
        {
            if (m_mainCamera == null) return;

            Ray ray = m_mainCamera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, m_groundLayer))
            {
                Vector3 targetPoint = hitInfo.point;
                targetPoint.y = transform.position.y;
                lookDirection = (targetPoint - transform.position).normalized;
            }
        }

        // =========================================================
        // 최종 회전 적용 (즉시 회전)
        // =========================================================
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            m_rb.MoveRotation(targetRotation);
            m_rb.angularVelocity = Vector3.zero;
        }
    }
    private bool IsOnSlope()
    {
        Ray ray = new (transform.position, Vector3.down);
        if (Physics.Raycast(ray, out m_slopeHit, SRayDistance, m_groundLayer))
        {
            var angle = Vector3.Angle(Vector3.up, m_slopeHit.normal);
            return angle != 0f && angle < SMaxSlopeAngle;
        }
        return false;
    }
    /// <summary>
    /// 투영된 벡터 방향 정보
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private Vector3 AdjustDirectionToSlope(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, m_slopeHit.normal).normalized;
    }
    private bool CheckIsGrounded()
    {
        Vector3 boxsize = transform.lossyScale;
        isGrounded = Physics.CheckBox(m_groundChecker.position, boxsize, Quaternion.identity, m_groundLayer);
        return isGrounded;
    }
    private void OnDrawGizmos()
    {
        Vector3 boxsize = transform.lossyScale;
        Gizmos.DrawWireCube(m_groundChecker.position, boxsize);
    }

}