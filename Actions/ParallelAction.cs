using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Modules.Actions
{
    public class ParallelAction : IAction
    {
        private readonly List<IAction> _actions = new ();
        private readonly ActionResultHandleType _resultHandleType;

        public ParallelAction(ActionResultHandleType resultHandleType = ActionResultHandleType.WhenAny)
        {
            _resultHandleType = resultHandleType;
        }

        public ParallelAction Add(IAction action)
        {
            _actions.Add(action);
            return this;
        }

        public async UniTask<bool> Do()
        {
            var tasks = new List<UniTask<bool>>();
            foreach (var action in _actions.ToList())
            {
                tasks.Add(action.Do());
            }

            var results = await UniTask.WhenAll(tasks);
            return _resultHandleType == ActionResultHandleType.WhenAll ? results.All(result => result) : results.Any(result => result);
        }
    }
}
