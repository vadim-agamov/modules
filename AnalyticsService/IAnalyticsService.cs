using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using Modules.ServiceLocator.Initializator;

namespace Modules.AnalyticsService
{
    public interface IAnalytic
    {
        UniTask Initialize(CancellationToken token);
        void Start();
        void Stop();
        void TrackEvent(string eventName, Dictionary<string, object> parameters);
    }

    public interface IAnalyticsService: IInitializableService
    {
        void Start();
        void Stop();
        void TrackEvent(string eventName, Dictionary<string, object> parameters = null);
    }
}