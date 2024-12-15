using System;
using Cysharp.Threading.Tasks;

namespace Modules.Actions
{
    public class SequenceActionInternal<TInput, TOutput> : IAction<TInput, TOutput>
    {
        private readonly Func<Result<TInput>, UniTask<Result<TOutput>>> _action;

        public SequenceActionInternal(Func<Result<TInput>, UniTask<Result<TOutput>>> action) => 
            _action = action;
        
        public SequenceActionInternal<TInput, TNextOutput> Append<TNextOutput>(IAction<TOutput, TNextOutput> nextAction)
        {
            return new SequenceActionInternal<TInput, TNextOutput>(CombineDo);

            // capture _action and nextAction
            async UniTask<Result<TNextOutput>> CombineDo(Result<TInput> input)
            {
                var intermediateOutput = await _action(input);
                if(!intermediateOutput.Success)
                {
                    return Result<TNextOutput>.Failed();
                }
                return await nextAction.Do(intermediateOutput);
            }
        }

        public UniTask<Result<TOutput>> Do(Result<TInput> input) => _action(input);
    }

    public static class SequenceAction
    {
        public static SequenceActionInternal<TInput, TOutput> Start<TInput, TOutput>(IAction<TInput, TOutput> action) => 
            new(action.Do);
    }
}