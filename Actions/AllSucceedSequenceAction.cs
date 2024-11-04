using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Modules.Actions
{
    public class AllSucceedSequenceAction : IAction
    {
        private readonly List<IAction> _actions = new ();

        public AllSucceedSequenceAction Add(IAction action)
        {
            _actions.Add(action);
            return this;
        }

        public async UniTask<bool> Do()
        {
            foreach (var action in _actions)
            {
                if (!await action.Do())
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
