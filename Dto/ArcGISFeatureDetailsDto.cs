using System.Collections.Generic;

namespace LazyGIS.FeatureCollector.Dto
{
    /// <summary>
    /// deserialization template for feature details json 
    /// Sample: http://ww1.bucoks.com/arcgis/rest/services/BuCoKs_NGStreets/MapServer/0/1?f=pjson
    /// </summary>
    public class ArcGISFeatureDetailsDto
    {
        public dynamic feature { get; set; }
    }
}
