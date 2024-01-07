using GameScrypt.GSUtils;
using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace GameScrypt.JsonData {
    public static class GSJsonService {
        public static string GetJsonFromFile(string assetsFilePath) {
            var filepath = GSStreamingAssets.GetStreamingAssetsFilePath(assetsFilePath, true);
            return ReadJsonFromFilePath(filepath);
        }

        public static string ReadJsonFromFilePath(string filepath) {
            try {
                using (StreamReader reader = new StreamReader(filepath)) {
                    var playerJson = reader.ReadToEnd();
                    reader.Close();
                    return playerJson;
                }
            } catch (Exception e) {
                Debug.LogWarning("No json at location, attempting to create. " + e.Message);
                return null;
            }
        }

        public static void WriteJsonToFile(string assetsFilePath, object obj) {
            var filepath = GSStreamingAssets.GetStreamingAssetsFilePath(assetsFilePath, true);
            using (StreamWriter writer = new StreamWriter(filepath)) {
                var jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented);
                GSJsonService.writeJson(writer, jsonString);
            }
        }

        public static string FormatJson(string jsonString) {
            jsonString = jsonString.Replace(",\"", @",
""");
            jsonString = jsonString.Replace(":{", @":{
");
            jsonString = jsonString.Replace("}}", @"}
}");
            return jsonString;
        }

        private static void writeJson(StreamWriter writer, string jsonString) {
            jsonString = GSJsonService.FormatJson(jsonString);
            writer.Write(jsonString);
            writer.Close();
        }
    }
}
