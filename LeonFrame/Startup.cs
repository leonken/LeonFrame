using Application.AutofacModules;

namespace LeonFrameAPI

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
            //services.AddDbContext<LContext>(r => r.UseSqlServer(Configuration["ConnectionString"]));
            //services.AddDbContext<IntegrationEventLogDbContext>(r => r.UseSqlServer(Configuration["ConnectionString"],
            //    optBuilder =>
            //    {
            //        optBuilder.MigrationsAssembly("Infrastructure");
            //    }
            //    ));
            services.AddMyDbContext(Configuration);
            //services.AddControllers();
            //services.AddMediatR(typeof(Startup));  ʹ��net core����Ӧ����չ�Ⲣʹ�����ַ�ʽע�롣��Ϊ��ʹ��autofac�ķ�ʽ����˲���autofacע�롣������չٷ��ĵ�
            services.AddCustomInterations(Configuration);
            //services.AddSingleton<IObjectModelValidator, CustomModelValidator>(); Get����������
            services.AddCors(setup =>
            {
                                                 //�����������ʾ���Է���cookie(Ajax����ʱ��Ҫ��withCredentials)
                                                 //����Ҫ��Origins������Ϊ*
                    configure.WithExposedHeaders("head1", "head2");//ָ����������˱�¶��6������head֮�������ͷ
                });
            {

                #region swagger��jwt֧��
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "���¿�����������ͷ����Ҫ����Jwt��ȨToken��Bearer Token",
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
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            });
                {
                    };

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)

            app.UseEndpoints(endpoints =>

            app.UseSwagger();
            {
            });

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder containerBuilder)



            #region Register MediatR //�ο��ٷ��ĵ�
            containerBuilder.RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();
            {

            /*MediatR�ٷ�����*/
            // finally register our custom code (individually, or via assembly scanning)
            // - requests & handlers as transient, i.e. InstancePerDependency()
            // - pre/post-processors as scoped/per-request, i.e. InstancePerLifetimeScope()
            // - behaviors as transient, i.e. InstancePerDependency()

            #endregion
            containerBuilder.RegisterModule(new ApplicationModule(Configuration["ConnectionString"], Configuration));
                                                             //opt.Filters.Add<ViewModelRequestValidationFilter>();
                opt.Filters.Add<ResultFilter>();
            //LContext
            services.AddDbContext<LContext>(optionsBuilder =>
            {

            //IntegrationEventLogDbContext
            services.AddDbContext<IntegrationEventLogDbContext>(optionsBuilder =>
        }

        {