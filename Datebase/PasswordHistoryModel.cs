using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datebase
{
	public class PasswordHistoryModel
	{
		private static int id = 0;
		public PasswordHistoryModel()
		{
			Id = id++;
		}
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public string Username { get; set; }
		public int Password { get; set; }
	}
}
