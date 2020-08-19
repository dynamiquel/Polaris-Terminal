namespace Polaris.Terminal.Unity
{
    public interface ITerminalUI
    {
        void Write(LogMessage logMessage);
        void Clear();
        void ParseInput();
    }
}