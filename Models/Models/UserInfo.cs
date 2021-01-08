using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Models
{
	public class UserInfo
	{
		public Guid Id { get; set; }
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
