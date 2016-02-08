using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Windows;

namespace Sakuya_Aki
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// 检查是否是管理员身份
        /// </summary>
        private void CheckAdministrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);

            bool runAsAdmin = wp.IsInRole(WindowsBuiltInRole.Administrator);

            if (!runAsAdmin)
            {
                // It is not possible to launch a ClickOnce app as administrator directly,
                // so instead we launch the app as administrator in a new process.
                var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

                // The following properties run the new process as administrator
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";

                // Start the new process
                try
                {
                    Process.Start(processInfo);
                }
                catch
                {
                    MessageBox.Show("没有管理员权限\n请右键该程序之后点击“以管理员权限运行”");
                }

                // Shut down the current process
                Environment.Exit(0);
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //CheckAdministrator();
            ////如果不是管理员，程序会直接退出，并使用管理员身份重新运行。
            //StartupUri = new Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute);

            /**
             * 当前用户是管理员的时候，直接启动应用程序
             * 如果不是管理员，则使用启动对象启动程序，以确保使用管理员身份运行
             */
            //获得当前登录的Windows用户标示
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            //创建Windows用户主题
            System.Windows.Forms.Application.EnableVisualStyles();

            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
            //判断当前登录用户是否为管理员
            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                //如果是管理员，则直接运行

                StartupUri = new Uri("iFrame.xaml", UriKind.RelativeOrAbsolute);
            }
            else
            {
                //创建启动对象
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                //设置运行文件
                startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
                //设置启动参数
                //startInfo.Arguments = String.Join(" ", Args);
                //设置启动动作,确保以管理员身份运行
                startInfo.Verb = "runas";
                //如果不是管理员，则启动UAC
                System.Diagnostics.Process.Start(startInfo);
                //退出
                System.Windows.Forms.Application.Exit();
            }
        }
    }
}
