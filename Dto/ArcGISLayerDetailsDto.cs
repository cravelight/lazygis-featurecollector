namespace LazyGIS.FeatureCollector.Dto
{
    /// <summary>
    /// deserialization template for layer details json 
    /// Sample: http://ww1.bucoks.com/arcgis/rest/services/BuCoKs_NGStreets/MapServer/0?f=pjson
    /// not all properties are mapped
    /// </summary>
    public class ArcGISLayerDetailsDto
    {
        public string name { get; set; }
        public string type { get; set; }
        //public string description { get; set; }
    }
}
