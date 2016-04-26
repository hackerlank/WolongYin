public enum EActionState
{
    playing,
    stop,
    pause,
}

public interface IActionControllerPlayable
{
    EActionState actionState { get; }

    void Pause();
    void Resume();
    void Stop();
}
