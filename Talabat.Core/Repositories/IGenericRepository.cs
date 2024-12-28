﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Repositories
{
	public interface IGenericRepository<T> where T : BaseEntity
	{
		Task<IReadOnlyList<T>> GetAllAsync();

		Task<T> GetByIdAsync(int id);


		Task<IReadOnlyList<T>> GetAllSpecAsync(ISpecifications<T> Spec);

		Task<T> GetEntitySpecAsync(ISpecifications<T> Spec);
		Task<int> GetCountWithSpecAsync(ISpecifications<T> Spec);

		Task AddAsync(T item);
		void Update(T item);
		void Delete(T item);

	}
}
