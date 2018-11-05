using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Contracts
{
    [DataContract]
    public class User
    {
        [DataMember]
        public String Username { get; set; }
        [DataMember]
        public SecureString Password {get; set;}
    }
}
