using System;
using UnityEngine;

namespace AdSystem
{
    public class AdService : MonoBehaviour
    {
        [SerializeField] private InterstitialAdService _interstitialAdService;
        [SerializeField] private RewardAdService _rewardAdService;
        [SerializeField] private BannerAdService _bannerAdService;

        public void ShowInterstitialAd()
        {
            if (_interstitialAdService != null)
                _interstitialAdService.ShowInterstitial();
        }

        public void ShowRewardAd(Action onRewarded)
        {
            if (_rewardAdService != null)
                _rewardAdService.ShowRewarded(onRewarded);
        }

        public void ShowBanner()
        {
            if (_bannerAdService != null)
                _bannerAdService.ShowBanner();
        }

        public void HideBanner()
        {
            if (_bannerAdService != null)
                _bannerAdService.HideBanner();
        }
    }
}