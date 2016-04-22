using UnityEngine;
using System.Collections.Generic;


public interface IGizmoListener
{
    void OnDrawGizmos();
    void OnDrawGizmosSelected();
}
