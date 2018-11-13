using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
	[DataContract]
	public class CredentialsException
	{
		private string reason;
		[DataMember]
		public string Reason
		{
			get { return reason; }
			set { reason = value; }
		}
	}
}
