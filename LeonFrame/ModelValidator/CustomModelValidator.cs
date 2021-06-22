using Core.Bases.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace LeonFrameAPI.ModelValidator
{
    public class CustomModelValidator : IObjectModelValidator
    {
        public void Validate(ActionContext actionContext, ValidationStateDictionary validationState, string prefix, object model)
        {
            if (actionContext.HttpContext.Request.Method == "POST" && !actionContext.ModelState.IsValid)
            {
                var res = new StdResponse
                {
                    Success = false,
                    Message = ""
                };
                actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                actionContext.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(res), Encoding.UTF8);
            }
        }
    }
}
