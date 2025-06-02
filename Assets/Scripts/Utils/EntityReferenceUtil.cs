using Unity.Collections;
using Unity.Entities;

namespace Utils
{
    public class EntityReferenceUtil
    {
        public static Entity GetNikkePrefabByName(in Entity refEntity, in FixedString32Bytes nikkePrefabName, EntityManager em)
        {
            DynamicBuffer<NikkePrefabBuffer> buffer = em.GetBuffer<NikkePrefabBuffer>(refEntity);

            foreach (var entry in buffer)
            {
                if (entry.NikkeName == nikkePrefabName)
                {
                    return entry.PrefabEntity;
                }
            }

            return Entity.Null;
        }
    }
}