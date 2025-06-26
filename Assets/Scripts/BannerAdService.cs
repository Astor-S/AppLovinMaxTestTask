using UnityEngine;

public class BannerAdService : MonoBehaviour
{
#if UNITY_IOS
string bannerAdUnitId = "«...»"; // Retrieve the ID from your account
#else // UNITY_ANDROID
    private string _bannerAdUnitId = "«...»"; // Retrieve the ID from your account
#endif

    [SerializeField]
    private string _bannerBackgroundColorString = "#FFFFFF";
    private Color _bannerBackgroundColor;

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

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdk.AdInfo adInfo) { }

    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdk.ErrorInfo errorInfo) { }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdk.AdInfo adInfo) { }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdk.AdInfo adInfo)
    {
        var adRevenue = new AdjustAdRevenue("applovin_max_sdk");
        adRevenue.SetRevenue(adInfo.Revenue, "USD");
        adRevenue.AdRevenueNetwork = adInfo.NetworkName;
        adRevenue.AdRevenueUnit = adInfo.AdUnitIdentifier;
        adRevenue.AdRevenuePlacement = adInfo.Placement;

        Adjust.TrackAdRevenue(adRevenue);
    }

    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdk.AdInfo adInfo) { }

    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdk.AdInfo adInfo) { }
}