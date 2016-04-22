public enum EActionState
{
    playing,
    stop,
    pause,
}

public interface IActionControllerPlayable
{
    EActionState actionState { get; }

    void CrossFade(string name, float blendtime = 0.3f, float normalizedTime = 0f);
    void Pause();
    void Resume();
    void Stop();
}