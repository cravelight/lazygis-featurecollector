using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        #region ArcGISFeatureSet builder

        public ArcGISFeatureSetDto GetArcGISFeatureSetObjectForLayer()
        {
            var dto = new ArcGISFeatureSetDto
            {
                displayFieldName = _layerDetails.displayField,
                fields = _layerDetails.fields,
                geometryType = _layerDetails.geometryType,
                spatialReference = _layerDetails.extent["spatialReference"],
                fieldAliases = _layerDetails.fields.ToDictionary<dynamic, string, string>(field => field["alias"], field => field["name"]),
                features = GetFeaturesForLayer()
            };

            return dto;
        }

        private List<dynamic> GetFeaturesForLayer()
        {
            var allFeatures = new List<dynamic>();

            ObjectIds.Sort(); // make sure ids are in numerical order

            /* cycle through the object ids in groups and add the range
             * 
             * Why 500? The ArcGIS servers will only return data for 1000 records at a time.
             * 
             * I tried a variety of numbers and found that chunks less than 500 tended to be
             * slow because of too much internet overhead.  The overall time for ~9000 records
             * was the same when using chunks of either 500 or 1000. Given that neither made a 
             * big difference in performance I chose the smaller number to decrease the risk of
             * a slow connection timing out with the bigger payload you get with chunks of 1000.
             */
            var objectIdLists = ObjectIds.ChunkBy(500); // break the object list into groups of 500
            foreach (var list in objectIdLists)
            {
                var lower = list.Min() - 1;
                var upper = list.Max() + 1;
                allFeatures.AddRange(GetFeaturesInRange(lower, upper));
            }

            return allFeatures;
        }

        // does not include start/end ids
        private List<dynamic> GetFeaturesInRange(int startObjectId, int endObjectId)
        {
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString(GetFeatureDetailsCollectionUrl(startObjectId, endObjectId));
                var dto = JsonConvert.DeserializeObject<ArcGISFeatureDetailsCollectionDto>(json);
                return dto.features;
            }
        }

        // does not include start/end ids
        private string GetFeatureDetailsCollectionUrl(int startObjectId, int endObjectId)
        {
            const string template = "{baseurl}/query?where=objectid+%3E+{startId}+and+objectid+%3C+{endId}&returnGeometry=true&outFields=*&f=pjson";
            var url = template.Replace("{baseurl}", _baseUrl);
            url = url.Replace("{startId}", startObjectId.ToString());
            return url.Replace("{endId}", endObjectId.ToString());
        }


        #region worked but slow

        //ObjectIds.Select(id => GetFeatureForObject(id)).ToList()

        // this worked but getting them one at a time took forever
        private dynamic GetFeatureForObject(int objectId)
        {
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString(GetFeatureDetailsUrl(objectId));
                var dto = JsonConvert.DeserializeObject<ArcGISFeatureDetailsDto>(json);
                return dto.feature;
            }
        }

        // this worked but getting them one at a time took forever
        private string GetFeatureDetailsUrl(int objectId)
        {
            const string template = "{baseurl}/{id}?f=pjson";
            var url = template.Replace("{baseurl}", _baseUrl);
            return url.Replace("{id}", objectId.ToString());
        }
        
        #endregion // worked but slow

        #endregion // ArcGISFeatureSet builder



        #region Layer Details

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


    // HT: http://stackoverflow.com/a/24087164
    /// <summary>
    /// Helper methods for the lists.
    /// </summary>
    public static class ListExtensions
    {
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }

}
