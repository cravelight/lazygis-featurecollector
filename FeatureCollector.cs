using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyGIS.FeatureCollector.Dto;
using Newtonsoft.Json;

namespace LazyGIS.FeatureCollector
{
    public class FeatureCollector
    {

        /// <summary>
        /// Gathers the features and writes them to json in batches 
        /// </summary>
        /// <param name="pathBase"></param>
        /// <param name="batchSizeInThousands"></param>
        /// <returns></returns>
        public List<string> WriteFeaturesToFiles(string pathBase, int batchSizeInThousands = 50)
        {
            var fileList = new List<string>();
            var batchSize = batchSizeInThousands*1000;
            var baseDto = GetBaseDtoForFeatureSet();

            _proxy.ObjectIds.Sort(); // make sure ids are in numerical order

            var objectIdBatches = _proxy.ObjectIds.ChunkBy(batchSize);
            var batchNum = 1;
            foreach (var batch in objectIdBatches)
            {
                // get the features for this batch and place them on the baseDto
                var features = GetFeaturesForBatch(batch);
                baseDto.features = features;

                // write the dto to disk
                var filename = string.Format("{0}_{1}.json", _proxy.Url.AsValidFilename(), batchNum);
                var path = Path.Combine(pathBase, filename);
                WriteFeaturesetToDisk(baseDto, path);

                // add the file path to the return list
                fileList.Add(path);

                batchNum++;
                baseDto.features = null; // encourage garbage collection?
            }

            return fileList;
        }

        private void WriteFeaturesetToDisk(ArcGISFeatureSetDto featureSet, string path)
        {
            var featuresetJson = JsonConvert.SerializeObject(featureSet, Formatting.Indented);
            File.WriteAllText(path, featuresetJson);
        }

        private List<dynamic> GetFeaturesForBatch(List<int> batch)
        {
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
            var objectIdLists = batch.ChunkBy(500); // break the object list into groups of 500
            var allFeatures = new List<dynamic>();
            foreach (var list in objectIdLists)
            {
                var lower = list.Min() - 1;
                var upper = list.Max() + 1;
                allFeatures.AddRange(GetFeaturesFromServerInRange(lower, upper));
            }
            return allFeatures;
        }

        private List<dynamic> GetFeaturesFromServerInRange(int startObjectId, int endObjectId)
        {
            // does not include start/end ids
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString(GetFeatureDetailsCollectionUrl(startObjectId, endObjectId));
                var dto = JsonConvert.DeserializeObject<ArcGISFeatureDetailsCollectionDto>(json);
                return dto.features;
            }
        }

        private string GetFeatureDetailsCollectionUrl(int startObjectId, int endObjectId)
        {
            // does not include start/end ids
            const string template = "{baseurl}/query?where=objectid+%3E+{startId}+and+objectid+%3C+{endId}&returnGeometry=true&outFields=*&f=pjson";
            var url = template.Replace("{baseurl}", _proxy.Url);
            url = url.Replace("{startId}", startObjectId.ToString());
            return url.Replace("{endId}", endObjectId.ToString());
        }

        private ArcGISFeatureSetDto GetBaseDtoForFeatureSet()
        {
            var layerDetails = _proxy.GetLayerDetails();
            var dto = new ArcGISFeatureSetDto
            {
                displayFieldName = layerDetails.displayField,
                fields = layerDetails.fields,
                geometryType = layerDetails.geometryType,
                spatialReference = layerDetails.extent["spatialReference"],
                fieldAliases = GetAliasesFromFieldList(layerDetails.fields)
            };
            return dto;
        }

        private Dictionary<string, string> GetAliasesFromFieldList(List<dynamic> fields)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var field in fields)
            {
                string name = field["name"];
                string alias = field["alias"];
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(alias))
                {
                    continue;
                }
                dictionary.Add(name, alias);
            }
            return dictionary;
        }

        
        
        public FeatureCollector(ArcGISLayerProxy proxy)
        {
            _proxy = proxy;
        }
        private readonly ArcGISLayerProxy _proxy;

    }
}
