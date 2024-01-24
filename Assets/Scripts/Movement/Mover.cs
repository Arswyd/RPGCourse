using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxSpeed = 5.66f;
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
