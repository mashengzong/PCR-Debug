using log4net;
using log4net.Appender;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PortControlDemo
{
    public class log4netHelper
    {
        private static ILog logger;
        static log4netHelper()
        {
            if (logger == null)
            {
                var repository = LogManager.CreateRepository("NETCoreRepository");
                //log4net从log4net.config文件中读取配置信息
                XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
                logger = LogManager.GetLogger(repository.Name, "InfoLogger");
                RunClearJob();
            }
        }

        #region 清除过期日志

        /// <summary>
        /// 启动清除过期日志线程
        /// </summary>
        private static void RunClearJob()
        {
            Task.Run(() =>
            {
                try
                {
                    ClearOverdue();
                    Console.WriteLine("清除log4net过期日志");
                }
                catch (Exception e)
                {
                }
            });
        }

        /// <summary>
        /// 定期清除过期日志
        /// </summary>
        private static void ClearOverdue()
        {
            var days = 15;
            if (File.Exists("Log4net.config"))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(@"Log4net.config");
                    var node = doc.SelectSingleNode("/configuration/log4net");
                    days = Convert.ToInt32(node.Attributes["OverdueDays"].Value);
                }
                catch
                {
                    // ignored
                }
            }

            var apps = logger.Logger.Repository.GetAppenders();
            if (apps.Length <= 0)
            {
                return;
            }

            var now = DateTime.UtcNow.AddDays(-days);
            foreach (var item in apps)
            {
                if (item is RollingFileAppender roll)
                {
                    var dir = Path.GetDirectoryName(roll.File);
                    var files = Directory.GetFiles(dir, "*.log.*");

                    foreach (var filePath in files)
                    {
                        var file = new FileInfo(filePath);
                        if (file.CreationTime < now || file.LastWriteTime < now)
                        {
                            try
                            {
                                file.Delete();
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// （调试信息）：记录系统用于调试的一切信息，内容或者是一些关键数据内容的输出。 下面演示根据每个日志等级生成对应的一个文件。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Debug(string message, Exception exception = null)
        {
            if (exception == null)
                logger.Debug(message);
            else
                logger.Debug(message, exception);
        }

        /// <summary>
        /// INFO（一般信息）：记录系统运行中应该让用户知道的基本信息。例如，服务开始运行，功能已经开户等。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Info(string message, Exception exception = null)
        {
            if (exception == null)
                logger.Info(message);
            else
                logger.Info(message, exception);
        }

        /// <summary>
        /// WARN（警告）：记录系统中不影响系统继续运行，但不符合系统运行正常条件，有可能引起系统错误的信息。例如，记录内容为空，数据内容不正确等。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Warn(string message, Exception exception = null)
        {
            if (exception == null)
                logger.Warn(message);
            else
                logger.Warn(message, exception);
        }

        /// <summary>
        /// （一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Error(string message, Exception exception = null)
        {
            if (exception == null)
                logger.Error(message);
            else
                logger.Error(message, exception);
        }

        /// <summary>
        /// FATAL（致命错误）：记录系统中出现的能使用系统完全失去功能，服务停止，系统崩溃等使系统无法继续运行下去的错误。例如，数据库无法连接，系统出现死循环。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Fatal(string message, Exception exception = null)
        {
            if (exception == null)
                logger.Fatal(message);
            else
                logger.Fatal(message, exception);
        }

    }
}
