using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 1f;
        CanvasGroup canvasGroup;
        Coroutine activeFade = null;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public Coroutine FadeOut()
        {
            return Fade(1f, fadeOutTime);
        }

        public Coroutine FadeIn()
        {
            return Fade(0, fadeInTime);
        }

        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
        }

        Coroutine Fade(float target, float time)
        {
            if (activeFade != null)
            {
                StopCoroutine(activeFade);
            }
            activeFade = StartCoroutine(FadeRoutine(target, time));
            return activeFade;
        }

        IEnumerator FadeRoutine(float target, float time)
        {
            while(!Mathf.Approximately(canvasGroup.alpha, target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }
    }
}
