using Cysharp.Threading.Tasks;

namespace Modules.Actions
{
    public abstract class LogicActionBase
    {
        public abstract bool Do(); // return true if board was updated
    }

    public abstract class VisualActionBase
    {
        public abstract UniTask Do();
    }

    public abstract class ActionBase : IAction
    {
        protected LogicActionBase LogicAction;
        protected VisualActionBase VisualAction;

        public virtual async UniTask<bool> Do()
        {
            var success = LogicAction.Do();
            if (success)
            {
                if (VisualAction != null)
                {
                    await VisualAction.Do();
                }
            }

            return success;
        }
    }
}
