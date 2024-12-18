using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.DiContainer;
using Modules.PlatformService;

namespace Modules.AnalyticsService
{
    public class PlatformAnalytic : IAnalytic
    {
        private IPlatformService PlatformService { get; set; } 
        
        UniTask IAnalytic.Initialize(CancellationToken token)
        {
            PlatformService = Container.Resolve<IPlatformService>();
            return UniTask.CompletedTask;
        }

        void IAnalytic.Start() { }

        void IAnalytic.Stop() { }

        void IAnalytic.TrackEvent(string eventName, Dictionary<string, object> parameters) => PlatformService.LogEvent(eventName, parameters);
    }
}