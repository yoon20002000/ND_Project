using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;

namespace Structs
{
    public struct EntityHPPair
    {
        public Entity Entity;
        public float HP;
    }

    [BurstCompile]
    public struct HealthAscendingComparer : IComparer<EntityHPPair>
    {
        public int Compare(EntityHPPair x, EntityHPPair y)
        {
            return x.HP.CompareTo(y.HP);
        }
    }
}