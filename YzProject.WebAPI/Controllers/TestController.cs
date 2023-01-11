using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula;
using System;
using System.Threading.Tasks;
using YzProject.Domain.RequestModel;
using YzProject.EventBus.Abstractions;
using YzProject.Redis;
using YzProject.WebAPI.IntegrationEvents.Events;

namespace YzProject.WebAPI.Controllers
{
    /// <summary>
    /// 功能测试控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IStringLocalizer<TestController> _loc;
        private readonly IRedisCacheRepository _redisCacheRepository;
        private readonly IEventBus _eventBus;

        /// <summary>
        /// ILogger 和 IStringLocalizer,IRedisCacheRepository, IEventBus 实例的构造函数注入
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="loc"></param>
        /// <param name="redisCacheRepository"></param>
        /// <param name="eventBus"></param>
        public TestController(ILogger<TestController> logger,
            IStringLocalizer<TestController> loc,
            IRedisCacheRepository redisCacheRepository,
            IEventBus eventBus)
        {
            _logger = logger;
            _loc = loc;
            _redisCacheRepository = redisCacheRepository;
            _eventBus = eventBus;
        }

        #region redis功能的测试

        [HttpGet("HashGetString")]
        public async Task<IActionResult> HashGetStringAsync(string key)
        {
            var value = await  _redisCacheRepository.HashGetStringAsync("HashKey", key);
            return Ok(value);
        }

        [HttpPost("HashGetAll")]
        public async Task<IActionResult> HashGetAllAsync()
        {
            var res = await _redisCacheRepository.HashGetAllAsync<string>("HashKey");
            return Ok(res);
        }

        [HttpPost("HashSet")]
        public async Task<IActionResult> HashSetAsync(string value)
        {
            var isTrue = await _redisCacheRepository.HashSetAsync("HashKey","hda", value);
            return Ok(isTrue);
        }

       

        [HttpPost("HashDelete")]
        public async Task<IActionResult> HashDeleteAsync(string key)
        {
            var isTrue = await _redisCacheRepository.HashDeleteAsync("HashKey", key);
            return Ok(isTrue);
        }


        [HttpPost("HashExists")]
        public async Task<IActionResult> HashExistsAsync(string key)
        {
            var isTrue = await _redisCacheRepository.HashExistsAsync("HashKey", key);
            return Ok(isTrue);
        }

        #endregion

        #region 事件总线功能的测试

        [HttpPost("PublishEventMessage")]
        public IActionResult PublishEventMessage()
        {
            string userid = Guid.NewGuid().ToString();
            var eventMessage = new OrderStartedIntegrationEvent(userid);
            try
            {
                _eventBus.Publish(eventMessage);
                //发布成功
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName}", eventMessage.Id, Program.AppName);

                //throw;
            }

            return Accepted();
        }

        #endregion

        #region 多语言/全球化

        /// <summary>
        /// 这是一个简单的测试，我们尝试将键“hi”的本地化版本打印到控制台上，并将其作为响应返回。
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation(_loc["hi"]);
            var message = _loc["hi"].ToString();
            return Ok(message);
        }

        /// <summary>
        /// 在这里，我们将一个随机名称传递给该端点，并且应用程序应该返回一个本地化版本的“欢迎 xxxx，你好吗？”。就如此容易。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{name}")]
        public IActionResult Get(string name)
        {
            var message = string.Format(_loc["welcome"], name);
            return Ok(message);
        }

        /// <summary>
        /// 理想情况下，此方法将返回在相应 JSON 文件中找到的所有键和值。
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        //[Authorize]
        public IActionResult GetAll()
        {
            var message = _loc.GetAllStrings();
            return Ok(message);
        }

        #endregion

        #region 模型验证

        [HttpPost("ModelValidation")]
        public async Task<IActionResult> ModelValidation(ParamLogin param)
        {
            await Task.CompletedTask;
            return Content("ok");
        }

        #endregion


        #region

        //[ServiceFilter(typeof(Filters.AuthorizationFilter))]
        //[HttpGet("")]
        //public async Task<IActionResult> GetActionResultAsync()
        //{
        //    return Ok();
        //}

        #endregion
    }
}
