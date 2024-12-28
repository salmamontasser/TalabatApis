using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
	
	public class UnitOfWork : IUnitOfWork
	{
		private readonly StoreContext _dbContext;
		private Hashtable _repositoires;
        public UnitOfWork(StoreContext dbContext)
        {
            _repositoires = new Hashtable();
			_dbContext = dbContext;
		}
        public async Task<int> CompleteAsync()
		{
			return await _dbContext.SaveChangesAsync();
		}

		public async ValueTask DisposeAsync()
		{
			 await _dbContext.DisposeAsync();
		}

		public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
		{
			var type=typeof(TEntity).Name;
			if(!_repositoires.ContainsKey(type))
			{
				var Repository= new GenericRepository<TEntity>(_dbContext);
				_repositoires.Add(type, Repository);
			}
				return _repositoires[type] as IGenericRepository<TEntity> ;
		}
	}
}
