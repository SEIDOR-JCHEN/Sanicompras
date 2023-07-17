namespace Sanicompras.Classes
{
    public class clsLog
    {
        public log4net.ILog log = log4net.LogManager.GetLogger("root");

        public clsLog()
        {
            log4net.Config.XmlConfigurator.Configure(); 
        }
    }
}
