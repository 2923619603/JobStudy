using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public static class MonoUtils
    {
        public static Coroutine NextFrame(this MonoBehaviour mono, Action action)
        {
            return mono.StartCoroutine(NextFrameCoroutine(action));
        }

        public static Coroutine Delay(this MonoBehaviour mono, bool isRealtime, float delayTime, Action action)
        {
            return mono.StartCoroutine(DelayCoroutine(isRealtime, delayTime, action));
        }


        private static IEnumerator NextFrameCoroutine(Action action)
        {
            yield return null;
            action?.Invoke();
        }

        private static IEnumerator DelayCoroutine(bool isRealtime, float delay, Action action)
        {
            if (isRealtime)
                yield return new WaitForSecondsRealtime(delay);
            else
            {
                yield return new WaitForSeconds(delay);
            }

            action?.Invoke();
        }
    }
}