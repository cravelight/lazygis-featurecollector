using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LazyGIS.FeatureCollector
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            txtSourceDataUrls.Text = @"http://ww1.bucoks.com/arcgis/rest/services/BuCoKs_NGStreets/MapServer/0" +
                                     Environment.NewLine + 
                                     @"http://ww1.bucoks.com/arcgis/rest/services/BuCoKs_NGAddressPoints/MapServer/0";
        }

        private void btnInspect_Click(object sender, EventArgs e)
        {
            ResetMessageBox();
            GatherUrlCollection();
            if (_sourceDataContainsBadUrl)
            {
                ResetUrlCollection();
                DisableDataCollectionButton();
                return;
            }

            InspectUrlCollection();


        }

        private void btnCollect_Click(object sender, EventArgs e)
        {

        }


        private void InspectUrlCollection()
        {
            foreach (var url in _urlCollection)
            {
                var layer = new ArcGISLayerProxy(url.ToString());
                AddMessage(string.Format(
                    "{0} is a {1} with {2} records.",
                    layer.Name,
                    layer.Type,
                    layer.RecordCount));
            }
        }




        private void GatherUrlCollection()
        {
            _sourceDataContainsBadUrl = false;
            foreach (var line in txtSourceDataUrls.Lines)
            {
                Uri uriResult;
                var url = ReplaceWhitespace(line, string.Empty);

                if (string.IsNullOrWhiteSpace(url)) { continue; }
                
                var isValidUrl = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                                 && (uriResult.Scheme == Uri.UriSchemeHttp
                                     || uriResult.Scheme == Uri.UriSchemeHttps);
                
                if (!isValidUrl)
                {
                    AddMessage(string.Format("Error: this is not a url -> {0}", url));
                    _sourceDataContainsBadUrl = true;
                    continue;
                }
                _urlCollection.Add(uriResult);
            }
        }
        private bool _sourceDataContainsBadUrl;


        
        private void ResetUrlCollection()
        {
            _urlCollection = new List<Uri>();
        }
        private List<Uri> _urlCollection = new List<Uri>();


        private void DisableDataCollectionButton()
        {
            btnCollect.Enabled = false;
        }




        private void AddMessage(string message)
        {
            txtMessageBox.AppendText(message + Environment.NewLine);
        }

        private void ResetMessageBox()
        {
            txtMessageBox.Text = string.Empty;
        }





        public static string ReplaceWhitespace(string input, string replacement)
        {
            return Whitespace.Replace(input, replacement);
        }
        private static readonly Regex Whitespace = new Regex(@"\s+");

    }
}
