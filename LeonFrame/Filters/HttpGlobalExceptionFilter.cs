using Core.Bases.Response;
using Domain.Exceptions;
using Infrastructure.ActionResults;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeonFrameAPI.Filters
{
    /// <summary>
    /// 全局异常过滤器
    /// </summary>
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        ILogger<HttpGlobalExceptionFilter> _logger;
        IWebHostEnvironment _env;

        public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);

            if (context.Exception.GetType() == typeof(DomainException))
            {
                var problemDetails = new ValidationProblemDetails()
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "请参考异常属性获取更多信息"
                };

                problemDetails.Errors.Add("DomainValidations", new string[] { context.Exception.Message.ToString() });

                var response = new StdResponse
                {
                    Success = false,
                    Message = context.Exception.Message,
                    Data = problemDetails
                };

                //context.Result = new BadRequestObjectResult(problemDetails);
                context.Result = new BadRequestObjectResult(response);
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            else
            {
                var json = new JsonErrorResponse {
                    Messages = new[] { "发生错误，请重试"}
                };

                if (_env.IsDevelopment())
                {
                    json.DeveloperMessage = context.Exception;
                }

                //context.Result = new InternalServerErrorObjectResult(json);
                var response = new StdResponse
                {
                    Success = false,
                    Message = context.Exception.Message,
                    Data = json
                };

                context.Result = new InternalServerErrorObjectResult(response);
                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
             
            context.ExceptionHandled = true;
        }

        private class JsonErrorResponse
        {
            public string[] Messages { get; set; }

            public object DeveloperMessage { get; set; }
        }
    }
}
