using YzProject.EventBus.Events;

namespace YzProject.WebAPI.IntegrationEvents.Events
{
    // 集成事件说明：事件是“过去发生的事情”，
    // 因此它的名称必须是集成事件是一个可能对其他微服务、限界上下文或外部系统产生副作用的事件。
    public record OrderStartedIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; init; }

        public OrderStartedIntegrationEvent(string userId)
            => UserId = userId;
    }
}
