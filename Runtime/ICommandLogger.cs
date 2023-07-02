namespace Ametrin.Command{
    public interface ICommandLogger{
        public void Log(string message);
        public void LogWarning(string message);
        public void LogError(string message);
    }
}
