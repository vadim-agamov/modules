using Cysharp.Threading.Tasks;

namespace Modules.Actions
{
    public abstract class LogicActionBase<TInput, TOutput>
    {
        public abstract Result<TOutput> Do(Result<TInput> input);
    }

    public abstract class VisualActionBase
    {
        public abstract UniTask Do();
    }

    public abstract class ActionBase<TInput, TOutput> : IAction<TInput, TOutput>
    {
        protected LogicActionBase<TInput,TOutput> LogicAction;
        protected VisualActionBase VisualAction;
        
        public virtual async UniTask<Result<TOutput>> Do(Result<TInput> input)
        {
            if(!input.Success)
            {
                return Result<TOutput>.Failed();
            }
            
            var result = LogicAction.Do(input);
            if (result.Success)
            {
                if (VisualAction != null)
                { 
                    await VisualAction.Do();
                }
            }
            
            return result;
        }
    }
}
