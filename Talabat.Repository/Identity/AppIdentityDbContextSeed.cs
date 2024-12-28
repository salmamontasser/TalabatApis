using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
	public static class AppIdentityDbContextSeed
	{
		public static async Task SeedUserAsync(UserManager<AddUser> userManager)
		{
			if(!userManager.Users.Any())
			{
				var User = new AddUser()
				{
					DisplayName = "Salma Montasser",
					Email = "salmamontasser@gmail.com",
					UserName = "salmamontasser",
					PhoneNumber = "0124596378"
				};
				await userManager.CreateAsync(User, "Pa$$w0rd");

			}



		}
	}
}
