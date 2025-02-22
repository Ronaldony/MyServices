using System.Text;
using System.Windows.Forms;

namespace TestClientApp
{
    /// <summary>
    /// CustomLogger.
    /// </summary>
    public static class CustomLogger
    {
        private static StringBuilder _log = new StringBuilder();

        private static Lock _lock = new Lock();
        private static RichTextBox _textBox;

        /// <summary>
        /// SetLogBox.
        /// </summary>
        public static void SetLogBox(RichTextBox textBox)
        {
            _textBox = textBox;
        }

        /// <summary>
        /// Log.
        /// </summary>
        public static void Log(string log)
        {
            lock (_lock)
            {
                _log.AppendLine(log);
                _textBox.Invoke(new MethodInvoker(delegate { _textBox.Text = _log.ToString(); }));
            }
        }

        /// <summary>
        /// Flush.
        /// </summary>
        public static void Clear()
        {
            _log.Clear();
            _textBox.Invoke(new MethodInvoker(delegate { _textBox.Text = ""; }));
        }
    }
}
