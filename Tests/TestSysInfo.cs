using System.Diagnostics;
using TestJobService;

namespace Tests;

[TestClass]
public class TestSysInfo
{
    [TestMethod]
    public void TestInfoModel()
    {
        var currentProcessName = Process.GetCurrentProcess().ProcessName;
        var settings = new Settings()
        {
            TrackedProcesses = new string[] {
            currentProcessName,
            "SomeUnexistentProcess" + DateTime.Now.Ticks
        }
        };
        var info = SystemInfo.GetInfo(settings);
        Assert.IsNotNull(info);

        // проверим что всё заполнено
        var props = typeof(InfoModel).GetProperties();
        foreach(var prop in props) Assert.IsNotNull(prop.GetValue(info), $"свойство {prop.Name} не установлено");

        //Есть запущенные процессы
        Assert.IsTrue(info.Processes.Any());

        // в отслеживаемых процессах должен быть только наш текущий
        Assert.AreEqual(1, info.TrackedProcesses.Length);
        Assert.AreEqual(currentProcessName, info.TrackedProcesses.FirstOrDefault());

    }
}
