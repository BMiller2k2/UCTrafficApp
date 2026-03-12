using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCTrafficApp.Models
{
    internal class IssueDTO
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string IssueText { get; set; }
    }
}
