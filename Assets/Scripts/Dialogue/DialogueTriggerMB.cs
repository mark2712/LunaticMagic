using System.Collections.Generic;
using Entities_1;
using UnityEngine;

namespace Dialog
{
    public class DialogueTriggerMB : MonoBehaviour
    {
        public BaseEntity Player;
        public readonly List<BaseEntity> NearbyNPCs = new(); // Список всех NPC, находящихся в триггере

        void OnTriggerEnter(Collider other)
        {
            if (Player == null) return;
            if (!other.TryGetComponent<BaseEntity>(out var npc)) return;
            if (!NearbyNPCs.Contains(npc)) NearbyNPCs.Add(npc);
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<BaseEntity>(out var npc)) return;
            NearbyNPCs.Remove(npc);
        }

        // Возвращает ближайшего NPC (или null, если никого)
        public BaseEntity GetClosestNPC()
        {
            if (NearbyNPCs.Count == 0 || Player == null) return null;

            BaseEntity closest = null;
            float minDist = float.MaxValue;
            Vector3 playerPos = Player.transform.position;

            foreach (var npc in NearbyNPCs)
            {
                if (npc == null) continue; // вдруг был удалён
                float dist = Vector3.SqrMagnitude(npc.transform.position - playerPos);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = npc;
                }
            }

            return closest;
        }
    }
}