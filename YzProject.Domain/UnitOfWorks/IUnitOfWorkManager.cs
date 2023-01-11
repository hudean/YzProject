using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.UnitOfWorks
{
    /// <summary>
    /// 工作单元管理器接口
    /// </summary>
    public interface IUnitOfWorkManager
    {
        IUnitOfWork this[string dataBaseName] { get; }

        /// <summary>
        /// 尝试添加工作单元
        /// </summary>
        /// <param name="dataBaseName"></param>
        /// <param name="unitOfWork"></param>
        void TryAddUnitOfWork(string dataBaseName, IUnitOfWork unitOfWork);

        /// <summary>
        /// 尝试获取工作单元
        /// </summary>
        /// <param name="dataBaseName"></param>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        bool TryGetUnitOfWork(string dataBaseName, out IUnitOfWork unitOfWork);

    }
}
