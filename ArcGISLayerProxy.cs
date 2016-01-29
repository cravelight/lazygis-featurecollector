using System.Collections.Generic;
using LazyGIS.FeatureCollector.Dto;
using Newtonsoft.Json;

namespace LazyGIS.FeatureCollector
{
    public class ArcGISLayerProxy
    {
        public ArcGISLayerProxy(string layerUrl)
        {
            _baseUrl = layerUrl;
            LoadLayerDetails();
            LoadIdsForObjectsInLayer();
        }
        private readonly string _baseUrl;



        #region Properties

        public string Url 
        {
            get { return _baseUrl; }
        }

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

        #endregion // Properties



        #region Layer Details

        public ArcGISLayerDetailsDto GetLayerDetails()
        {
            return _layerDetails;
        }

        private void LoadLayerDetails()
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

        private void LoadIdsForObjectsInLayer()
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
