
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewEnemyStats",
    menuName = "CarrotRush/Enemy Stats"
)]
public class EnemyStats : ScriptableObject
{
    [Header("General")]
    public float moveSpeed;
    public int maxHealth;
    public int damage;

    [Header("Patroller Movement")]
    public float patrolSpeed;
    public float chaseSpeed;
    public float stoppingDistance;
    public float chaseTargetMultiplier;

    [Header("Player Detection")]
    public float detectionRange;
    public float losePlayerRange;

    [Header("Attack")]
    public float attackRange;
    public float attackCooldown;
    public float warningDuration;
    public float attackHeightTolerance;

    [Header("Special Behavior")]
    public float windUpTime;


    [Header("Charger Settings")]
    public float chargeSpeed;
    public float chargeDuration;
    public float recoveryDuration;
    public float hitRangeX;
    public float hitRangeY;


    [Header("Shooter Settings")]
    public float fieldOfView;
    public float facingAngle;
    
    [Header("Lunge Attack")]
    public float lungeDistance;
    public float lungeDuration;

}
