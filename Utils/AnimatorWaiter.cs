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
            Debug.Assert(_handlers.All(h => h.StateNameHash != Animator.StringToHash(state)), $"[{nameof(AnimatorWaiter)}] There are handlers for {state}");

            var animationEventHandler = new AnimationEventHandler(state, duration);
            _handlers.AddLast(animationEventHandler);
            // Debug.Log($"[{nameof(AnimatorWaiter)}|{animationEventHandler.Id}] BEGIN SetTriggerAsync {state}");
            
            await animationEventHandler.CompletionSource.Task.AttachExternalCancellation(token);
            
            await UniTask.Yield();
            
            // Debug.Log($"[{nameof(AnimatorWaiter)}|{animationEventHandler.Id}] END SetTriggerAsync {state}");
        }

        private void Update()
        {
            foreach (var handler in _handlers)
            {
                handler.MaxDuration -= Time.deltaTime;
            }

            Complete();
            CompleteExpired();
        }

        private void Complete()
        {
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            var currentStateHash = stateInfo.shortNameHash;
            if (currentStateHash == _lastStateHash)
            {
                return;
            }
            
            _lastStateHash = currentStateHash;
            
            while (_handlers.Any(h => h.StateNameHash == currentStateHash))
            {
                var h = _handlers.First(h => h.StateNameHash == currentStateHash);
                _handlers.Remove(h);
                h.CompletionSource.TrySetResult();
                // Debug.Log($"[{nameof(AnimatorWaiter)}|{h.Id}] COMPLETED animation trigger {h.StateNameHash}");
            }
                
            Debug.Assert(_handlers.All(h => h.StateNameHash != currentStateHash), $"[{nameof(AnimatorWaiter)}] There are still handlers for {currentStateHash}");
        }

        private void CompleteExpired()
        {
            while (_handlers.Any(h => h.MaxDuration < 0))
            {
                var h = _handlers.First(h =>  h.MaxDuration < 0);
                _handlers.Remove(h);
                h.CompletionSource.TrySetResult();
                // Debug.Log($"[{nameof(AnimatorWaiter)}|{h.Id}] COMPLETED animation trigger {h.StateNameHash}");
            }
            
            Debug.Assert(_handlers.All(h => h.MaxDuration > 0), $"[{nameof(AnimatorWaiter)}] There are still handlers with expired duration");
        }

        private void OnValidate()
        {
            _animator = GetComponent<Animator>();
        }
    }
}
