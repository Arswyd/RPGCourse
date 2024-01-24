using UnityEngine;

namespace RPG.Core
{
    public class DestroyEffect : MonoBehaviour
    {
        void Update()
        {
            if(!GetComponent<ParticleSystem>().IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}
