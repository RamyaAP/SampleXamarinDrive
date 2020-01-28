using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Android.Gms.Drive;
using Java.IO;
using Java.Lang;
using Newtonsoft.Json;

using Org.Apache.Http.Client.Methods;
using Org.Apache.Http.Entity;
using Org.Apache.Http.Impl.Client;
using RestSharp;
using Task = Android.Gms.Tasks.Task;
using Xamarin.Auth;
using Console = System.Console;
using Exception = System.Exception;
using File = System.IO.File;

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

        /// <summary>
        /// Vatan
        /// </summary>
        /// <param name="savedInstanceState"></param>
        
        private static HttpClient _httpClient = new HttpClient();

        public MainActivity()
        {
                _httpClient.BaseAddress = new Uri("https://www.googleapis.com");
        }


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
                   UploadFileUsingResumable(e.Account.Properties["access_token"], "");
                   //GetGdriveItemsInfoAsync(e.Account.Properties["access_token"]);
                   //Delete(e.Account.Properties["access_token"],"1EgR-GS5PxmIC92n2IruRuPFZJ8gLXKRzHzpZ9ZTKKJ8");
                   //CreateFolder(e.Account.Properties["access_token"], "Backup-MobileFitting");
                   //GetFoldersByBrand(e.Account.Properties["access_token"]);
                   //UploadFileUsingResumable(e.Account.Properties["access_token"],
                   //    "17sZ12Rgrfd_zsvlxCnuQzHbedNS9uTnS", "");
                   //DownloadFile(e.Account.Properties["access_token"], "1_OUGARZznIekMIvJdUY92B6mcMcrev6L");
                   //CreateFolder(e.Account.Properties["access_token"]);
                   //  var a=System.Threading.Tasks.Task.Run(() => CreateFolder(e.Account.Properties["access_token"]));
                   //   UploadFile(e.Account.Properties["access_token"], "1HaEE_nmZQc95J9g_bGYMl-2DQ0pQ_R4m");
                   //  SaveAccount(e.Account);
                   //  RetriveAccount();
                   //System.Threading.Tasks.Task.Run(() =>  CreateFolder(e.Account.Properties["access_token"]) );
                   //   System.Threading.Tasks.Task.Run(() =>  GetFolders(e.Account.Properties["access_token"]) );
                   //  System.Threading.Tasks.Task.Run(() => Delete(e.Account.Properties["access_token"]));
                   //DownloadFile(e.Account.Properties["access_token"],"");
            }
            else
            {
                
            }
        }

        public static async Task<bool> UploadFileUsingResumable(string accessToken, string filePath)
        {
            //Check if expected folder exist
            var folder = (await GetFoldersByBrand(accessToken, "Backup-MobileFitting")).FirstOrDefault();

            if (folder!=null)
            {
                //Check if file already exist to the folder.
                if (await IsFileExistForFolder("t",folder.Id,accessToken))
                {
                    return false;
                }
                else
                {
                    await UploadFile(accessToken, folder.Id);
                }
            }
            else
            {
                    CreateFolder(accessToken,"Backup-MobileFitting");
                    await UploadFileUsingResumable(accessToken, filePath);
            }
            return true;
        }

        private static async System.Threading.Tasks.Task UploadFile(string accessToken, string folderId)
        {
            //Look for filePath correction
            //FileInfo file = new FileInfo(filePath);
            var sessionRequest = new HttpRequestMessage(HttpMethod.Post, "upload/drive/v3/files?uploadType=resumable");
            sessionRequest.Headers.Add("Authorization", "Bearer " + accessToken);
            sessionRequest.Headers.Add("X-Upload-Content-Type", "*/*");
            sessionRequest.Headers.Add("X-Upload-Content-Length", GetFileSize().ToString());

            string body = "{\"name\": \"" + "t" + "\", \"parents\": [\"" + folderId + "\"]}";
            sessionRequest.Content = new StringContent(body);
            sessionRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var sessionResponse = await _httpClient.SendAsync(sessionRequest);
            var sessionUri = sessionResponse.Headers.Location;

            Stream stream = Application.Context.Assets.Open("t");

            var uploadRequest = new HttpRequestMessage(HttpMethod.Post, sessionUri.PathAndQuery);

            uploadRequest.Headers.Add("Authorization", "Bearer " + accessToken);

            //Creating Content (Body)
            HttpContent content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue("*/*");
            content.Headers.ContentLength = GetFileSize();

            //Attaching body to request
            uploadRequest.Content = content;

            try
            {
                var response =
                    await _httpClient.SendAsync(uploadRequest);
                response.EnsureSuccessStatusCode();
                var contents = await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static async Task<bool> IsFileExistForFolder(string fileName, string folderId, string accessToken)
        {
            var listFileInfo = await GetFiles(folderId,accessToken);

            var isFileExist = listFileInfo.Files.Any(x=>x.Name.ToLower().Equals(fileName.ToLower()) 
                                                        && x.Trashed==false);
            return isFileExist;
        }

        public static async Task<GoogleDriveItems> GetFiles(string folderId, string accessToken)
        {
            //https://www.googleapis.com/drive/v3/files?q='17sZ12Rgrfd_zsvlxCnuQzHbedNS9uTnS'+in+parents&fields=files%28id%2C+name%2C+parents%29
            var request = new HttpRequestMessage(HttpMethod.Get, 
                $"drive/v3/files?q='{folderId}'+in+parents&fields=files%28id%2C+name%2C+trashed%2C+parents%29");
            request.Headers.Add("Authorization", "Bearer "+ accessToken);
            request.Headers.Add("Accept", "application/json");
            request.Headers.CacheControl = new CacheControlHeaderValue(){NoCache = true};

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GoogleDriveItems>(content);
        }

        public static async void DownloadFile(string accessToken, string fileId)
        {
            //path error vatan after download
            var request = new HttpRequestMessage(HttpMethod.Get, $"drive/v3/files/{fileId}?fields=*&alt=media");
            request.Headers.Add("Authorization", "Bearer "+ accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var  contents = await response.Content.ReadAsStreamAsync();
                Stream inputStream = contents;
                string path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                    "Backup-MobileFitting");

                Stream outputStream = System.IO.File.OpenWrite(Path.Combine(path, "t.1"));
                await inputStream.CopyToAsync(outputStream);
            }
        }

        public static long GetFileSize()
        {
            byte[] t;
            AssetManager assets = Application.Context.Assets;
            using (StreamReader sr = new StreamReader(assets.Open("t")))
            {
                t = default(byte[]);
                using (var memstream = new MemoryStream())
                {
                    sr.BaseStream.CopyTo(memstream);
                    t = memstream.ToArray();
                }

            }
            return t.Length;
        }

        public static async void CreateFolder(string accessToken, string brandFolderName)
        {
            var gDriveItems = await GetFoldersByBrand(accessToken,brandFolderName);
            if (gDriveItems.Any(x=>x.Name.ToLower() == brandFolderName.ToLower()))
            {
                return;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "drive/v3/files");
            request.Headers.Add("Authorization", "Bearer "+ accessToken);
            
            JsonObject jsonFolderObject = new JsonObject();
            jsonFolderObject.Add("name", brandFolderName);
            jsonFolderObject
                .Add("mimeType", "application/vnd.google-apps.folder");
            var data = JsonConvert.SerializeObject(jsonFolderObject);

            request.Content = new StringContent(data, Encoding.UTF8, "application/json");
            var responce = await _httpClient.SendAsync(request);
            var mm = responce.Content.ReadAsStringAsync();
            responce.EnsureSuccessStatusCode();
        }

        public static async Task<List<GoogleDriveItemInfo>> GetFoldersByBrand(string accessToken, string brand="Backup-MobileFitting")
        {
            //https://www.googleapis.com/drive/v3/files?supportsAllDrives=true&corpora=allDrives&includeItemsFromAllDrives=true&pageSize=1000&q=name%20%3D%20'Backup-MobileFitting'&fields=files%28id%2C+name%2C+parents%2C+modifiedTime%2C+trashed%2C+mimeType%29&files[orderBy]=modifiedTime
            
            var request = new HttpRequestMessage(HttpMethod.Get, $"drive/v3/files?supportsAllDrives=true&corpora=allDrives&includeItemsFromAllDrives=true&pageSize=1000&q=name%20%3D%20'{brand}'&fields=files%28id%2C+name%2C+parents%2C+modifiedTime%2C+trashed%2C+mimeType%29&files[orderBy]=modifiedTime");
            request.Headers.Add("Authorization", "Bearer "+ accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var items = JsonConvert.DeserializeObject<GoogleDriveItems>(content);

            return items.Files.Where(x => x.MimeType == "application/vnd.google-apps.folder" && x.Trashed==false).ToList();

        }

        /// <summary>
        /// It deletes file and folder based on ItemId
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="itemId"></param>
        public static async void Delete(string accessToken, string itemId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"drive/v3/files/{itemId}");
            request.Headers.Add("Authorization", "Bearer "+ accessToken);

            var response =
                await _httpClient.SendAsync(request);

            var content = response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
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