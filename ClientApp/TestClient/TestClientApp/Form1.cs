using TestClientApp.Test;

namespace TestClientApp
{
    public partial class Form1 : Form
    {
        private readonly Test_Network_DataIntegrity testNetworkDataIntegrity;

        public Form1()
        {
            InitializeComponent();

            // Initializer test objects.
            testNetworkDataIntegrity = new Test_Network_DataIntegrity(100, 9050, 2000);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonTestNetwork_DataIntegrity(object sender, EventArgs e)
        {
            if (testNetworkDataIntegrity.IsTestEnd())
            {
                CustomLogger.Log($"Test network dataIntegrity start");

                testNetworkDataIntegrity.Start();
            }
            else
            {
                CustomLogger.Log($"Network dataIntegrity is testing...");
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
