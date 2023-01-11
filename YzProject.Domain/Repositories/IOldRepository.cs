using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YzProject.Domain.Entites;

namespace YzProject.Domain.Repositories
{
    public interface IOldRepository
    {

    }

    public interface IOldRepository<T> : IOldRepository, IDisposable where T : Entity, IEntity
    {

        /// <summary>
        /// 提交当前单元操作的更改
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// 异步提交当前单元操作的更改
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="orderBy">排序方式</param>
        /// <param name="includeProperties">包含属性</param>
        /// <returns></returns>
        IQueryable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");

        List<T> QueryAll();

        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById(object id);

        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetByIdAsync(object id);

        /// <summary>
        /// 新增 - 通过实体对象添加
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        T Add(T entity);

        /// <summary>
        /// 异步新增 - 通过实体对象添加
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// 删除 - 通过主键ID删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int Delete(object id);

        /// <summary>
        /// 异步删除 - 通过主键ID删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(object id);

        /// <summary>
        /// 删除 - 通过实体对象删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        void Delete(T entity);

        /// <summary>
        /// 批量删除 - 通过条件删除
        /// </summary>
        /// <param name="where">过滤条件</param>
        void Delete(Expression<Func<T, bool>> @where);

        /// <summary>
        /// 更新 - 通过实体对象修改
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        int Update(T entity);

        /// <summary>
        /// 异步更新 - 通过实体对象修改
        /// </summary>
        /// <param name="entity">实体对象</param>
        Task<int> UpdateAsync(T entity);

        bool Any(Expression<Func<T, bool>> predicate);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    }

}
