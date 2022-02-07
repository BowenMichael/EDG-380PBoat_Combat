using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.BowenIvanov.BoatCombat
{
    public class SpawnPoint : MonoBehaviour
    {
        private void Awake()
        {
            PlayerSpawnManager.AddSpawnPoint(transform);
        }

        private void OnDestroy()
        {
            //PlayerSpawnManager.RemoveSpawnPoint(transform);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 1f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
        }
    }
}
