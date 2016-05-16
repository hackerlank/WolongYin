using UnityEngine;
using System.Collections.Generic;

public class GameStartStage : IStage
{
    public void OnEnter()
    {
        ClientRoot.instance.Client.InitClientFiles();
    }

    public void OnExit()
    {
    }

    public void OnUpdate(float deltaTime)
    {
    }

    public void OnGUI()
    {
    }


    public void Pause()
    {
    }

    public void Resume()
    {
    }

    public void Stop()
    {
    }
}