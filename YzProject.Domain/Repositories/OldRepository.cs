using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YzProject.Domain.Entites;

namespace YzProject.Domain.Repositories
{
    public class OldRepository<TEntity> : IOldRepository<TEntity> where TEntity : Entity, IEntity
    {
        protected YzProjectContext _dbContext;
        protected DbSet<TEntity> _dbSet;
        public OldRepository(YzProjectContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        /// <summary>
        /// 提交修改
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        /// <summary>
        /// 异步提交修改
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="orderBy">排序规则</param>
        /// <param name="includeProperties">包含属性</param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }

        public List<TEntity> QueryAll()
        {
            try
            {
                return _dbSet.AsQueryable<TEntity>().ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        /// <summary>
        /// 根据主键ID获取数据实体
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public virtual TEntity GetById(object id)
        {
            return _dbSet.Find(id);
        }

        /// <summary>
        /// 异步根据主键ID获取数据实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TEntity> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="isSave">是否执行</param>
        /// <returns></returns>
        public virtual TEntity Add(TEntity entity)
        {
            _dbSet.Add(entity);
            var res = SaveChanges();
            return entity;
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            var res = await SaveChangesAsync();
            return entity;
        }

        public virtual int Delete(object id)
        {
            var item = _dbSet.Find(id);
            if (item == null)
            {
                return 0;
            }
            else
            {
                _dbSet.Remove(item);
                return SaveChanges();
            }
        }

        public virtual async Task<int> DeleteAsync(object id)
        {
            int de = 0;
            //执行查询
            var todoItem = await _dbSet.FindAsync(id);
            if (todoItem == null)
            {
                de = 0;
            }
            else
            {
                _dbSet.Remove(todoItem);
                de = await SaveChangesAsync();
            }
            return de;
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
            SaveChanges();
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> @where)
        {
            TEntity[] entitys = this._dbSet.Where(@where).ToArray();
            if (entitys.Length > 0)
            {
                this._dbSet.RemoveRange(entitys);
            }
            SaveChanges();
        }

        public virtual int Update(TEntity entity)
        {
            var entry = this._dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                entry.State = EntityState.Modified;
            }
            var da = SaveChanges();
            return da;
        }

        public async Task<int> UpdateAsync(TEntity entity)
        {
            var entry = this._dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                entry.State = EntityState.Modified;
            }
            var da = await Task.Run(SaveChangesAsync);
            return da;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }


        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

    }

}
