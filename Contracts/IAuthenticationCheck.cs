﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
	[ServiceContract]
	public interface IAuthenticationCheck
	{
		[OperationContract]
		[FaultContract(typeof(SecurityException))]
		bool IsAuthenticated(string username);
		
	}
}
