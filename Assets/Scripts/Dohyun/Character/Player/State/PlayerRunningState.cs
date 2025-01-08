using Unity.IO.LowLevel.Unsafe;

public class PlayerRunningState : IPlayerState
{
    public void Enter(Player player)
    {
        Debug.Log("Player entered Running state.");
        player.NotifyObservers("Running");
    }

    public void Update(Player player)
    {
        if (player.MovementInput == Vector2.zero)
        {
            player.ChangeState(new IdleState());
        }
    }

    public void Exit(Player player)
    {
        Debug.Log("Player exited Running state.");
    }
}