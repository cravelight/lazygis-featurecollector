using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using LazyGIS.FeatureCollector.Dto;
using Newtonsoft.Json;

namespace LazyGIS.FeatureCollector
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            //demo urls
            txtSourceDataUrls.Text = @"http://ww1.bucoks.com/arcgis/rest/services/BuCoKs_NGStreets/MapServer/0";
            //txtSourceDataUrls.Text = @"http://ww1.bucoks.com/arcgis/rest/services/BuCoKs_NGStreets/MapServer/0" +
            //                         Environment.NewLine + 
            //                         @"http://ww1.bucoks.com/arcgis/rest/services/BuCoKs_NGAddressPoints/MapServer/0";
        }

        private void btnInspect_Click(object sender, EventArgs e)
        {
            ShowWaitingCursor();
            ResetMessageBox();
            CollectLayerUrlsFromTextbox();
            if (_sourceDataContainsBadUrl)
            {
                ResetUrlCollection();
                DisableDataCollectionButton();
                ShowNormalCursor();
                return;
            }

            ResetLayerProxies();
            GetLayerProxiesForSourceData();
            EnableDataCollectionButton();
            ShowNormalCursor();
        }


        private void btnCollect_Click(object sender, EventArgs e)
        {
            ShowWaitingCursor();
            ResetMessageBox();

            foreach (var layer in _layerProxies)
            {
                var hasErrors = false;
                WriteMessageForProcessingStart(layer);
                try
                {
                    var featureset = layer.GetArcGISFeatureSetObjectForLayer();
                    var outpath = WriteFeaturesetToDisk(layer, featureset);
                    AddMessage("  created: " + outpath);
                }
                catch (Exception exception)
                {
                    AddMessage(exception.Message);
                    hasErrors = true;
                }
                WriteMessageForProcessingEnd(layer, hasErrors);
            }


            ShowNormalCursor();
        }

        private void txtSourceDataUrls_TextChanged(object sender, EventArgs e)
        {
            ShowWaitingCursor();
            DisableDataCollectionButton();
            ResetMessageBox();
            ShowNormalCursor();
        }



        #region Layer Processing

        private string WriteFeaturesetToDisk(ArcGISLayerProxy layer, ArcGISFeatureSetDto featureSet)
        {
            var filename = string.Format("{0}_{1}.json", 
                MakeValidFileNameFromUrl(layer.Url),
                DateTime.Now.ToString("yyyy-MM-dd_H-mm-ss"));
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename);

            var featuresetJson = JsonConvert.SerializeObject(featureSet, Formatting.Indented);

            File.WriteAllText(path, featuresetJson);
            return path;
        }


        private void GetLayerProxiesForSourceData()
        {
            foreach (var url in _urlCollection)
            {
                var layer = new ArcGISLayerProxy(url.ToString());
                _layerProxies.Add(layer);
                WriteInfoMessageForLayer(layer);
            }
        }
        private List<ArcGISLayerProxy> _layerProxies;

        private void ResetLayerProxies()
        {
            _layerProxies = new List<ArcGISLayerProxy>();
        }

        private void WriteInfoMessageForLayer(ArcGISLayerProxy layer)
        {
            AddMessage(string.Format(
                "{0} is a {1} with {2} records.",
                layer.Name,
                layer.Type,
                layer.RecordCount));
        }

        private void WriteMessageForProcessingStart(ArcGISLayerProxy layer)
        {
            AddMessage(string.Format(
                "----------" + Environment.NewLine +
                "Processing {0} ({1}, {2} records)",
                layer.Name,
                layer.Type,
                layer.RecordCount));
        }

        private void WriteMessageForProcessingEnd(ArcGISLayerProxy layer, bool hasErrors = false)
        {
            AddMessage(string.Format(
                "{0} finished {1}" + Environment.NewLine,
                layer.Name,
                hasErrors ? "with errors." : "successfully."));
        }

        public static string MakeValidFileNameFromUrl(string url)
        {
            return Path.GetInvalidFileNameChars().Aggregate(url, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        #endregion // Layer Processing



        #region Url Collection Management

        private void CollectLayerUrlsFromTextbox()
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

        
        public static string ReplaceWhitespace(string input, string replacement)
        {
            return Whitespace.Replace(input, replacement);
        }
        private static readonly Regex Whitespace = new Regex(@"\s+");
        
        #endregion // Url Collection Management



        #region MessageBox

        private void AddMessage(string message)
        {
            txtMessageBox.AppendText(message + Environment.NewLine);
        }

        private void ResetMessageBox()
        {
            txtMessageBox.Text = string.Empty;
        }

        #endregion // MessageBox



        #region UI Helpers

        private void DisableDataCollectionButton()
        {
            btnCollect.Enabled = false;
        }

        private void EnableDataCollectionButton()
        {
            btnCollect.Enabled = true;
        }

        private void ShowWaitingCursor()
        {
            Cursor.Current = Cursors.WaitCursor;
        }

        private void ShowNormalCursor()
        {
            Cursor.Current = Cursors.AppStarting;
        }

        #endregion // UI Helpers


    }
}
