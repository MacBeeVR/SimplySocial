using System;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SimplySocial.Server.Core.Extensions
{
    public static class TempDataExtensions
    {
        /// <summary>
        /// Serializes a given complex value into a JSON string and stores it
        /// in the TempDataDictionary with a specified key.
        /// </summary>
        /// <typeparam name="T">Type to serialize</typeparam>
        /// <param name="tempData"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set<T>(this ITempDataDictionary tempData, String key, T value)
            where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Deserializes a JSON String from TempData with a given key and returns
        /// the deserialized object. 
        /// </summary>
        /// <typeparam name="T">Type to deserialize the stored item to</typeparam>
        /// <param name="tempData"></param>
        /// <param name="key"></param>
        /// <returns>
        /// A deserialized object if one with the provided key exists. null otherwise.
        /// </returns>
        public static T Get<T>(this ITempDataDictionary tempData, String key)
            where T : class
        {
            // If a value exists for the given key, treat it as a json string and return
            // the deserialized generic typed object. Otherwise null.
            Object tempObj;
            return tempData.TryGetValue(key, out tempObj)
                ? JsonConvert.DeserializeObject<T>((String)tempObj)
                : null;
        }
    }
}
