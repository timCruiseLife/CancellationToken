using System.Data;
using Example.Logging;
using Example.Models;
using Example.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Example.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageBoardsController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly ExamplesContext context;

        public MessageBoardsController(
            ILogger<MessageBoardsController> logger,
            ExamplesContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<WebApiResult<List<MessageBoard>>>> Get([FromQuery] string? name = null)
        {
            var callContext = this.GetCallContext();
            using OperationMonitor operationMonitor = new OperationMonitor(this.logger, AppEvent.IncomingRequest, nameof(Get), callContext.TraceActivityId);
            var query = this.context.MessageBoards.AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }

            query.OrderByDescending(x => x.CreateTime);
            var existings = await query.ToListAsync(callContext.CancellationToken);
            return this.Ok(new WebApiResult<List<MessageBoard>>(existings));
        }

        [HttpGet("{id}")]
        [ResponseCache(VaryByHeader = "User-Agent", NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<WebApiResult<MessageBoard>>> GetOne([FromRoute] long id)
        {
            var callContext = this.GetCallContext();
            using OperationMonitor operationMonitor = new OperationMonitor(this.logger, AppEvent.IncomingRequest, nameof(GetOne), callContext.TraceActivityId);
            var existings = await this.context.MessageBoards.Where(x => x.Id == id).FirstOrDefaultAsync(callContext.CancellationToken);
            if (existings == null)
            {
                return this.NotFound(this.CreateErrorResult<MessageBoard>(ExampleConstants.WebApiErrors.ObjectNotFound, "message boards not found"));
            }

            return this.Ok(new WebApiResult<MessageBoard>(existings));
        }

        [HttpPost]
        public async Task<ActionResult<WebApiResult<MessageBoard>>> Post([FromBody] MessageBoard request)
        {
            (var isValid, string reason) = request.IsValid();
            if (!isValid)
            {
                return this.BadRequest(this.CreateErrorResult<MessageBoard>(ExampleConstants.WebApiErrors.InvalidData, reason));
            }

            var callContext = this.GetCallContext();
            using var monitor = new OperationMonitor(this.logger, AppEvent.IncomingRequest, nameof(Post), callContext.TraceActivityId);
            var conflict = await this.context.MessageBoards.Where(x => x.Name == request.Name).FirstOrDefaultAsync(callContext.CancellationToken);
            if (conflict != null)
            {
                return this.Conflict(this.CreateErrorResult<MessageBoard>(ExampleConstants.WebApiErrors.ObjectConflict, "message boards name already exists"));
            }

            request.CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            await this.context.MessageBoards.AddAsync(request, callContext.CancellationToken);
            await this.context.SaveChangesAsync(callContext.CancellationToken);
            return this.Ok(new WebApiResult<MessageBoard>(request));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<WebApiResult<MessageBoard>>> Put(
            [FromRoute] long id,
            [FromBody] MessageBoard request)
        {
            var callContext = this.GetCallContext();
            using var monitor = new OperationMonitor(this.logger, AppEvent.IncomingRequest, nameof(Put), callContext.TraceActivityId);
            var existing = await this.context.MessageBoards.Where(x => x.Id == id).FirstOrDefaultAsync(callContext.CancellationToken);
            if (existing == null)
            {
                return this.NotFound(this.CreateErrorResult<MessageBoard>(ExampleConstants.WebApiErrors.ObjectNotFound, "message boards not found"));
            }

            if (!string.IsNullOrEmpty(request.Content) &&
                existing.Content != request.Content)
            {
                existing.Content = request.Content;
            }

            existing.UpdateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            await this.context.SaveChangesAsync(callContext.CancellationToken);
            return this.Ok(new WebApiResult<MessageBoard>(existing));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<WebApiResult<MessageBoard>>> Delete([FromRoute] long id)
        {
            var callContext = this.GetCallContext();
            using OperationMonitor operationMonitor = new OperationMonitor(this.logger, AppEvent.IncomingRequest, nameof(Delete), callContext.TraceActivityId);
            var existing = await this.context.MessageBoards.Where(x => x.Id == id).FirstOrDefaultAsync(callContext.CancellationToken);
            if (existing == null)
            {
                return this.NotFound(this.CreateErrorResult<MessageBoard>(ExampleConstants.WebApiErrors.ObjectNotFound, "message boards not found"));
            }

            this.context.MessageBoards.Remove(existing);
            await this.context.SaveChangesAsync(callContext.CancellationToken);
            return this.NoContent();
        }
    }
}