using Application.AutofacModules;using Application.DIStartup;using Autofac;using AutoMapper;using Domain.IntegrationEvents.Events;using EventBus.Abstractions;using Infrastructure.DBContext;using IntegrationEventLog;using IntegrationEventLog.Services;using LeonFrameAPI.Filters;using LeonFrameAPI.ModelValidator;using MediatR;using Microsoft.AspNetCore.Builder;using Microsoft.AspNetCore.Hosting;using Microsoft.AspNetCore.Http;using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;using Microsoft.EntityFrameworkCore;using Microsoft.Extensions.Configuration;using Microsoft.Extensions.DependencyInjection;using Microsoft.Extensions.Hosting;using System;using System.Data.Common;using Swashbuckle.AspNetCore;using System.IO;using Microsoft.AspNetCore.Authentication.JwtBearer;using Microsoft.IdentityModel.Tokens;using System.Text;using Microsoft.OpenApi.Models;

namespace LeonFrameAPI{    public class Startup    {        public Startup(IConfiguration configuration)        {            Configuration = configuration;        }        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)        {
            //services.AddDbContext<LContext>(r => r.UseSqlServer(Configuration["ConnectionString"]));
            //services.AddDbContext<IntegrationEventLogDbContext>(r => r.UseSqlServer(Configuration["ConnectionString"],
            //    optBuilder =>
            //    {
            //        optBuilder.MigrationsAssembly("Infrastructure");
            //    }
            //    ));
            services.AddMyDbContext(Configuration);            services.AddAutoMapper(typeof(Application.Mapper.MapperRegister));            services.AddCustomMvc();
            //services.AddControllers();
            //services.AddMediatR(typeof(Startup));  使用net core可以应用扩展库并使用这种方式注入。但为了使用autofac的方式，因此采用autofac注入。具体参照官方文档
            services.AddCustomInterations(Configuration);
            //services.AddSingleton<IObjectModelValidator, CustomModelValidator>(); Get请求有问题
            services.AddCors(setup =>
            {                setup.AddDefaultPolicy(configure =>                {                    configure.WithOrigins("http://Leonsite.com");                    configure.AllowCredentials();//会在Head中设置Access-Control-Allow-Credentials 
                                                 //以向浏览器表示可以发送cookie(Ajax请求时需要打开withCredentials)
                                                 //但是要求Origins不能设为*
                    configure.WithExposedHeaders("head1", "head2");//指定向浏览器端暴露除6个基本head之外的其他头
                });            });            services.AddSwaggerGen(opt =>
            {                opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo                {                    Title = "Leon框架",                    Version = "V1.0"                });

                #region swagger的jwt支持
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

                #endregion
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);                var xmlPath = Path.Combine(basePath, "LeonFrameAPI.xml");                opt.IncludeXmlComments(xmlPath);//加载XML注释,可以添加多个
            });            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)                .AddJwtBearer(option =>
                {                    option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters                    {                        ValidateIssuer = true,                        ValidateAudience = true,                        ValidateLifetime = true,                        ClockSkew = TimeSpan.FromSeconds(30),                        ValidateIssuerSigningKey = true,                        ValidAudience = "http://www.baidu.com",                        ValidIssuer = "http://www.baidu.com",                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("LeonKeykey1111111111111111111"))//用来做校验hash的key
                    };                });        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)        {            app.Use(next => new RequestDelegate(                  async context =>                  {                      context.Request.EnableBuffering();                      await next(context);                  }              ));            if (env.IsDevelopment())            {                app.UseDeveloperExceptionPage();            }            app.UseHttpsRedirection();            app.UseRouting();            app.UseCors();            app.UseAuthentication();            app.UseAuthorization();

            app.UseEndpoints(endpoints =>            {                endpoints.MapControllers();            });            app.RegisterIntegrationEvent();//注册集成事件处理 

            app.UseSwagger();            app.UseSwaggerUI(opt =>
            {                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Leon framework api");                opt.RoutePrefix = "";//将根目录作为swagger首页 
            });        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder containerBuilder)        {            containerBuilder.Register<Func<DbConnection, IIntegrationEventLogService>>(r => (conn) =>            {                return new IntegrationEventLogService(conn);            }).AsSelf().InstancePerDependency();



            #region Register MediatR //参考官方文档
            containerBuilder.RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();            containerBuilder.Register<ServiceFactory>(context =>
            {                var c = context.Resolve<IComponentContext>();                return t => c.Resolve(t);            });            containerBuilder.RegisterModule<MediatrModule>();//自定义逻辑

            /*MediatR官方建议*/
            // finally register our custom code (individually, or via assembly scanning)
            // - requests & handlers as transient, i.e. InstancePerDependency()
            // - pre/post-processors as scoped/per-request, i.e. InstancePerLifetimeScope()
            // - behaviors as transient, i.e. InstancePerDependency()

            #endregion
            containerBuilder.RegisterModule(new ApplicationModule(Configuration["ConnectionString"], Configuration));        }    }    static class CustomExtesionMethods    {        public static IServiceCollection AddCustomMvc(this IServiceCollection services)        {            services.AddControllers(opt =>            {                opt.Filters.Add<HttpGlobalExceptionFilter>();//全局异常过滤器
                                                             //opt.Filters.Add<ViewModelRequestValidationFilter>();
                opt.Filters.Add<ResultFilter>();            });            return services;        }        public static IServiceCollection AddMyDbContext(this IServiceCollection services, IConfiguration config)        {
            //LContext
            services.AddDbContext<LContext>(optionsBuilder =>
            {                optionsBuilder.UseSqlServer(config["ConnectionString"]);            });

            //IntegrationEventLogDbContext
            services.AddDbContext<IntegrationEventLogDbContext>(optionsBuilder =>            {                optionsBuilder.UseSqlServer(config["ConnectionString"]                    , sqlOptionsBuilder =>                    {                        sqlOptionsBuilder.MigrationsAssembly(typeof(LContext).Assembly.FullName);                    });            });            return services;
        }        public static void RegisterIntegrationEvent(this IApplicationBuilder app)

        {            var eventBus = app.ApplicationServices.GetService<IEventBus>();            eventBus.Subscribe<UserCreatedIntegrationEvent, IIntegrationEventHandler<UserCreatedIntegrationEvent>>();        }    }}