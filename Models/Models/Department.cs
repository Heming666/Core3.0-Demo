using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Models
{
   public class Department
    {
        public Guid DeptId { get; set; }
        public string DeptName { get; set; }
        public string Location { get; set; }
        public int EmployeeCount { get; set; }
    }
}
