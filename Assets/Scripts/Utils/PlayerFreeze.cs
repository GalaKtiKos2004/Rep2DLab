using UnityEngine;

/// <summary>
/// Блокирует и разблокирует управление игроком во время диалогов.
/// </summary>
public static class PlayerFreeze
{
    public static void SetFrozen(bool frozen)
    {
        PlayerMover mover = Object.FindObjectOfType<PlayerMover>();
        PlayerFighter fighter = Object.FindObjectOfType<PlayerFighter>();
        PlayerDash dash = Object.FindObjectOfType<PlayerDash>();

        if (mover != null)
        {
            mover.MovementLocked = frozen;
            mover.ClearDashVelocity();
        }

        if (fighter != null)
        {
            fighter.enabled = !frozen;
        }

        if (dash != null)
        {
            dash.enabled = !frozen;
        }
    }
}
