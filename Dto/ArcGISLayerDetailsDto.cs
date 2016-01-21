using System.Collections.Generic;

namespace LazyGIS.FeatureCollector.Dto
{
    /// <summary>
    /// deserialization template for layer details json 
    /// not all properties are mapped
    /// Sample: http://ww1.bucoks.com/arcgis/rest/services/BuCoKs_NGStreets/MapServer/0?f=pjson
    /// </summary>
    public class ArcGISLayerDetailsDto
    {
        public string name { get; set; }
        public string type { get; set; }
        //public string description { get; set; }
        public string displayField { get; set; }
        public List<dynamic> fields { get; set; }
        public string geometryType { get; set; }
        public dynamic extent { get; set; } // contains spatialReference which we need

    }
}
