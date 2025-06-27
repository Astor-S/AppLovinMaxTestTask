using UnityEngine;
using AdjustSdk;
using AdSystem.Enums;

namespace AdSystem
{
    public class BannerAdService : MonoBehaviour
    {
        [SerializeField] private AdAnalyticsService _analyticsService;
        [SerializeField] private string _bannerBackgroundColorString = "#FFFFFF";

        private Color _bannerBackgroundColor;

#if UNITY_IOS
private string _bannerAdUnitId = "«...»"; // Retrieve the ID from your account
#else // UNITY_ANDROID
        private string _bannerAdUnitId = "«...»"; // Retrieve the ID from your account
#endif

        private bool _bannerIsVisible = false;

        private void Start()
        {
            if (ColorUtility.TryParseHtmlString(_bannerBackgroundColorString, out _bannerBackgroundColor) == false)
                _bannerBackgroundColor = Color.white;

            InitializeBannerAds();
        }

        public void InitializeBannerAds()
        {
            // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
            // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
            var adViewConfiguration = new MaxSdk.AdViewConfiguration(MaxSdk.AdViewPosition.BottomCenter);
            MaxSdk.CreateBanner(_bannerAdUnitId, adViewConfiguration);

            // Set background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(_bannerAdUnitId, _bannerBackgroundColor);

            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;

            HideBanner();
        }

        public void ShowBanner()
        {
            if (_bannerIsVisible == false)
            {
                MaxSdk.ShowBanner(_bannerAdUnitId);
                _bannerIsVisible = true;
            }
        }

        public void HideBanner()
        {
            if (_bannerIsVisible)
            {
                MaxSdk.HideBanner(_bannerAdUnitId);
                _bannerIsVisible = false;
            }
        }

        private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdk.AdInfo adInfo)
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