using System;
using System.Collections;
using System.Collections.Generic;

public class UnityCoroutine : IEnumerator
{
    /// <summary>
    /// All processing is done.
    /// </summary>
    public bool isDone { get; protected set; }

    /// <summary>
    /// Run all remaining routines at this time.
    /// </summary>
    public void DoSync()
    {
        while (MoveNext()) ;
    }

    /// <summary>
    /// Run the routines one at a time(each routine at each update).
    /// </summary>
    protected Queue<Action> routines = new Queue<Action>();

    /// <summary>
    /// Run the routines.
    /// </summary>
    /// <returns>Is there remaining work.</returns>
    public bool MoveNext()
    {
        if (!isDone && routines.Count > 0)
        {
            Action routine = routines.Dequeue();
            routine();
        }

        return !isDone;
    }

    /// <summary>
    /// Clear queued routines.
    /// </summary>
    public void Reset()
    {
        routines.Clear();
    }

    /// <summary>
    /// Remaining routines count.
    /// </summary>
    public object Current
    {
        get
        {
            return routines.Count;
        }
    }
}