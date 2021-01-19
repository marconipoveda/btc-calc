using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.Text;
using Xamarin.Essentials;

namespace CalcBTC
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        EditText btcs;
        EditText usds;
        TextView pricetoday;
        Button btn_updateprice;

        double priceInUSD;

        const string Url = "https://api.pro.coinbase.com/products/BTC-USD/ticker";
       
        //private double priceBC = 0.00;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            pricetoday = FindViewById<TextView>(Resource.Id.pricetoday);
            btcs = FindViewById<EditText>(Resource.Id.btcs);
            usds = FindViewById<EditText>(Resource.Id.usds);
            btn_updateprice = FindViewById<Button>(Resource.Id.btn_updateprice);
            GetPrice();
            btn_updateprice.Click += UpdateOnClic;
            btcs.TextChanged += CalculateUSDs;
            usds.TextChanged += CalculateBTCs;

            //Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            
            //SetSupportActionBar(toolbar);

        }

        void UpdateOnClic(object sender, EventArgs e)
        {
            GetPrice();
        }

        void CalculateUSDs(object sender, EventArgs e)
        {
            double inBtcs;
            if (!(string.IsNullOrEmpty(btcs.Text)))
            {
                if (double.TryParse(btcs.Text, out inBtcs))
                {
                    double outUSDs = priceInUSD * inBtcs;

                    usds.TextChanged -= CalculateBTCs;
                    usds.Text = outUSDs.ToString();
                    usds.TextChanged += CalculateBTCs;
                }
            }
            else
            {
                usds.TextChanged -= CalculateBTCs;
                usds.Text = "";
                usds.TextChanged += CalculateBTCs;
            }
        }
        void CalculateBTCs(object sender, EventArgs e)
        {
            double inUSDs;
            if (!(string.IsNullOrEmpty(usds.Text)))
            {
                if (double.TryParse(usds.Text, out inUSDs))
                {
                    double outBTCs = inUSDs / priceInUSD;

                    btcs.TextChanged -= CalculateUSDs;
                    btcs.Text = outBTCs.ToString();
                    btcs.TextChanged += CalculateUSDs;
                }
            }
            else
            {
                btcs.TextChanged -= CalculateUSDs;
                btcs.Text = "";
                btcs.TextChanged += CalculateUSDs;
            }
        }


            private async void GetPrice()
        {
            HttpClient client = new HttpClient();
            var result = await client.GetStringAsync(Url);

            BitcoinPrice myData = JsonConvert.DeserializeObject<BitcoinPrice>(result);
            pricetoday.Text = myData.Price;
            priceInUSD = Convert.ToDouble(myData.Price);
            System.Console.WriteLine("price: --> " + priceInUSD.ToString());
        }

        public class BitcoinPrice
        {
            public int Trade_id { get; set; }
            public string Price { get; set; }
            public string Size { get; set; }
            public string Time { get; set; }
            public string Bid { get; set; }
            public string Ask { get; set; }
            public string Volume { get; set; }
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}
