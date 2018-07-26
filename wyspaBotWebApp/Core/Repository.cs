using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;

namespace wyspaBotWebApp.Core {
    public class Repository<T> : IRepository<T> {
        private readonly ISession session;

        public Repository(ISession session) {
            this.session = session;
        }

        public void Delete(T obj) {
            using (var transaction = this.session.BeginTransaction()) {
                this.session.Delete(obj);
                transaction.Commit();
            }
        }

        public void DeleteAll() {
            using (var transaction = this.session.BeginTransaction()) {
                var allItems = this.session.Query<T>();
                foreach (var item in allItems) {
                    this.session.Delete(item);
                }
                transaction.Commit();
            }
        }

        public void Dispose() {
        }

        public IQueryable<T> Filter<T>(Expression<Func<T, bool>> func) where T : class {
            IQueryable<T> filteredItems;
            using (var transaction = this.session.BeginTransaction()) {
                filteredItems = this.session.Query<T>().Where(func);
                transaction.Commit();
            }

            return filteredItems;
        }

        public T Get(object id) {
            T result;
            using (var transaction = this.session.BeginTransaction()) {
                result = this.session.Get<T>(id);
                transaction.Commit();
            }
            return result;
        }

        public IQueryable<T> GetAll() {
            IQueryable<T> allItems;
            using (var transaction = this.session.BeginTransaction()) {
                allItems = this.session.Query<T>();
                transaction.Commit();
            }
            return allItems;
        }

        public void Save(T obj) {
            using (var transaction = this.session.BeginTransaction()) {
                this.session.Save(obj);
                transaction.Commit();
            }
        }

        public void Update(T obj) {
        }
    }
}