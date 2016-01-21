using System.Collections.Generic;

namespace LazyGIS.FeatureCollector.Dto
{
    /// <summary>
    /// deserialization template for id list json response
    /// Sample: http://ww1.bucoks.com/arcgis/rest/services/BuCoKs_NGStreets/MapServer/0/query?where=1%3D1&returnIdsOnly=true&f=pjson
    /// </summary>
    internal class ArcGISObjectIdCollectionDto
    {
        public string objectIdFieldName { get; set; }
        public List<int> objectIds { get; set; }
    }
}