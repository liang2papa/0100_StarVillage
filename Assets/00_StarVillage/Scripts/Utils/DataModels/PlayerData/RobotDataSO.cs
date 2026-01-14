using UnityEngine;

/// <summary>
/// 로봇의 데이터를 들고 있음, 모든 데이터는 Excel에서 관리하고 ScriptableObject로 변환하여 사용
/// </summary>
[CreateAssetMenu(fileName = "RobotDataSO", menuName = "Data/RobotDataSO")]
public class RobotDataSO : ScriptableObject
{
    [Header("MovementSpeed")]
    public float MovementSpeed;
    public float RotationSpeed;

    [Header("Mining Stats")]
    public float MiningPower;

}
