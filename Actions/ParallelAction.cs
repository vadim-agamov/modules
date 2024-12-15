using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Modules.Actions
{
    public class ParallelAction<TInput> : IAction<TInput, Void>
    {
        private readonly List<IAction<TInput, Void>> _actions = new List<IAction<TInput, Void>>();

        public ParallelAction<TInput> Add<TOutput>(IAction<TInput, TOutput> action)
        {
            _actions.Add(new SuppressResultAction<TInput, TOutput>(action));
            return this;
        }
        
        public ParallelAction<TInput> Add<TOutput>(IAction<Void, TOutput> action)
        {
            _actions.Add(new SuppressInputAction<TInput, TOutput>(action));
            return this;
        }
        
        public async UniTask<Result<Void>> Do(Result<TInput> input = default)
        {
            await _actions.Select(action => action.Do(input));
            return Result<Void>.Succeed(default);
        }
    }
}