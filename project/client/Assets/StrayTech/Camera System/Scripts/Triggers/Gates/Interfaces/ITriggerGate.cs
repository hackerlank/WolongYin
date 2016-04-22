using UnityEngine;
using System.Collections;

namespace StrayTech
{
    public interface ITriggerGate
    {
        void TriggerWasEntered(Collider other);
        bool IsTriggerBlocked();

        bool IsActive{ get; }
    }
}