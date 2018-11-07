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
        public User(string username, int password)
        {
            Username = username;
            Password = password;
            PasswordHistory = new Dictionary<int, int>();
            PasswordHistory.Add(password, 1);
            PasswordInitialized = DateTime.Now;
            //period vazenja
            //sadasnje vreme
        }
        [DataMember]
        public String Username { get; set; }
        [DataMember]
        public int Password { get; set; }
        [DataMember]
        public Dictionary<int, int> PasswordHistory { get; set; }
        [DataMember]
        public DateTime PasswordInitialized { get; set; }
    }
}
