using UnityEngine;
using Firebase.Analytics;

namespace AdSystem
{
    public class AdAnalyticsService : MonoBehaviour
    {
        public void CollectImpression(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Parameter[] AdParameters = new Parameter[] {
            new Parameter("ad_platform", "applovin_max_sdk"),
            new Parameter("ad_source", adInfo.NetworkName),
            new Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
            new Parameter("ad_format", adInfo.Placement.ToString()),
            new Parameter("currency", "USD"),
            new Parameter("value", adInfo.Revenue)
        };

            FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
        }
    }
}