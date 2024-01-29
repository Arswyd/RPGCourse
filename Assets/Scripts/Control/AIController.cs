using System;
using GameDevTV.Utils;
using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float alertDuration = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 3f;
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        [SerializeField] float aggroCooldownTime = 5f;
        [SerializeField] float alertDistance = 5f;
        Fighter fighter;
        Mover mover;
        GameObject player;
        Health health;
        LazyValue<Vector3> guardPosition;
        ActionScheduler actionScheduler;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceWaypointReached = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;
        int currentWaypointIndex = 0;

        void Awake()
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            actionScheduler = GetComponent<ActionScheduler>();
            player = GameObject.FindWithTag("Player");

            guardPosition = new LazyValue<Vector3>(SetGuardPosition);
        }

        Vector3 SetGuardPosition()
        {
            return transform.position;
        }

        void Start()
        {
            guardPosition.ForceInit();
        }

        void Update()
        {
            if (health.IsDead()) { return; }

            if (IsAggrevated() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < alertDuration)
            {
                AlertBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        public void Aggrevate()
        {
            timeSinceAggrevated = 0;
        }

        void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceWaypointReached += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer <= chaseDistance || timeSinceAggrevated < aggroCooldownTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;

            if(patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceWaypointReached = 0;
                    CycleWaypoint();             
                }
                nextPosition = GetCurrentWaypoint();
            }

            if(timeSinceWaypointReached > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint <= waypointTolerance;
        }

        private void AlertBehaviour()
        {
            actionScheduler.CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);

            AggrevateNearbyEnemies();
        }

        void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, alertDistance, Vector3.up, 0);
            foreach(RaycastHit hit in hits)
            {
                AIController ai = hit.collider.GetComponent<AIController>();
                if(ai == null) { continue; }

                ai.Aggrevate();
            }
        }

        // Called by Unity
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
