using System;
using System.Collections.ObjectModel;

namespace 进程查看器.Models;

public class ProcessInfo
{
    public int ProcessId { get; set; } //进程ID
    public string ProcessName { get; set; } //进程名
    public string ProcessPath { get; set; } //进程可执行文件所在路径
    public ObservableCollection<ProcessModuleInfo> Modules { get; set; } //模块信息
    public DateTime StartTime { get; set; } //开始时间
    public int ThreadCount { get; set; } //线程数量
    public double MemorySize { get; set; } //内存占用
}