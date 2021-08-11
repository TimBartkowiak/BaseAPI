using Newtonsoft.Json;

namespace BaseAPI.Models
{
    public class UserModel : AbstractModel
    {
        public string Username { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public string Salt { get; set; }
    }
}