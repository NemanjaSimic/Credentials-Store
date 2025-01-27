﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Contracts
{
    [DataContract]
    public class User
    {
		public User(){	}
        public User(string username, int password)
        {
            Username = username;
            Password = password;
			PasswordInitialized = DateTime.Now;
        }
        [DataMember]
		[Key]
        public string Username { get; set; }
        [DataMember]
        public int Password { get; set; }
   
		[DataMember]
        public DateTime PasswordInitialized { get; set; }
    }
}
