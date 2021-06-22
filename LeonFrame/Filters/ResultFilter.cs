using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Core.Bases.Response;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace LeonFrameAPI.Filters
{
    public class ResultFilter : Attribute, IResultFilter
    {
        ILogger<ResultFilter> _logger;
        //IWebHostEnvironment _env;
        bool _isDebug = false;

        public ResultFilter(ILogger<ResultFilter> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            //_env = env;
            _isDebug = env.IsDevelopment();
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            if (_isDebug)
                _logger.LogInformation("Filter -> OnResultExecuted...");
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (_isDebug)
                _logger.LogInformation("Filter -> OnResultExecuting...");

            if (context.Result is EmptyResult)
            {
                context.Result = new OkResult();
            }
            else
            {
                StdResponse stdResponse = new StdResponse
                {
                    Data = ((ObjectResult)context.Result).Value
                };

                context.Result = new JsonResult(stdResponse);
            }
        }
    }
}
