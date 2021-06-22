using Domain.IntegrationEvents;
using Infrastructure.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/12/7 16:20:11
*描述：
*
***********************************************************/
namespace Application.Behaviors
{
    /// <summary>
    /// 管道-事务管理
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private LContext _dbContext;
        private ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
        private IIntegrationEventService _IntegrationEventService;

        public TransactionBehavior(LContext dbContext
            , ILogger<TransactionBehavior<TRequest, TResponse>> logger
            , IIntegrationEventService integrationEventService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(LContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger<TransactionBehavior<TRequest, TResponse>>));
            _IntegrationEventService = integrationEventService ?? throw new ArgumentNullException(nameof(integrationEventService));
        }

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <param name="next">委托</param>
        /// <returns></returns>
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            string requestTypeName = request.GetType().Name;
            TResponse response = default(TResponse);

            try
            {
                if (_dbContext.HasActiveTransaction)
                    return await next();

                //执行策略
                IExecutionStrategy executionStrategy = _dbContext.Database.CreateExecutionStrategy();
                //手动执行
                await executionStrategy.ExecuteAsync(async () =>
                {
                    Guid transactionID;

                    //启动事务
                    using (var transaction = await _dbContext.BeginTransactionAsync())
                    {
                        transactionID = transaction.TransactionId;

                        _logger.LogInformation($"--- Begin Transaction {transactionID} for {requestTypeName} ({request})");

                        response = await next();

                        _logger.LogInformation($"--- End Transaction {transactionID} for {requestTypeName}");

                        await _dbContext.CommitTransactionAsync(transaction);

                        transactionID = transaction.TransactionId;
                    }

                    //触发集成事件
                    await _IntegrationEventService.PublishIntegrationEventAsync(transactionID);
                });

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError($"处理事务发生错误：RequestType:{requestTypeName},Command:{request}");

                throw;
            }
        }
    }
}
