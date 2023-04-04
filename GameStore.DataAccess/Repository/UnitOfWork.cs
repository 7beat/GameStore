﻿using GameStore.DataAccess.Repository.IRepository;
using GameStoreWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public IPlatformRepository Platform { get; private set; }
        public IGenreRepository Genre { get; private set; }

        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Platform = new PlatformRepository(_db);
            Genre = new GenreRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges(); 
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
