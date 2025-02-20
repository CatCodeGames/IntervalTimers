using System;
using System.Collections;
using UnityEngine;

namespace CatCode.Timers
{
    public sealed class Effect : MonoBehaviour
    {
        private Vector3 _startPosition;
        private Coroutine _coroutine;
        public float _elapsedTime;
        private Action _callback;
        public float AwakeTime;
        public float Duration;
        public float StartOffset;
        public float Diff;
        public Vector3 MinPosition;
        public Vector3 MaxPosition;

        private void Awake()
        {
            AwakeTime = Time.time;
        }

        public void Play(float time = 0, Action callback = null)
        {
            StartOffset = time;
            _elapsedTime = time;
            _startPosition = transform.localPosition;
            Diff = AwakeTime - time;
            _callback = callback;
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            _coroutine = StartCoroutine(Test());
        }

        private IEnumerator Test()
        {
            while (_elapsedTime < Duration)
            {
                var relative = _elapsedTime / Duration;
                transform.localPosition = _startPosition + Vector3.Lerp(MinPosition, MaxPosition, relative);
                yield return null;
                _elapsedTime += Time.deltaTime;
            }
            transform.localPosition = _startPosition + MaxPosition;
            yield return null;
            _callback?.Invoke();
        }
    }
}