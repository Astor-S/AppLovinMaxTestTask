using System;
using UnityEngine;
using AdjustSdk;
using AdSystem.Enums;

namespace AdSystem
{
    public class InterstitialAdService : MonoBehaviour
    {
        [SerializeField] private AdAnalyticsService _analyticsService;

#if UNITY_IOS
private string _adUnitId = "«...»";
#else // UNITY_ANDROID
        private string _adUnitId = "«...»";
#endif

        private int _retryAttempt;

        private void Start()
        {
            InitializeInterstitialAds();
        }

        public void InitializeInterstitialAds()
        {
            // Attach callback
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialAdRevenuePaidEvent;

            // Load the first interstitial
            LoadInterstitial();
        }

        public void ShowInterstitial()
        {
            if (MaxSdk.IsInterstitialReady(_adUnitId))
                MaxSdk.ShowInterstitial(_adUnitId);
        }

        private void LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(_adUnitId);
        }

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdk.AdInfo adInfo)
        {
            // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

            // Reset retry attempt
            _retryAttempt = 0;
        }

        private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdk.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

            int baseForExponentiation = 2;
            int maxRetryExponent = 6;
            string loadMessage = "LoadInterstitial";

            _retryAttempt++;
            double retryDelay = Math.Pow(baseForExponentiation, Math.Min(maxRetryExponent, _retryAttempt));

            Invoke(loadMessage, (float)retryDelay);
        }

        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdk.ErrorInfo errorInfo, MaxSdk.AdInfo adInfo)
        {
            // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
            LoadInterstitial();
        }

        private void OnInterstitialHiddenEvent(string adUnitId, MaxSdk.AdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad.
            LoadInterstitial();
        }

        private void OnInterstitialAdRevenuePaidEvent(string adUnitId, MaxSdk.AdInfo adInfo)
        {
            _analyticsService.CollectImpression(adUnitId, adInfo);

            var adRevenue = new AdjustAdRevenue(AdPlatformValue.AppLovinMaxSdk.ToString());
            adRevenue.SetRevenue(adInfo.Revenue, AdCurrencyValue.USD.ToString());
            adRevenue.AdRevenueNetwork = adInfo.NetworkName;
            adRevenue.AdRevenueUnit = adInfo.AdUnitIdentifier;
            adRevenue.AdRevenuePlacement = adInfo.Placement;

            Adjust.TrackAdRevenue(adRevenue);
        }
    }
}