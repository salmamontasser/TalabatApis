using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order_Aggregation
{
	public class Address
	{
        
        public Address(string fristName, string lastName, string city, string country, string street)
		{
			FristName = fristName;
			LastName = lastName;
			City = city;
			Country = country;
			Strret = street;
		}

		public string FristName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Strret { get; set; }
        public Address()
        {
            
        }
    }
}
