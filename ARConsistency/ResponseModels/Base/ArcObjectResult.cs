using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace ARConsistency.ResponseModels.Base
{
    public abstract class ArcObjectResult : ActionResult
    {
        private protected int StatusCode { get; set; }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            IActionResultExecutor<ObjectResult> executor = context.HttpContext.RequestServices.GetRequiredService<IActionResultExecutor<ObjectResult>>();

            context.HttpContext.Response.StatusCode = StatusCode;
            return executor.ExecuteAsync(context, new ObjectResult(this));
        }
    }
}
