using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Core.Bases.Response;
using Domain.Exceptions;

namespace Application.Behaviors
{
    /// <summary>
    /// 验证command
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class ValidateBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private IValidator<TRequest>[] _validators;
        private ILogger<ValidateBehavior<TRequest, TResponse>> _logger;
        private bool _isDebug;

        public ValidateBehavior(IValidator<TRequest>[] validators, ILogger<ValidateBehavior<TRequest, TResponse>> logger, IWebHostEnvironment env)
        {
            _validators = validators;
            _logger = logger;
            _isDebug = env.IsDevelopment();
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var requestType = typeof(TRequest).Name;

            if (_isDebug)
                _logger.LogInformation("MediatR -> Pipeline " + typeof(ValidateBehavior<,>).Name);

            _logger.LogInformation("正在验证" + requestType + "类型请求");

            var errors = _validators.Select(r => r.Validate(request))
                  .SelectMany(r => r.Errors)
                  .Where(error => error != null)
                  .ToList();

            if (errors.Any())
            {
                throw new DomainException(string.Join(';', errors.Select(r => r.ErrorMessage).ToArray()));
            }

            return await next();
        }
    }
}
