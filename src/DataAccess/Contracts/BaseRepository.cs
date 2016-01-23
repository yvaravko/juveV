﻿using System.Collections.Generic;
using System.Linq;
using DataAccess.Domain;
using Microsoft.Data.Entity;

namespace DataAccess.Contracts
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntityWithId
    {
        private readonly JuveDbContext _context;

        public BaseRepository()
        {
            _context = new JuveDbContext();
        }

        public T GetById(int id)
        {
            return DbSet().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<T> GetList()
        {
            return DbSet().ToList();
        }

        public void Update(T entity)
        {
            if (IsDetached(entity))
            {
                DbSet().Attach(entity);
            }

            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _context.Entry(entity).State = EntityState.Deleted;
                _context.SaveChanges();
            }
        }

        public int Create(T entity)
        {
            var added = DbSet().Add(entity);
            _context.Entry(entity).State = EntityState.Added;
            _context.SaveChanges();
            return added.Entity.Id;
        }

        public DbSet<T> DbSet()
        {
            return _context.Set<T>();
        }

        protected bool IsDetached(T entity)
        {
            return _context.Entry(entity).State == EntityState.Detached;
        }
    }
}