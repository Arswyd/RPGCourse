using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.AI;
using Unity.VisualScripting;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float raycastRadius = 1f;
        Mover mover;
        Health health;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        void Awake()
        {
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
        }
        void Update()
        {
            if(InteractWithUI()) { return; }
            if(health.IsDead()) 
            { 
                SetCursor(CursorType.None);
                return; 
            }

            if(InteractWithComponent()) { return; };
            if(InteractWithMovement()) { return; };

            SetCursor(CursorType.None);
        }

        bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastHitsSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if(raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastHitsSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius).OrderBy(x => x.distance).ToArray();
            // float[] distances = new float[hits.Length];
            // for (int i = 0; i < hits.Length; i++)
            // {
            //    distances[i] = hits[i].distance;
            // }
            // Array.Sort(distances, hits);
            return hits;
        }

        bool InteractWithUI()
        {
            if(EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true; 
            }
            return false;
        }

        void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        CursorMapping GetCursorMapping(CursorType type)
        {
            foreach(CursorMapping cursorMapping in cursorMappings)
            {
                if(cursorMapping.type == type)
                {
                    return cursorMapping;
                }
            }

            return cursorMappings[0];
        }        

        bool InteractWithMovement()
        {
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if(!mover.CanMoveTo(target)) { return false; }

                if (Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }

            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if(!hasHit) { return false; }

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if(!hasCastToNavMesh) { return false; }

            target = navMeshHit.position;

            return true;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}