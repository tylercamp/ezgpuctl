using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GPUControl.Util
{
    public class AutoStart : ObservableObject
    {
        public static AutoStart Instance = new AutoStart();

        private static readonly string TaskName = "GPU Control";
        public bool IsEnabled
        {
            get
            {
                var task = TaskService.Instance.RootFolder.GetTasks().FirstOrDefault(t => t.Name == TaskName);
                return task?.Definition?.Actions?.Any(a => (a as ExecAction)?.Path == Environment.ProcessPath) == true;
            }

            set
            {
                TaskService.Instance.RootFolder.DeleteTask(TaskName, exceptionOnNotExists: false);

                if (value)
                {
                    var td = TaskService.Instance.NewTask();

                    td.RegistrationInfo.Description = "Run GPUControl on startup";

                    td.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;
                    td.Settings.AllowDemandStart = true;
                    td.Settings.ExecutionTimeLimit = TimeSpan.Zero;
                    td.Settings.DisallowStartOnRemoteAppSession = true;

                    td.Principal.RunLevel = TaskRunLevel.Highest;

                    td.Actions.Add(Environment.ProcessPath, null, Environment.CurrentDirectory);

                    td.Triggers.Add(new LogonTrigger() { UserId = null });

                    TaskService.Instance.RootFolder.RegisterTaskDefinition(TaskName, td);
                }

                OnPropertyChanged();
            }
        }
    }
}
