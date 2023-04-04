﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IPlatformRepository Platform { get; }
        IGenreRepository Genre { get; }

        void Save();
        Task SaveAsync();
    }
}
