using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyGIS.FeatureCollector
{
    public static class Extensions
    {

        // HT: http://stackoverflow.com/a/24087164
        /// <summary>
        /// Helper methods for the lists.
        /// </summary>
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }


        public static string AsValidFilename(this string str)
        {
            return Path.GetInvalidFileNameChars().Aggregate(str, (current, c) => current.Replace(c.ToString(), string.Empty));
        }



    }
}
