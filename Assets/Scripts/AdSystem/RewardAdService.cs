using System;
using UnityEngine;
using AdjustSdk;
using AdSystem.Enums;

namespace AdSystem
{
    public class RewardAdService : MonoBehaviour
    {
        [SerializeField] private AdAnalyticsService _analyticsService;

#if UNITY_IOS
private string _adUnitId = "«...»";
#else // UNITY_ANDROID
        private string _adUnitId = "«...»";
#endif

        private int _retryAttempt;

        private Action _onRewarded;

        private void Start()
        {
            InitializeRewardedAds();
        }

        public void InitializeRewardedAds()
        {
            // Attach callback
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

            // Load the first rewarded ad
            LoadRewardedAd();
        }

        public void ShowRewarded(Action onRewarded)
        {
            if (MaxSdk.IsRewardedAdReady(_adUnitId))
            {
                _onRewarded = onRewarded;
                MaxSdk.ShowRewardedAd(_adUnitId);
            }
            else
            {
                LoadRewardedAd();
                _onRewarded = null;
            }
        }

        private void LoadRewardedAd()
        {
            MaxSdk.LoadRewardedAd(_adUnitId);
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdk.AdInfo adInfo)
        {
            // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

            // Reset retry attempt
            _retryAttempt = 0;
        }

        private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdk.ErrorInfo errorInfo)
        {
            // Rewarded ad failed to load
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

            _retryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _retryAttempt));

            Invoke("LoadRewardedAd", (float)retryDelay);
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdk.AdInfo adInfo) { }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdk.ErrorInfo errorInfo, MaxSdk.AdInfo adInfo)
        {
            // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
            LoadRewardedAd();
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdk.AdInfo adInfo) { }

        private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdk.AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            LoadRewardedAd();
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdk.AdInfo adInfo)
        {
            // The rewarded ad displayed and the user should receive the reward.
            _onRewarded?.Invoke();
        }

        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdk.AdInfo adInfo)
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