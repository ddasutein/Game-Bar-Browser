﻿using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameBarBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TabView : Page
    {
        private WebView _webView;
        public WebView WebView { get; private set; }
        public bool LoadingPage { get; private set; } = false;
        public bool CanGoBack => WebView.CanGoBack;
        public bool CanGoForward => WebView.CanGoForward;
        public string DocumentTitle => WebView.DocumentTitle;
        public string URL { get; private set; }

        public event Action<TabView> OnNavigationStart;
        public event Action<TabView> OnNavigationComplete;

        public TabView()
        {
            this.InitializeComponent();

            WebView = new WebView(WebViewExecutionMode.SameThread);
            Grid.SetRow(WebView, 0);
            content.Children.Add(WebView);

            WebView.NavigationStarting += HandleNavigationStarting; 
            WebView.NavigationCompleted += HandleNavigationCompleted;
        }

        public new async void Focus(FocusState focusState)
        {
            WebView.Focus(focusState);
            await WebView.InvokeScriptAsync(@"eval", new string[] { "document.focus()" });
        }

        public void Query(string url)
        {
            Debug.WriteLine("URL recieved!");
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                WebView.Navigate(new Uri(url));
            else
                WebView.Navigate(new Uri(string.Format(UserSettings.SearchEngineURL, url)));
        }

        public void Refresh()
        {
            if (LoadingPage)
                WebView.Stop();
            else
                WebView.Refresh();
        }

        public void GoBack()
        {
            if (!CanGoBack)
                return;

            WebView.GoBack();
        }

        public void GoForward()
        {
            if (!CanGoForward)
                return;

            WebView.GoForward();
        }

        private void HandleNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            URL = args.Uri.ToString();
            LoadingPage = true;

            OnNavigationStart?.Invoke(this);
        }

        private void HandleNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            URL = args.Uri.ToString();
            LoadingPage = false;

            OnNavigationComplete?.Invoke(this);
        }
    }
}
