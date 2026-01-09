using UnityEngine;

/// <summary>
/// [Lv 4] 이동 모듈
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

    [Header("Slope")]
    private RaycastHit m_slopeHit;
    private const float SRayDistance = 3f; // 슬로프 체크를 위한 레이의 길이
    private const float SMaxSlopeAngle = 45f; // 등반할 수 있는 최대각도

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
        // 2. 바닥에 서있으면 그냥 멈추고, 아니면 낙하속도를 유지
        if (inputDir.sqrMagnitude < 0.01f)
        {
            if (CheckIsGrounded())
            {
                m_rb.linearVelocity = Vector3.zero;
            }
            else
            {
                m_rb.linearVelocity = new Vector3(0, m_rb.linearVelocity.y, 0);
            }

            m_rb.angularVelocity = Vector3.zero;
            return;
        }
        // 2. 카메라 기준 방향 계산
        Vector3 camForward = m_cameraTransform.forward;
        Vector3 camRight = m_cameraTransform.right;
        camForward.y = 0; 
        camRight.y = 0;
        camForward.Normalize(); 
        camRight.Normalize();

        Vector3 planarMoveDir = (camForward * inputDir.y + camRight * inputDir.x).normalized;

        // 4. 속도 적용
        var isSlope = IsOnSlope();
        if (isSlope)
        {
            // m_rb.linearVelocity = new Vector3(AdjustDirectionToSlope(planarMoveDir).x * m_robotData.MovementSpeed, m_rb.linearVelocity.y, AdjustDirectionToSlope(planarMoveDir).z * m_robotData.MovementSpeed);
            m_rb.linearVelocity = AdjustDirectionToSlope(planarMoveDir) * m_robotData.MovementSpeed;
        }
        else
        {
            m_rb.linearVelocity = new Vector3(planarMoveDir.x * m_robotData.MovementSpeed, m_rb.linearVelocity.y, planarMoveDir.z * m_robotData.MovementSpeed);

        }

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