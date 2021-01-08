using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DBModel.Models
{
   public class Department
    {
        [Key]
        public Guid DeptId { get; set; }
        [Required(ErrorMessage = "请填写部门名称")]
        public string DeptName { get; set; }
        public string Location { get; set; }
        public int EmployeeCount { get; set; }
    }
}
