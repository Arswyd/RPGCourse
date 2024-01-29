using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas rootCanvas = null;

        void Update()
        {
            float fraction = healthComponent.GetFraction();
            if(Mathf.Approximately(fraction, 0) || Mathf.Approximately(fraction, 1))
            {
                rootCanvas.enabled = false;
                return;
            }
            rootCanvas.enabled = true;
            foreground.localScale = new Vector3(fraction, 1, 1);
        }
    }
}
