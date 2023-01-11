using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.ResultModel
{
    /// <summary>
    /// 分页列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedList<T>
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// 总条数
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// 列表泛型实体
        /// </summary>
        public List<T> Items { get; set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            PageSize = pageSize;
            Items = items ?? new List<T>();
        }

        #region

        ///// <summary>
        ///// 是否有上一页
        ///// </summary>
        //public bool HasPreviousPage
        //{
        //    get
        //    {
        //        return (PageIndex > 1);
        //    }
        //}

        ///// <summary>
        ///// 是否有下一页
        ///// </summary>
        //public bool HasNextPage
        //{
        //    get
        //    {
        //        return (PageIndex < TotalPages);
        //    }
        //}


        #endregion


    }
}
