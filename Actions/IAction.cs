using Cysharp.Threading.Tasks;

namespace Modules.Actions
{
    public sealed class SuppressResultAction<TInput, TOutput> : IAction<TInput, Void>
    {
        private readonly IAction<TInput, TOutput> _action;

        public SuppressResultAction(IAction<TInput, TOutput> action) => _action = action;

        public async UniTask<Result<Void>> Do(Result<TInput> input)
        {
            await _action.Do(input);
            return Result<Void>.Succeed(default);
        }
    }
    
    public sealed class SuppressInputAction<TInput, TOutput> : IAction<TInput, Void>
    {
        private readonly IAction<Void, TOutput> _action;
        public SuppressInputAction(IAction<Void, TOutput> action) => _action = action;
        public async UniTask<Result<Void>> Do(Result<TInput> _)
        {
            await _action.Do(Result<Void>.Succeed(default));
            return Result<Void>.Succeed(default);
        }
    }

    public interface IAction<TInput, TOutput> 
    {
        UniTask<Result<TOutput>> Do(Result<TInput> input);
    }
}
