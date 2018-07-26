using System;
using System.Linq;
using System.Linq.Expressions;

namespace wyspaBotWebApp.Core {
    public interface IRepository<T> : IDisposable {
        T Get(object id);
        void Update(T obj);
        void Save(T obj);
        void Delete(T obj);
        void DeleteAll();
        IQueryable<T> GetAll();
        IQueryable<T> Filter<T>(Expression<Func<T, bool>> func) where T : class;
    }
}