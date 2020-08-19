namespace Polaris.Debug.Terminal
{
    public class TerminalInputField
    {
        // The current index of the history to look at.
        // Since it's a LIFO structure, 0 = Size - 1.
        // -1 is no history selected.
        public int historyIndex = -1;

        public void OnInputField_TextChanged()
        {
            historyIndex = -1;
        }

        public void OnInputField_UpArrow()
        {
            historyIndex++;
        }

        public void OnInputField_DownArrow()
        {
            historyIndex++;
        }
    }
}