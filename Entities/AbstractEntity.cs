using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseAPI.Entities
{
    public abstract class AbstractEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("id")]
        public Guid? Id { get; set; }

        [Column("datecreated")]
        [Required]
        public DateTimeOffset DateCreated { get; set; }

        [Column("datemodified")]
        public DateTimeOffset DateModified { get; set; }
    }
}