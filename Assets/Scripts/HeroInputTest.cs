using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody2D))]
public class HeroInputTest : MonoBehaviour
{
[SerializeField] float speed = 6f;
[SerializeField] float jumpSpeed = 12f;
Rigidbody2D rb;
InputAction move, jump, attack;
void Awake()
{
rb = GetComponent<Rigidbody2D>();
move = InputSystem.actions.FindAction("Move");
jump = InputSystem.actions.FindAction("Jump");
attack = InputSystem.actions.FindAction("Attack");
}
void Update()
{
if (PauseMenu.IsPaused) return;
Vector2 input = move.ReadValue<Vector2>();
rb.linearVelocity = new Vector2(input.x * speed,
rb.linearVelocity.y);
if (jump.WasPressedThisFrame())
rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
if (attack.WasPressedThisFrame())
Debug.Log("FIRE");
}
}