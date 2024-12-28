using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Identity
{
	public class Address
	{
        public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
        public string City { get; set; }
		public string Strret { get; set; }
		public string Country { get; set; }
        public string AddUserId { get; set; }
        public AddUser User { get; set; }

    }
}
