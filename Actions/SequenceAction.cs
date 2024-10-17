using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Modules.Actions
{
    public class SequenceAction : IAction
    {
        private readonly List<IAction> _actions = new ();
        private readonly ActionResultHandleType _resultHandleType;

        public SequenceAction(ActionResultHandleType resultHandleType = ActionResultHandleType.WhenAny)
        {
            _resultHandleType = resultHandleType;
        }

        public SequenceAction Add(IAction action)
        {
            _actions.Add(action);
            return this;
        }

        public async UniTask<bool> Do()
        {
            var results = new List<bool>();
            foreach (var action in _actions)
            {
                results.Add(await action.Do());
            }

            return _resultHandleType == ActionResultHandleType.WhenAll ? results.All(result => result) : results.Any(result => result);
        }
    }
}
