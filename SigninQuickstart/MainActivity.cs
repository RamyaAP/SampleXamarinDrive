using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common.Apis;
using Android.Support.V7.App;
using Android.Gms.Common;
using Android.Util;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Auth.Api;
using Android.Gms.Drive;using Java.Lang;
using Newtonsoft.Json;
using RestSharp;
using Task = Android.Gms.Tasks.Task;
using Xamarin.Auth;
using Exception = System.Exception;

namespace SigninQuickstart
{
	[Activity (MainLauncher = true, Theme = "@style/ThemeOverlay.MyNoTitleActivity")]
	[Register("com.xamarin.signinquickstart.MainActivity")]
	public class MainActivity : AppCompatActivity, View.IOnClickListener
    {
		const string TAG = "MainActivity";

		const int RC_SIGN_IN = 9001;

		GoogleApiClient mGoogleApiClient;
		TextView mStatusTextView;
		ProgressDialog mProgressDialog;
        private GoogleSignInClient client;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_main);

			mStatusTextView = FindViewById<TextView>(Resource.Id.status);
			FindViewById(Resource.Id.sign_in_button).SetOnClickListener(this);
			FindViewById(Resource.Id.sign_out_button).SetOnClickListener(this);
			FindViewById(Resource.Id.disconnect_button).SetOnClickListener(this);
           


        }

        private void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (e.IsAuthenticated)
            {
                
                   Log.Debug("tokentype", ""+e.Account.Properties["token_type"]);
                   Log.Debug("accessToken", "" + e.Account.Properties["access_token"]);
                
                   UploadFile(e.Account.Properties["access_token"], "ram");
                   //  SaveAccount(e.Account);
                   //  RetriveAccount();

            }
            else
            {
                
            }
        }


        public static void UploadFile(string accessToken, string parentId)
        {
            var client = new RestClient { BaseUrl = new Uri("https://www.googleapis.com/") };

            var request = new RestRequest($"/upload/drive/v3/files?uploadType=multipart&access_token={accessToken}", Method.POST);
            // Stream stream = Application.Context.Assets.Open("myPdf");
            byte[] t;
            AssetManager assets = Application.Context.Assets;
            using (StreamReader sr = new StreamReader(assets.Open("myPdf.pdf")))
            {
                 t = default(byte[]);
                using (var memstream = new MemoryStream())
                {
                    sr.BaseStream.CopyTo(memstream);
                    t = memstream.ToArray();
                }

            }


            var bytes = t;// File.ReadAllBytes(@"C:\Users\z003d4ks\source\repos\Ramya\Ramya\ram\mypdf.pdf");

            var  content = new { title = "myPdf.pdf", description = "myPdf.pdf", parents = new[] { new { id = parentId } }, mimeType = "application/pdf" };

            var data = JsonConvert.SerializeObject(content);

            request.AddFile("content", Encoding.UTF8.GetBytes(data), "content", "application/json; charset=utf-8");

            request.AddFile("myPdf.pdf", bytes, "myPdf.pdf", "application/pdf");

            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK) throw new Exception("Unable to upload file to google drive");
        }

       

        private async void SaveAccount(Account account)
        {
            await SecureStorageAccountStore.SaveAsync(account, "Gmail");
        }

        private async void RetriveAccount()
        {
            var accounts = await SecureStorageAccountStore.FindAccountsForServiceAsync("Gmail");
        }

        protected override void OnStart()
		{
			base.OnStart();

		
		}
       

        protected override void OnResume()
		{
			base.OnResume();
			
		}

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            Log.Debug(TAG, "onActivityResult:" + requestCode + ":" + resultCode + ":" + data);

        }

        public void HandleSignInResult(GoogleSignInResult result)
		{
			Log.Debug(TAG, "handleSignInResult:" + result.IsSuccess);
			
		}

		
		void SignOut()
		{
          
          

        }

       
       


        void RevokeAccess()
        {
            RetriveAccount();


        }

	
		protected override void OnStop()
		{
			base.OnStop();
			//mGoogleApiClient.Disconnect();
		}

		public void ShowProgressDialog()
		{
			if (mProgressDialog == null)
			{
				mProgressDialog = new ProgressDialog(this);
				mProgressDialog.SetMessage(GetString(Resource.String.loading));
				mProgressDialog.Indeterminate = true;
			}

			mProgressDialog.Show();
		}

		public void HideProgressDialog()
		{
			if (mProgressDialog != null && mProgressDialog.IsShowing)
			{
				mProgressDialog.Hide();
			}
		}

		public void UpdateUI (bool isSignedIn)
		{
			if (isSignedIn)
			{
				FindViewById(Resource.Id.sign_in_button).Visibility = ViewStates.Gone;
				FindViewById(Resource.Id.sign_out_and_disconnect).Visibility = ViewStates.Visible;
			}
			else
			{
				mStatusTextView.Text = GetString(Resource.String.signed_out);

				FindViewById(Resource.Id.sign_in_button).Visibility = ViewStates.Visible;
				FindViewById(Resource.Id.sign_out_and_disconnect).Visibility = ViewStates.Gone;
			}
		}

		public void OnClick(View v)
		{
			switch (v.Id)
			{
				case Resource.Id.sign_in_button:
                    Constants.Authenticator = new OAuth2Authenticator(
                        Constants.ClientId,
                        null,
                        "https://www.googleapis.com/auth/drive",
                        new Uri(Constants.AuthorizeUrl),
                        new Uri("com.sample.xamarin.app:/oauth2redirect"),
                        new Uri(Constants.AccessTokenUrl),
                        null,
                        true);
                    Constants.Authenticator.Completed += OnAuthCompleted;

                    Xamarin.Auth.CustomTabsConfiguration.CustomTabsClosingMessage = null;

                    Intent loginUI = Constants.Authenticator.GetUI(this);
                    StartActivity(loginUI);
                    break;
				case Resource.Id.sign_out_button:
					SignOut();
					break;
				case Resource.Id.disconnect_button:
					RevokeAccess();
					break;
			}
		}

    
    }
}


