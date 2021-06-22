using Core.Bases.Response;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace LeonFrameAPI.Filters
{
    /// <summary>
    /// 请求模型验证类(异步)
    /// </summary>
    public class ViewModelRequestValidationFilter : IAsyncActionFilter
    {
        private IValidator[] _validators;

        public ViewModelRequestValidationFilter(IValidator[] validators)
        {
            _validators = validators;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Method.ToUpper() == "POST")
            {
                //首先在中间件中启用EnableBuffering()
                //位置置为0
                context.HttpContext.Request.Body.Position = 0;

                #region 这种读取方式会让后面的代码不能读取（Action的模型绑定不受影响）
                //using (var reader = new StreamReader(context.HttpContext.Request.Body ))
                //{
                //    var a = await reader.ReadToEndAsync();
                //}
                #endregion

                using (var ms = new MemoryStream())
                {
                    context.HttpContext.Request.Body.CopyTo(ms);
                    var b = ms.ToArray();
                    var bodyStr = Encoding.UTF8.GetString(b); //把body赋值给bodyStr
                    if (!string.IsNullOrWhiteSpace(bodyStr))
                    {
                        var pros = context.ActionDescriptor.BoundProperties;
                        if (pros != null && pros.Count > 0)
                        {
                            Type entityType = pros[0].ParameterType;
                            JsonConvert.DeserializeObject(bodyStr, entityType);
                            //_validators.Select(r=>r.Validate()  )
                        }
                    }
                    else
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                        var res = new StdResponse
                        {
                            Success = false,
                            Message = "请求数据不能为空"
                        };
                        await context.HttpContext.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(res), Encoding.UTF8);
                    }
                }

                //位置置为0
                context.HttpContext.Request.Body.Position = 0;
            }

            await next();
        }
    }
}
