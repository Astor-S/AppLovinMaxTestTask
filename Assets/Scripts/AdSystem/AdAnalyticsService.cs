using UnityEngine;
using Firebase.Analytics;
using AdSystem.Enums;

namespace AdSystem
{
    public class AdAnalyticsService : MonoBehaviour
    {
        public void CollectImpression(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Parameter[] AdParameters = new Parameter[] {
                new Parameter(AdParameterKeys.AdPlatform.ToString(), AdPlatformValue.AppLovinMaxSdk.ToString()), 
                new Parameter(AdParameterKeys.AdSource.ToString(), adInfo.NetworkName),
                new Parameter(AdParameterKeys.AdUnitName.ToString(), adInfo.AdUnitIdentifier),
                new Parameter(AdParameterKeys.AdFormat.ToString(), adInfo.Placement.ToString()),
                new Parameter(AdParameterKeys.Currency.ToString(), AdCurrencyValue.USD.ToString()),
                new Parameter(AdParameterKeys.Value.ToString(), adInfo.Revenue)
            };

            string logEventMessage = "ad_impression";

            FirebaseAnalytics.LogEvent(logEventMessage, AdParameters);
        }
    }
}