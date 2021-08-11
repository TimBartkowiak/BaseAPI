using System;
using System.ComponentModel.DataAnnotations.Schema;
using BaseAPI.Models;

namespace BaseAPI.Entities
{
    [Table("test")]
    public class TestEntity : AbstractEntity
    {
        [Column("number")]
        public int? Number { get; set; }
        [Column("data")]
        public string Data { get; set; }
        
    }
}