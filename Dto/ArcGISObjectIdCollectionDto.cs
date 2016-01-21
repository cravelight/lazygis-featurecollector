using System.Collections.Generic;

namespace LazyGIS.FeatureCollector.Dto
{
    /// <summary>
    /// deserialization template for id list json response
    /// </summary>
    internal class ArcGISObjectIdCollectionDto
    {
        public string objectIdFieldName { get; set; }
        public List<int> objectIds { get; set; }
    }
}