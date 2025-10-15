using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Threading;
using Prism.Mvvm;
using 进程查看器.Models;

namespace 进程查看器.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private ObservableCollection<ProcessInfo> _processInfos = new ObservableCollection<ProcessInfo>();

        public ObservableCollection<ProcessInfo> ProcessInfos
        {
            get => _processInfos;
            set => SetProperty(ref _processInfos, value);
        }

        private ProcessInfo _selectedProcessInfo;

        public ProcessInfo SelectedProcessInfo
        {
            get => _selectedProcessInfo;
            set => SetProperty(ref _selectedProcessInfo, value);
        }
        public MainWindowViewModel()
        {
            LoadProcess();
        }
        /// <summary>
        /// 记载进程信息
        /// </summary>
        private void LoadProcess()
        {
            var processes = Process.GetProcesses();
            ProcessInfos.Clear();
            foreach (var process in processes)
            {
                try
                {
                    ProcessInfos.Add(new ProcessInfo
                    {
                        ProcessId = GetProcessId(process),
                        ProcessName = GetProcessName(process),
                        ProcessPath = GetProcessPath(process),
                        Modules = GetProcessModules(process),
                        StartTime = GetStartTime(process),
                        ThreadCount = GetThreadCount(process),
                        MemorySize = GetMemorySize(process)
                    });
                }
                catch (Exception e) when (IsAccessException(e))
                {
                    continue;
                }
            }
        }
        /// <summary>
        /// 获取进程加载的模块信息
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private ObservableCollection<ProcessModuleInfo> GetProcessModules(Process process)
        {
            var modules = new ObservableCollection<ProcessModuleInfo>();
            try
            {
                foreach (ProcessModule module in process.Modules)
                {
                    try
                    {
                        modules.Add(new ProcessModuleInfo
                        {
                            ModuleName = module.ModuleName,
                            ModulePath = module.FileName
                        });
                    }
                    catch (Exception e) when (IsAccessException(e))
                    {
                        continue;
                    }
                }

                return modules;
            }
            catch (Exception e) when (IsAccessException(e))
            {
                return null;
            }
        }
        /// <summary>
        /// 获取进程ID
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private int GetProcessId(Process process)
        {
            try
            {
                return process.Id;
            }
            catch (Exception e) when (IsAccessException(e))
            {
                return -1;
            }
        }
        /// <summary>
        /// 获取进程名
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private string GetProcessName(Process process)
        {
            try
            {
                return process.ProcessName;
            }
            catch (Exception e) when (IsAccessException(e))
            {
                return "拒绝访问";
            }
        }
        /// <summary>
        /// 获取进程路径
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private string GetProcessPath(Process process)
        {
            try
            {
                return process.MainModule == null ? "" : process.MainModule.FileName;
            }
            catch (Exception e) when (IsAccessException(e))
            {
                return "拒绝访问";
            }
        }
        /// <summary>
        /// 获取进程启动时间
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private DateTime GetStartTime(Process process)
        {
            try
            {
                return process.StartTime;
            }
            catch (Exception e) when (IsAccessException(e))
            {
                return DateTime.Now.AddYears(-1000);
            }
        }
        /// <summary>
        /// 获取进程的线程数量
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private int GetThreadCount(Process process)
        {
            try
            {
                return process.Threads.Count;
            }
            catch (Exception e) when (IsAccessException(e))
            {
                return -1;
            }
        }
        /// <summary>
        /// 获取进程内存占用
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private double GetMemorySize(Process process)
        {
            try
            {
                return process.WorkingSet64 / (1024 * 1024.0);
            }
            catch (Exception e) when (IsAccessException(e))
            {
                return -1;
            }
        }
        /// <summary>
        /// 判断是否报错
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool IsAccessException(Exception e)
        {
            return e is Win32Exception or UnauthorizedAccessException or NotSupportedException;
        }
    }
}