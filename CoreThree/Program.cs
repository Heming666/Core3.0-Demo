using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace CoreThree
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(option=> {
                        ////Ϊ����Ӧ�����ò����򿪵���� TCP ������,Ĭ������£������������������ (NULL)
                        //opton.Limits.MaxConcurrentConnections = 100;
                        //�����Ѵ� HTTP �� HTTPS ��������һ��Э�飨���磬Websocket ���󣩵����ӣ���һ�����������ơ� ���������󣬲������ MaxConcurrentConnections ����
                        //opton.Limits.MaxConcurrentUpgradedConnections = 100;
                        //��ȡ�����ñ��ֻ״̬��ʱ�� Ĭ��ֵΪ 2 ���ӡ�
                        option.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(20);
                        //  opton.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
                    });
                    webBuilder.UseStartup<Startup>();
                   
                }).ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddNLog("NLog.config");
                }).UseNLog();
    }
}
