using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DBModel.Models
{
	public class UserInfo
	{
		public Guid Id { get; set; }
		[Required(ErrorMessage = "不能为空")]
		public string Name { get; set; }
		public Sex Sex { get; set; }
		public string Address { get; set; }
		public int? Age { get; set; }
		public Guid DeptId { get; set; }
	}

	public enum Sex {
		男,
		女
	}
}
