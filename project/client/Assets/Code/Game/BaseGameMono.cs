using UnityEngine;
using System.Collections.Generic;


public abstract class BaseGameMono
{
    public Transform transform = null;
    public GameObject gameObject = null;
    public bool enable = true;
    public string name = string.Empty;
    public bool started = false;

    public virtual void Awake(){ }

    public virtual void Start(){}

    public virtual void Update(float deltaTime){}

    public virtual void LateUpdate(float deltaTime){}

    public virtual void FixedUpdate(){}

    public virtual void OnDestroy(){}


}