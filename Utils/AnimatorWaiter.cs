using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Modules.Utils
{
    public class AnimationEventHandler
    {
        private static int _idCounter;
        public int Id { get; }
        
        public int StateNameHash { get; }
        public float MaxDuration { get; set; }
        public UniTaskCompletionSource CompletionSource { get; }

        public AnimationEventHandler(string stateName, float maxDuration)
        {
            Id = _idCounter++;
            StateNameHash = Animator.StringToHash(stateName);
            MaxDuration = maxDuration;
            CompletionSource = new UniTaskCompletionSource();
            // Debug.Log($"[{nameof(AnimatorWaiter)}|{Id}] CREATED animation trigger {stateName} {StateNameHash}");
        }
    }

    [RequireComponent(typeof(Animator))]
    public class AnimatorWaiter : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        private Animator _animator;
        
        private int _lastStateHash;

        private readonly LinkedList<AnimationEventHandler> _handlers = new();
        
        public async UniTask WaitState(string state, float duration = float.MaxValue, CancellationToken token = default)
        {
            Cleanup();

            var animationEventHandler = new AnimationEventHandler(state, duration);
            _handlers.AddLast(animationEventHandler);
            // Debug.Log($"[{nameof(AnimatorWaiter)}|{animationEventHandler.Id}] BEGIN SetTriggerAsync {state}");
            await animationEventHandler.CompletionSource.Task;
            
            token.Register(() =>
            {
                Debug.LogWarning($"[{nameof(AnimatorWaiter)}|{animationEventHandler.Id}] CANCELLED awaiter for {state} {animationEventHandler.StateNameHash}");
                animationEventHandler.CompletionSource.TrySetCanceled();
                _handlers.Remove(animationEventHandler);
            });
            
            // Debug.Log($"[{nameof(AnimatorWaiter)}|{animationEventHandler.Id}] END SetTriggerAsync {state}");

            void Cleanup()
            {
                var handlersToRemove = _handlers.Where(handler => handler.StateNameHash ==  Animator.StringToHash(state)).ToList();
                handlersToRemove.ForEach(h =>
                {
                    Debug.LogWarning($"[{nameof(AnimatorWaiter)}|{h.Id}] CLEANED awaiter for {state} {h.StateNameHash}");
                    h.CompletionSource.TrySetResult();
                    _handlers.Remove(h);
                });
            }
        }

        private void Update()
        {
            Complete();
            CompleteExpired();
        }

        private void Complete()
        {
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            var currentStateHash = stateInfo.shortNameHash;
            if (currentStateHash != _lastStateHash)
            {
                _lastStateHash = currentStateHash;
                
               var handlersToRemove = _handlers.Where(handler => stateInfo.shortNameHash == handler.StateNameHash).ToList();

                handlersToRemove.ForEach(h =>
                {
                    // Debug.Log($"[{nameof(AnimatorWaiter)}|{h.Id}] COMPLETED animation trigger {h.StateNameHash}");
                    h.CompletionSource.TrySetResult();
                    _handlers.Remove(h);
                });
            }
        }

        private void CompleteExpired()
        {
            var handlersToRemove = new List<AnimationEventHandler>();
            foreach (var handler in _handlers)
            {
                handler.MaxDuration -= Time.deltaTime;
                if (handler.MaxDuration < 0)
                {
                    Debug.LogWarning($"[{nameof(AnimatorWaiter)}|{handler.Id}] EXPIRED animation trigger {handler.StateNameHash}");
                    handlersToRemove.Add(handler);
                }
            }

            handlersToRemove.ForEach(h =>
            {
                h.CompletionSource.TrySetResult();
                _handlers.Remove(h);
            });
        }

        private void OnValidate()
        {
            _animator = GetComponent<Animator>();
        }
    }
}
