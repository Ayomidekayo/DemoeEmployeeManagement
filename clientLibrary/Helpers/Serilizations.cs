
using System.Runtime.Serialization;
using System.Text.Json;

namespace clientLibrary.Helpers
{
    public static class Serilizations
    {
        public static  string SerializeObj<T>(T modelObject)=> JsonSerializer.Serialize(modelObject);
        public static T DeserilizeJsonString<T>(string jsonString) => JsonSerializer.Deserialize<T>(jsonString);
        public static IList<T> DeserializeJsonstringList<T>(string jsonString)=>JsonSerializer.Deserialize<IList>()

    }
}
