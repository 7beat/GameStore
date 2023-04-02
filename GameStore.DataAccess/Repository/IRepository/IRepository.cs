﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        T GetFirstOrDefault(Expression<Func<T, bool>> predicate, params string[] includeProperties);
        IEnumerable<T> GetAll(params string[] includeProperties);
        void Add(T entity);
        void Remove(T entity);
    }
}
