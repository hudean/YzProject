using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YzProject.Domain.Entites;

namespace YzProject.Domain
{
    internal static class ModelBuilderExtensions
    {
        /// <summary>
        /// 追加全局查询过滤器
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static ModelBuilder AppendGlobalQueryFilter<TInterface>(this ModelBuilder modelBuilder, Expression<Func<TInterface, bool>> filter)
        {
            // get a list of entities without a baseType that implement the interface TInterface
            var entities = modelBuilder.Model.GetEntityTypes()
                  //.Where(e => e.BaseType is null && e.ClrType.GetInterface(typeof(TInterface).Name) is not null)
                .Where(e => e.ClrType.GetInterface(typeof(TInterface).Name) is not null)
                .Select(e => e.ClrType);

            foreach (var entity in entities)
            {
                var parameterType = Expression.Parameter(modelBuilder.Entity(entity).Metadata.ClrType);
                var filterBody = ReplacingExpressionVisitor.Replace(filter.Parameters.Single(), parameterType, filter.Body);

                // get the existing query filter
                //获取现有的查询过滤器
                if (modelBuilder.Entity(entity).Metadata.GetQueryFilter() is { } existingFilter)
                {
                    var existingFilterBody = ReplacingExpressionVisitor.Replace(existingFilter.Parameters.Single(), parameterType, existingFilter.Body);

                    // combine the existing query filter with the new query filter
                    filterBody = Expression.AndAlso(existingFilterBody, filterBody);
                }

                // apply the new query filter
                //应用新的查询过滤器
                modelBuilder.Entity(entity).HasQueryFilter(Expression.Lambda(filterBody, parameterType));
            }

            return modelBuilder;
        }

        public static void AddSoftDeleteQueryFilter(
        this IMutableEntityType entityData)
        {
            var methodToCall = typeof(ModelBuilderExtensions)
                .GetMethod(nameof(GetSoftDeleteFilter),
                    BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(entityData.ClrType);
            var filter = methodToCall.Invoke(null, new object[] { });
            entityData.SetQueryFilter((LambdaExpression)filter);
        }

        private static LambdaExpression GetSoftDeleteFilter<TEntity>()
            where TEntity : class, ISoftDelete
        {
            Expression<Func<TEntity, bool>> filter = x => !x.IsDeleted;
            return filter;
        }


        public static ModelBuilder AddSoftDelete(this ModelBuilder modelBuilder)
        {

            //首先获取所有实现ISoftDelete接口的实体
            var entities = modelBuilder.Model.GetEntityTypes().Where(p => typeof(ISoftDelete).IsAssignableFrom(p.ClrType));

            //循环遍历实体
            foreach (var entity in entities)
            {
                //如果实体存在IsDelete属性则获取，如果没有则先添加后获取 GetOrAddProperty属性只有ef core 2.2版本及以下有
                //entity.GetOrAddProperty("IsDelete", typeof(bool));               
                //entity.AddProperty("IsDelete", typeof(bool));
                //构建表达式树
                var parameter = Expression.Parameter(entity.ClrType);
                var propertyMethodInfo = typeof(EF).GetMethod("Property").MakeGenericMethod(typeof(bool));
                var isDeleteProperty = Expression.Call(propertyMethodInfo, parameter, Expression.Constant("IsDelete"));
                BinaryExpression compareExpression = Expression.MakeBinary(ExpressionType.Equal, isDeleteProperty, Expression.Constant(false));
                var lambdaExpression = Expression.Lambda(compareExpression, parameter);

                modelBuilder.Entity(entity.ClrType).HasQueryFilter(lambdaExpression);
            }

            return modelBuilder;
        }
       
    }
}
