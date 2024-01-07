using System;
using Newtonsoft.Json;
using UnityEngine;

namespace GameScrypt.JsonData {
    public class GSJsonData : IJSONDatabase {
        protected string _path = "";

        public GSJsonData(string path) {
            _path = path;
        }

        public virtual void Recreate(string path = null, object obj = null) {
            throw new Exception("Extend in orderd to use this method.");
        }

        public virtual T GetJsonObj<T>() {
            var actualJson = GSJsonService.GetJsonFromFile(_path);
            if (actualJson != null) {
                return JsonConvert.DeserializeObject<T>(actualJson);
            }
            return default(T);
        }

        public virtual JSONNode GetJson() {
            var actualJson = GSJsonService.GetJsonFromFile(_path);
            return GSJsonData.GetParsedJson(actualJson);
        }

        public static JSONNode GetParsedJson(string actualJson) {
            Debug.Log(actualJson);
            return JSON.Parse(actualJson);
        }
    }

    public interface IJSONDatabase {
        public void Recreate(string path = null, object obj = null);
        public T GetJsonObj<T>();
    }
}