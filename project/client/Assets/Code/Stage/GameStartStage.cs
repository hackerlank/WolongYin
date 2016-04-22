using UnityEngine;
using System.Collections.Generic;

public class GameStartStage : IStage
{
    public bool  IsDone()
    {
        return true;
    }

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
}