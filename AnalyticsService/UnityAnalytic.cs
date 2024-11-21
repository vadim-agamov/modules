using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Services.Core;

namespace Modules.AnalyticsService
{
    public class UnityAnalytic : IAnalytic
    {
        async UniTask IAnalytic.Initialize(CancellationToken token)
        {
            await UnityServices.InitializeAsync();
        }

        void IAnalytic.Start() => Unity.Services.Analytics.AnalyticsService.Instance.StartDataCollection();

        void IAnalytic.Stop() => Unity.Services.Analytics.AnalyticsService.Instance.StopDataCollection();

        void IAnalytic.TrackEvent(string eventName, Dictionary<string, object> parameters)
        {
            var customEvent = new Unity.Services.Analytics.CustomEvent(eventName);
            foreach (var (k,v) in parameters)
            {
                customEvent.Add(k, v);
            }
            Unity.Services.Analytics.AnalyticsService.Instance.RecordEvent(customEvent);
        }
    }
}