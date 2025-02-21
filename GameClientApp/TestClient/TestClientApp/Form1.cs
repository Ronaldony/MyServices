using TestClientApp.Test;

namespace TestClientApp
{
    public partial class Form1 : Form
    {
        private readonly TestNetwork_DataIntegrity testNetwork_DataIntegrity;

        public Form1()
        {
            InitializeComponent();

            // Initializer test objects.
            testNetwork_DataIntegrity = new TestNetwork_DataIntegrity(10, 9050, 10000);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonTestNetwork_DataIntegrity(object sender, EventArgs e)
        {
            if (testNetwork_DataIntegrity.IsTestEnd())
            {
                testNetwork_DataIntegrity.Start();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            CustomLogger.Clear();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
