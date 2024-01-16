using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 1f;
        CanvasGroup canvasGroup;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public IEnumerator FadeOut()
        {
            while(canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime / fadeOutTime;
                yield return null;
            }
        }

        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
        }

        public IEnumerator FadeIn()
        {
            while(canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime / fadeInTime;
                yield return null;
            }
        }
    }
}
