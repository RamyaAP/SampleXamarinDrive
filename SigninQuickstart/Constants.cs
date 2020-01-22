using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Auth;

namespace SigninQuickstart
{
    public static class Constants
    {
        public static string ClientId = "798650191479-d6j0akko10v2ao5cdhvirol8n09sqb90.apps.googleusercontent.com";
        public static string Scope = "email";
        public const string AuthorizeUrl = "https://accounts.google.com/o/oauth2/v2/auth";
        public const string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        public static  OAuth2Authenticator Authenticator;

    }
}