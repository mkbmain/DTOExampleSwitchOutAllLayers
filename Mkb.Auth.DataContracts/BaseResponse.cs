using System.Text.Json.Serialization;

namespace Mkb.Auth.Contracts
{
    public abstract class BaseResponse
    {
        [JsonIgnore]
        public ResponseType ResponseType { get; set; }

        public string Message { get; set; }
    }
}