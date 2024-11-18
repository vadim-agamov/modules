using System;
using Cysharp.Threading.Tasks;
using Modules.Utils;
using TMPro;
using UnityEngine;

namespace Modules.FlyItemsService
{
    public class FlyUpItemView : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;
        
        [SerializeField]
        private AnimatorWaiter _animatorWaiter;
        
        [SerializeField]
        private TMP_Text _text;
        
        public UniTask Play(int value)
        {
            _text.text = $"+{value}";
            return _animatorWaiter.WaitState("Idle", 2);
        }

        public void ResetView()
        {
            _animator.Rebind();
            _animator.Update(0);
        }
    }
}