using System.Collections.Generic;
using UnityEngine;


public interface ITriggerListener
{
    void OnTriggerEnter(Collider other);
    void OnTriggerExit(Collider other);
}
