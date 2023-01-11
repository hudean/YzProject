﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.EventBus
{
    public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        /// <summary>
        /// 订阅信息
        /// </summary>
        public class SubscriptionInfo
        {
            public bool IsDynamic { get; }
            public Type HandlerType { get; }

            private SubscriptionInfo(bool isDynamic, Type handlerType)
            {
                IsDynamic = isDynamic;
                HandlerType = handlerType;
            }

            //public static SubscriptionInfo Dynamic(Type handlerType)
            //{
            //    return new SubscriptionInfo(true, handlerType);
            //}
            //public static SubscriptionInfo Typed(Type handlerType)
            //{
            //    return new SubscriptionInfo(false, handlerType);
            //}
            public static SubscriptionInfo Dynamic(Type handlerType) =>
              new SubscriptionInfo(true, handlerType);

            public static SubscriptionInfo Typed(Type handlerType) =>
                new SubscriptionInfo(false, handlerType);
        }
    }
}
