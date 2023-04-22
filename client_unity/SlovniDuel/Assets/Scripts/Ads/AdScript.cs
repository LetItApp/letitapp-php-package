using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdScript : MonoBehaviour
{
    private BannerView bannerView;
    private InterstitialAd interstitial;
	private bool showingAds = true;

    public void HideAds()
    {
        if(bannerView != null)
            bannerView.Hide();
            
		showingAds = false;
    }

    public void Start()
    {
		#if UNITY_ANDROID
		       string appId = "ca-app-pub-6336883811609351~9988543265"; 
		#elif UNITY_IPHONE
		       string appId = "unexpected_platform";
		#else
		       string appId = "unexpected_platform";
		#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);

        RequestBanner();
        PreloadFullscreenBanner();
    }
    
    public void PreloadFullscreenBanner()
    {
    	#if UNITY_EDITOR
				string adUnitId = "unused";
        #elif UNITY_ANDROID
		        string adUnitId = "ca-app-pub-6336883811609351/4778834217";
        #elif UNITY_IPHONE
		        string adUnitId = "unexpected_platform";
        #else
		        string adUnitId = "unexpected_platform";
        #endif

        interstitial = new InterstitialAd(adUnitId);
        AdRequest request = new AdRequest.Builder().AddTestDevice("AF5A92326D4C3E240F138B6D85DBEA33").Build();
        interstitial.LoadAd(request);
    }
    
    public void RequestFullscreenBanner()
    {
		if(!showingAds)
			return;
			
		if (interstitial != null && interstitial.IsLoaded()) {
			interstitial.Show();
		}
    }
    
    private void RequestBanner()
    {
		#if UNITY_ANDROID
				string adUnitId = "ca-app-pub-6336883811609351/8292318216";
		#elif UNITY_IPHONE
				string adUnitId = "unexpected_platform";
		#else
				string adUnitId = "unexpected_platform";
		#endif

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
        //bannerView.SetPosition(
        AdRequest request = new AdRequest.Builder().AddTestDevice("AF5A92326D4C3E240F138B6D85DBEA33").Build();
        bannerView.LoadAd(request);
        
        //todo remove
        bannerView.Hide();
    }
}
