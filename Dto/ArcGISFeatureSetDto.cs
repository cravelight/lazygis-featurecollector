using System.Collections.Generic;

namespace LazyGIS.FeatureCollector.Dto
{
    /// <summary>
    /// Represents an ArcGIS REST API specification for featureset. 
    /// From: http://pro.arcgis.com/en/pro-app/tool-reference/conversion/an-overview-of-the-json-toolset.htm
    /// </summary>
    /// <example>
    /// JSON representation of Featureset
    /// { 
    ///     "displayFieldName" : "<displayFieldName>",
    ///     "fieldAliases" : {
    ///         "<fieldName1>" : "<fieldAlias1>",
    ///         "<fieldName2>" : "<fieldAlias2>"
    ///     },
    ///     "geometryType" : "<geometryType>",
    ///     "hasZ" : <true|false>,    // Added at 10.1
    ///     "hasM" : <true|false>,    // Added at 10.1
    ///     "spatialReference" : <spatialReference>,
    ///     "fields": [
    ///         {
    ///             "name": "<field1>",
    ///             "type": "<field1Type>",
    ///             "alias": "<field1Alias>"
    ///         },{
    ///             "name": "<field2>",
    ///             "type": "<field2Type>",
    ///             "alias": "<field2Alias>"
    ///         }
    ///     ],
    ///     "features": [
    ///         {
    ///             "geometry": { <geometry1> },
    ///             "attributes": 
    ///                 {
    ///                 "<field1>": <value11>,
    ///                 "<field2>": <value12> 
    ///                 } 
    ///         },{
    ///             "geometry": { <geometry1> },
    ///             "attributes": 
    ///                 {
    ///                 "<field1>": <value11>,
    ///                 "<field2>": <value12> 
    ///                 } 
    ///         }
    ///     ]
    /// }
    /// </example>
    public class ArcGISFeatureSetDto
    {
        public string displayFieldName { get; set; }
        public Dictionary<string, string> fieldAliases { get; set; }
        public string geometryType { get; set; }
        //unused for now    public bool hasZ { get; set; }
        //unused for now    public bool hasM { get; set; }
        public dynamic spatialReference { get; set; }
        public List<dynamic> fields { get; set; }
        public List<dynamic> features { get; set; }
    }




}
