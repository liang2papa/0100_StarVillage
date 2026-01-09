using UnityEngine;

[CreateAssetMenu(fileName = "RobotDataSO", menuName = "Data/RobotDataSO")]
public class RobotDataSO : ScriptableObject
{
    [Header("MovementSpeed")]
    public float MovementSpeed;
    public float RotationSpeed;

    [Header("Mining Stats")]
    public float MiningPower;

}
