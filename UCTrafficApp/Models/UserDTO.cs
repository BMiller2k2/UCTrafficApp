using SQLite;

namespace UCTrafficApp.Models
{
    public class UserDto
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserPass { get; set; }
    }
}