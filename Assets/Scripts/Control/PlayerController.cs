using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using System;
using RPG.Combat;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Mover movement;
        Fighter fighter;

        void Awake()
        {
            movement = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
        }
        void Update()
        {
            if(InteractWithCombat()) { return; };
            if(InteractWithMovement()) { return; };
        }

        bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if(!fighter.CanAttack(target)) { continue; }
                
                if(Input.GetMouseButtonDown(0))
                {
                    fighter.Attack(target);
                }
                return true;
            }
            return false;
        }

        bool InteractWithMovement()
        {
            return DoRaycastAtCursor();
        }

        bool DoRaycastAtCursor()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    movement.StartMoveAction(hit.point);
                }
                return true;
            }

            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}