using System.ComponentModel.DataAnnotations.Schema;

namespace BaseAPI.Entities
{
    [Table("users")]
    public class UserEntity : AbstractEntity
    {
        [Column("username")]
        public string Username { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("salt")]
        public string Salt { get; set; }
    }
}