using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

using Org.Apache.Http.Client.Methods;
using Org.Apache.Http.Entity;
using Org.Apache.Http.Impl.Client;
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
                   UploadFileUsingResumable(e.Account.Properties["access_token"]);

                   //  var a=System.Threading.Tasks.Task.Run(() => CreateFolder(e.Account.Properties["access_token"]));
                   //   UploadFile(e.Account.Properties["access_token"], "1HaEE_nmZQc95J9g_bGYMl-2DQ0pQ_R4m");
                   //  SaveAccount(e.Account);
                   //  RetriveAccount();
                   //System.Threading.Tasks.Task.Run(() =>  CreateFolder(e.Account.Properties["access_token"]) );
                   //   System.Threading.Tasks.Task.Run(() =>  GetFolders(e.Account.Properties["access_token"]) );
                   //  System.Threading.Tasks.Task.Run(() => Delete(e.Account.Properties["access_token"]));
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

          var a= "{ 'name' : 'kkPdf.pdf', 'mimeType': 'application/pdf', 'parents' : [{'id': '1ZDFrh_idqo3GcY3QR-BSBgaBXLNwkX0x'}] }";

var  content = new { name = "kkPdf.pdf", description = "kkPdf.pdf", parents = new[] { new { id = parentId } }, mimeType = "application/pdf" };

            var data = JsonConvert.SerializeObject(content);

           

    request.AddFile("content", Encoding.UTF8.GetBytes(a), "content", "application/json; charset=utf-8");

            request.AddFile("kkPdf.pdf", bytes, "kkPdf.pdf", "application/pdf");
            

            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK) throw new Exception("Unable to upload file to google drive");
        }

        public static async void UploadFileUsingResumable(string accessToken)
        {

            // FileInfo fi = new FileInfo(@"D:\Data\Vatan\Neon Team\Ramya\2\SampleXamarinDrive\ConsoleApp1\PDF\Helloworld.pdf");
          

           // AssetFileDescriptor fd = Application.Context.Assets.OpenFd("myPdf.pdf");
            //long size = fd.Length;

            HttpClient client = new HttpClient();



                string body = "{\"name\": \"" + "myPdf.pdf" + "\", \"parents\": [\"" + "1kp8yKVoiKJoBNdZntZArv7jdpb8iCmJn" + "\"]}";



                client.DefaultRequestHeaders.Add("Authorization", "Bearer "+ accessToken); //= new AuthenticationHeaderValue("Bearer ya29.Il-6B4y29VzdgU3fR8a9fcxBun8wHBhn0MbO3KQRcdeNDKK84KWShETsV0Dj1TnFfGcL68irFqQskQf-TzyRmXZl2OI0ZV2jCZ1J6v7I0F-EV8VYPM8FeiUVwdT6596yGA"); 
                client.DefaultRequestHeaders.Add("X-Upload-Content-Type", "application/pdf");
                client.DefaultRequestHeaders.Add("X-Upload-Content-Length", "31521");
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post,
                    "https://www.googleapis.com/upload/drive/v3/files?uploadType=resumable");
                request.Content = new StringContent(body);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response1 = await client.SendAsync(request);
                var uri = response1.Headers.Location;

                //client.DefaultRequestHeaders.Add("Content-Type","application/json; charset=UTF-8");
                // var bodyLength = Encoding.ASCII.GetBytes(body).Length;
                //client.DefaultRequestHeaders.Add("Content-Length",bodyLength);

            //HttpContent ct = new StringContent(body);
            //    ct.Headers.ContentLength = bodyLength;
            //    ct.Headers.ContentType = new MediaTypeHeaderValue("application/json");



            //    try
            //    {
            //        var response =
            //            await client.PostAsync("https://www.googleapis.com/upload/drive/v3/files?uploadType=resumable",
            //                ct);
            //        string Location = response.Headers.Location.AbsolutePath;
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e);
            //        throw;
            //    }





        }

        public static async void UploadFileUsingHttpClient(string token)
        {

            HttpClient client = new HttpClient();

            MultipartFormDataContent form = new MultipartFormDataContent();

            HttpContent content = new StringContent("fileToUpload");
            form.Add(content, "fileToUpload");
            
            Stream stream = Application.Context.Assets.Open("myPdf");
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "myPdf",
                FileName = "myPdf",
                
            };
            form.Add(content);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsync("https://www.googleapis.com/drive/v3/files", form);
        }
        public static async void CreateFolder(string token)

        {
            var client= new HttpClient();
            JsonObject jsonObject = new JsonObject();
            jsonObject.Add("name", "Test folder");
            jsonObject
                .Add("mimeType", "application/vnd.google-apps.folder");
            var data = JsonConvert.SerializeObject(jsonObject);
          
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
           var response= await client.PostAsync("https://www.googleapis.com/drive/v3/files",new StringContent(data,Encoding.UTF8, "application/json"));


        }

        public static async void GetFolders(string token)

        {
            var client = new HttpClient();
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
           
            var response =
                await client.GetAsync(
                    "https://www.googleapis.com/drive/v3/files");
            var contents = await response.Content.ReadAsStringAsync();
        }

        public static async void Delete(string token)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response =
                await client.DeleteAsync(
                    "https://www.googleapis.com/drive/v3/files/1hgmhv7ALyuaL33eFK-iYRu3ojgtZZpxh");
            var contents = await response.Content.ReadAsStringAsync();
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


