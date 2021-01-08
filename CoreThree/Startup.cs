using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreThree.Filter;
using DBModel;
using IService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service;

namespace CoreThree
{
    public class Startup
    {
        private IConfiguration _configuration;
        public Startup(IConfiguration config)
        {
            _configuration = config;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();//ע��MVC���
            services.AddSingleton<IUserInfo,UserInfoService>();//ע��ӿ�
            services.AddSingleton<IDearptment, DepartmentService>();//ע��ӿ�
            services.AddSingleton<IDateService, DateService>();//时间帮助类
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            //�������ݿ�����
            EFCoreDbContext.ConnStr = _configuration.GetConnectionString("TestDB");

            //����Redis�������ַ���
            RedisClient.InitConnect(_configuration.GetConnectionString("RedisConStr"));

            Action<MvcOptions> filters = new Action<MvcOptions>(r => {
                r.Filters.Add(typeof(AuthFilter));
                r.Filters.Add(typeof(ExceptionFilter));
                r.Filters.Add(typeof(ResourceFilter));
                r.Filters.Add(typeof(ActionFilter));
                r.Filters.Add(typeof(ResourceFilter));
            });

            services.AddMvc(filters) //ע��ȫ�ֹ�����
              .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var provider = new FileExtensionContentTypeProvider();
            //����һЩ�µ�ӳ��
            provider.Mappings[".rmvb"] = "audio/x-pn-realaudio";
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();//ע���û������֤
            app.UseStaticFiles();//ע�ᾲ̬�ļ�
            app.UseRouting();//ע��·��
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=DateHelper}/{action=Index}/{id?}");//����·��
            });
        }
    }
}
