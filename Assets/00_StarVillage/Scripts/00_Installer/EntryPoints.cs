using UnityEngine;

/// <summary>
/// 역할
/// 1. Awake에서 객체 생성
/// 2. 의존성 주입
/// 3. 초기화
/// 
/// [Lv 0] 게임 진입점 (Installer)
/// ===========================
/// EntryPoints
/// ┠ Director - Factory
///     ┠ Coodinator - Service
///         ┠ Module
///             ┠ Viewer
/// </summary>
public class EntryPoints : MonoBehaviour
{
    [Header("SceneRefs")]
    [SerializeField] private GameDirector m_director;
    [SerializeField] private RobotCoordinator m_robot;
    [SerializeField] private UICoordinator m_uiCoordinator;
    [SerializeField] private Camera m_camera;

    [Header("DataRefs")]
    [SerializeField] private RobotDataSO m_robotData;

    /// <summary>
    /// 클래스 생성 및 연결 (Wiring)
    /// </summary>
    private void Awake()
    {
        var inputService = new InputService(m_director);
        // var dataService = new DataService(); // 아직 세부 내용이 없음

        m_robot.InitClass(inputService, m_robotData, m_camera.transform, m_uiCoordinator);
    }
    private void Start()
    {
        m_director.InitClass(EGameState.Playing); // 디렉터에게 지시
    }

}
