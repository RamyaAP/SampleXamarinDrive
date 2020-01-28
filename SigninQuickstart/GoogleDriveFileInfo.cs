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

namespace SigninQuickstart
{
    public class GoogleDriveItems
    {
        public GoogleDriveItemInfo[] Files { get; set; }
    }

    public class GoogleDriveItemInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Parents { get; set; }
        public string MimeType { get; set; }
        public DateTime ModifiedTime { get; set; }
        public bool Trashed { get; set; }
    }
}