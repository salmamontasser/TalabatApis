using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
	{
		private readonly StoreContext _dbContext;

		public GenericRepository(StoreContext dbContext)
        {
			_dbContext = dbContext;
		}
        public async Task<IReadOnlyList<T>> GetAllAsync()
		=> await _dbContext.Set<T>().ToListAsync();

		

		public async Task<T> GetByIdAsync(int id)
		{
			return await _dbContext.Set<T>().FindAsync(id);


		}


		public async Task<IReadOnlyList<T>> GetAllSpecAsync(ISpecifications<T> Spec)
		{
			return await ApplySpecification(Spec).ToListAsync();
		}

		public async Task<T> GetEntitySpecAsync(ISpecifications<T> Spec)
		{
			return await ApplySpecification(Spec).FirstOrDefaultAsync(); 
		}
		private IQueryable<T> ApplySpecification(ISpecifications<T> Spec) 
		{

			return SpecificationEnvalutor<T>.GetQuery(_dbContext.Set<T>(), Spec);
		}

		public async Task<int> GetCountWithSpecAsync(ISpecifications<T> Spec)
		{
			return await ApplySpecification(Spec).CountAsync();
		}

		public async Task AddAsync(T item)
		{
			await _dbContext.Set<T>().AddAsync(item);
		}

		public void Update(T item)
		{
			_dbContext.Set<T>().Update(item);
		}

		public void Delete(T item)
		{
			_dbContext.Set<T>().Remove(item);
		}
	}
}
