using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DBModel.Models;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

namespace DBModel
{
    public class BaseRepository<T> where T : class, new()
    {
        private EFCoreDbContext db;
        public BaseRepository()
        {
            db = new EFCoreDbContext();
        }

        /// <summary>
        /// 查找单个实体
        /// </summary>
        /// <param name="key">主键</param>
        /// <returns></returns>
        public Task<T> Find(object key)
        {
            return Task.Run(() =>
             db.Find<T>(key)
            );
        }
        public Task<IEnumerable<T>> GetList()
        {
            return Task.Run(() =>
            db.Set<T>().AsEnumerable()
           ); 
        }
        public Task<IEnumerable<T>> GetList(Expression<Func<T, bool>> where)
        {
            return Task.Run(() =>
            db.Set<T>().Where(where).AsEnumerable()
           );
        }

        public Task<IEnumerable<T>> GetPagedList<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy, bool isAsc = true)
        {
            if (isAsc)
            {
                return Task.Run(() =>
                db.Set<T>().Where(whereLambda).OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable()
                );
            }
            else
            {
                return Task.Run(() =>
                db.Set<T>().Where(whereLambda).OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable()
                );
            }
        }

        public Task<bool> Delete(Expression<Func<T, bool>> where)
        {
            return Task.Run(() =>
             {
                 IEnumerable<T> listDeleting = db.Set<T>().Where(where).AsEnumerable();
                 db.Set<T>().RemoveRange(listDeleting);
                 bool success = db.SaveChanges() > 0;
                 return success;
             });
        }

        public Task<bool> Delete(object key)
        {
            return Task.Run(() =>
            {
                var entity = db.Set<T>().Find(key);
                db.Set<T>().Attach(entity);  //先附加到EF 容器
                db.Set<T>().Remove(entity); //标识为删除状态
                bool success = db.SaveChanges() > 0;
                return success;
            });
        }

        public Task<bool> Add(T Entity)
        {
            return Task.Run(() =>
            {
                db.Set<T>().Add(Entity);
                bool success = db.SaveChanges() > 0;
                return success;
            });
        }

        public Task<bool> Add(IEnumerable<T> Entity)
        {
            return Task.Run(() =>
            {
                db.Set<T>().AddRange(Entity);
                bool success = db.SaveChanges() > 0;
                return success;
            });
        }
        public Task<bool> Modify(T Entity)
        {
            return Task.Run(() =>
            {
                // 将对象添加到EF中
                var entry = db.Set<T>().Attach(Entity);
                //打标记
                entry.State = EntityState.Modified;
                var success = db.SaveChanges() > 0;
                return success;
            });
        }
        public Task<bool> Modify(T model, params string[] propertyNames)
        {

            return Task.Run(() =>
            {
                //将对象添加到EF中
                EntityEntry entry = db.Entry<T>(model);
                //先设置对象的包装状态为 Unchanged
                entry.State = EntityState.Unchanged;
                //循环被修改的属性名数组
                foreach (string propertyName in propertyNames)
                {
                    //将每个被修改的属性的状态设置为已修改状态；这样在后面生成的修改语句时，就只为标识为已修改的属性更新
                    entry.Property(propertyName).IsModified = true;
                }
                var success = db.SaveChanges() > 0;
                return success;
            });
        }
        public Task<bool> Modify(IEnumerable<T> modifyList)
        {
            return Task.Run(() =>
            {
                db.Set<T>().UpdateRange(modifyList);
                var success = db.SaveChanges() > 0;
                return success;
            });
        }
        public Task<int> Count(Expression<Func<T, bool>> whereLambda)
        {
            return Task.Run(() =>
            {
                int count = db.Set<T>().Count(whereLambda);
                return count;
            });
        }

    }
}
