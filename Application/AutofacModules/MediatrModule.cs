using Application.Behaviors;
using Autofac;
using MediatR;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Application.AutofacModules
{
    /// <summary>
    /// MediatR相关注册模块
    /// </summary>
    public class MediatrModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //注册命令与处理器
            builder.RegisterAssemblyTypes(Assembly.Load("Application"))
                .AsClosedTypesOf(typeof(IRequestHandler<,>))
                .InstancePerDependency();

            //注册领域事件与处理器
            builder.RegisterAssemblyTypes(Assembly.Load("Application"))
                .AsClosedTypesOf(typeof(INotificationHandler<>))
                .InstancePerDependency();

            //管道-事务
            builder.RegisterGeneric(typeof(ValidateBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(TransactionBehavior<,>)).As(typeof(IPipelineBehavior<,>));

            base.Load(builder);
        }
    }
}
