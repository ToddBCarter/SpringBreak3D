using UnityEngine;
[CreateAssetMenu(fileName = "PlayerStatsSO", menuName = "Scripts/Player/PlayerStatsSO")]
public class PlayerStatsSO : ScriptableObject
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravityMultiplier = 1f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 10f;

    public float WalkSpeed => walkSpeed;
    public float SprintSpeed => sprintSpeed;
    public float JumpForce => jumpForce;
    public float GravityMultiplier => gravityMultiplier;
    public float Acceleration => acceleration;
    public float Deceleration => deceleration;
}
