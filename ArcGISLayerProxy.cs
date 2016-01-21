using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyGIS.FeatureCollector.Dto;
using Newtonsoft.Json;

namespace LazyGIS.FeatureCollector
{
    public class ArcGISLayerProxy
    {
        private readonly string _baseUrl;

        public ArcGISLayerProxy(string layerUrl)
        {
            _baseUrl = layerUrl;
            PopulateLayerDetails();
            PopulateIdList();
        }

        public const string ObjectRecordTemplate = "{baseurl}/{id}?f=pjson";



        #region Layer Details

        /// <summary>
        /// The name of the layer
        /// </summary>
        public string Name 
        {
            get { return _layerDetails.name; }
        }

        /// <summary>
        /// The type of the layer
        /// </summary>
        public string Type
        {
            get { return _layerDetails.type; }
        }

        
        private void PopulateLayerDetails()
        {
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString(GetUrlForLayerDetails());
                _layerDetails = JsonConvert.DeserializeObject<ArcGISLayerDetailsDto>(json);
            }
        }

        private ArcGISLayerDetailsDto _layerDetails;

        private string GetUrlForLayerDetails()
        {
            const string template = "{baseurl}?f=pjson";
            return template.Replace("{baseurl}", _baseUrl);
        }

        #endregion // Layer Details



        #region Object ID Collection

        /// <summary>
        /// List of the objectids available in the layer
        /// </summary>
        public List<int> ObjectIds { get; private set; }

        /// <summary>
        /// The number of records in the layer
        /// </summary>
        public int RecordCount 
        {
            get { return ObjectIds.Count; }
        }

        private void PopulateIdList()
        {
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString(GetUrlForIdList());
                var dto = JsonConvert.DeserializeObject<ArcGISObjectIdCollectionDto>(json);
                ObjectIds = dto.objectIds;
            }
        }

        private string GetUrlForIdList()
        {
            const string template = "{baseurl}/query?where=1%3D1&returnIdsOnly=true&f=pjson";
            return template.Replace("{baseurl}", _baseUrl);
        }

        #endregion //Object ID Collection

    }
}
