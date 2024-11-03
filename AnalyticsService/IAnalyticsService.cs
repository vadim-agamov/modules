using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.Initializator;

namespace Modules.AnalyticsService
{
    public interface IAnalytic
    {
        UniTask Initialize(CancellationToken token);
        void Start();
        void Stop();
        void TrackEvent(string eventName, Dictionary<string, object> parameters);
    }

    public interface IAnalyticsService: IInitializable
    {
        void Start();
        void Stop();
        void TrackEvent(string eventName, Dictionary<string, object> parameters = null);
    }
}