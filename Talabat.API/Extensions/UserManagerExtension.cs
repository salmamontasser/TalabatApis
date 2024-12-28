using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talabat.Core.Entities.Identity;

namespace Talabat.API.Extensions
{
	public static class UserManagerExtension
	{
		public static async Task<AddUser?> FindUserWithAdderessAsync(this UserManager<AddUser> userManager,ClaimsPrincipal User)
		{
			var Email = User.FindFirstValue(ClaimTypes.Email);
			var user = await userManager.Users.Include(U => U.Address).FirstOrDefaultAsync(U=>U.Email==Email);

			return user;
		}
	}
}
