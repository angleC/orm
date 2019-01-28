using MicroDust.Attribute;
using System;

namespace ORM.MicroDust.Test.Console.Model
{
    [Table("User")]
    public class UserModel
    {
        [Key]
        [Column("ID")]
        public string IDKey { get; set; }
        public string Name { get; set; }
        public bool Sex { get; set; }
        public int Status { get; set; }
        public int Age { get; set; }
        public DateTime? DtCreate { get; set; }
    }
}
