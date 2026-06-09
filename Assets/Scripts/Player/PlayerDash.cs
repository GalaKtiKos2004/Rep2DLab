using System.Collections;
using UnityEngine;

/// <summary>
/// Рывок игрока: краткая неуязвимость и ускоренное перемещение.
/// </summary>
[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerFighter))]
public class PlayerDash : MonoBehaviour
{
    [SerializeField] private float _dashSpeed = 7f;
    [SerializeField] private float _dashDuration = 0.12f;
    [SerializeField] private float _cooldown = 1.5f;

    private PlayerMover _mover;
    private PlayerFighter _fighter;
    private bool _isDashing;
    private bool _isOnCooldown;

    private void Awake()
    {
        _mover = GetComponent<PlayerMover>();
        _fighter = GetComponent<PlayerFighter>();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _isDashing = false;
        _isOnCooldown = false;

        if (_mover != null)
        {
            _mover.ClearDashVelocity();
        }

        if (_fighter != null)
        {
            _fighter.SetInvulnerable(false);
        }
    }

    private void Update()
    {
        if (_isOnCooldown || _isDashing || _fighter.IsDead)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        Vector2 direction = GetDashDirection();

        if (direction == Vector2.zero)
        {
            float facing = transform.localScale.x >= 0 ? 1f : -1f;
            direction = new Vector2(facing, 0f);
        }

        _isDashing = true;
        _isOnCooldown = true;
        _fighter.SetInvulnerable(true);
        _mover.MovementLocked = true;

        float elapsed = 0f;

        while (elapsed < _dashDuration)
        {
            _mover.SetDashVelocity(direction * _dashSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _mover.ClearDashVelocity();
        _mover.MovementLocked = false;
        _fighter.SetInvulnerable(false);
        _isDashing = false;

        yield return new WaitForSeconds(_cooldown);
        _isOnCooldown = false;
    }

    private Vector2 GetDashDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(horizontal, vertical);

        return input.sqrMagnitude > 0.01f ? input.normalized : Vector2.zero;
    }
}
