using System.Collections.Generic;

namespace LazyGIS.FeatureCollector.Dto
{
    /// <summary>
    /// deserialization template for feature details collection json
    /// all we need is the features, so we don't deserialize anything but the collection
    /// "get features where objectid &gt; 0 and objectid &lt; 10" 
    /// Sample: http://ww1.bucoks.com/arcgis/rest/services/BuCoKs_NGStreets/MapServer/0/query?where=objectid+%3E+0+and+objectid+%3C+10&returnGeometry=true&outFields=*&f=pjson
    /// </summary>
    public class ArcGISFeatureDetailsCollectionDto
    {
        public List<dynamic> features { get; set; }
    }
}
