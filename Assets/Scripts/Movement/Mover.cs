using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxSpeed = 5.66f;
        [SerializeField] float maxNavPathLenght = 40f;
        NavMeshAgent agent;
        Animator animator;
        ActionScheduler actionScheduler;
        Health health;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            agent.enabled = !health.IsDead();
            UpdateAnimator();
        }

        void UpdateAnimator()
        {
            Vector3 velocity =  agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity); // transform world to local
            float speed = localVelocity.z;
            animator.SetFloat("movementSpeed", speed);
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            actionScheduler.StartAction(this);
            MoveToDestination(destination, speedFraction);
        }

        public void MoveToDestination(Vector3 destination, float speedFraction)
        {
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            agent.destination = destination;
            agent.isStopped = false;
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if(!hasPath) { return false; }
            if(path.status != NavMeshPathStatus.PathComplete) { return false; }
            if(GetPathLenght(path) > maxNavPathLenght) { return false; }

            return true;
        }

        float GetPathLenght(NavMeshPath path)
        {
            float total = 0;
            if(path.corners.Length < 2) {return total;}
            
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i+1]);
            }
            return total;
        }

        public void Cancel()
        {
            agent.isStopped = true;
        }

        // [System.Serializable]
        // struct MoverSaveDate
        // {
        //     public SerializableVector3 position;
        //     public SerializableVector3 rotation;
        // }

        public JToken CaptureAsJToken()
        {
            return transform.position.ToToken();
        }

        // public JToken CaptureAsJToken()
        // {
        //     MoverSaveDate data = new MoverSaveDate
        //     {
        //         position = new SerializableVector3(transform.position),
        //         rotation = new SerializableVector3(transform.eulerAngles)
        //     };

        //     return JToken.FromObject(data);
        // }

        public void RestoreFromJToken(JToken state)
        {
            //MoverSaveDate data = (MoverSaveDate)state;
            GetComponent<NavMeshAgent>().enabled = false;
            //transform.position = data.position.ToVector();
            //transform.eulerAngles = data.rotation.ToVector();
            transform.position = state.ToObject<Vector3>();
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}
