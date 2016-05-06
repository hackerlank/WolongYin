using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;


public class AttackDefinitionController : BaseGameMono
{
    private List<AttackDefinition> mAttackDefs = new List<AttackDefinition>();
 
    public override void Update(float deltaTime)
    {
        for (int i = 0; i < mAttackDefs.Count; ++i)
        {
            AttackDefinition adf = mAttackDefs[i];
            adf.OnUpdate(deltaTime);

            if (adf.OutOfData)
            {
                ObjectPool.Delete<AttackDefinition>(adf);
                mAttackDefs.RemoveAt(i);
                --i;
            }
        }
    }
}