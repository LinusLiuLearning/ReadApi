using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;

namespace ReadApi01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {

                    string cvApiUrl = "https://cvimageserviceforlearning.cognitiveservices.azure.com/";
                    string cvApiKey = "5949389750fd437eab5b3799157e1393";
                    string imagePath = openFileDialog1.FileName;
                    // �إ�FileStream����fs�}�ҹ���
                    FileStream fs = File.Open(imagePath, FileMode.Open);

                    // �إ߹q����ı���Ѫ���A �P�ɫ��w�q����ı���Ѫ����ݪA��Key
                    ComputerVisionClient visionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(cvApiKey),
                        new System.Net.Http.DelegatingHandler[] { });

                    // �q����ı���Ѫ�����w���ݪA��Api��}
                    visionClient.Endpoint = cvApiUrl;

                    // ���� ReadInStreamAsync() ��k�ǰe���ɵo�e�ШD
                    ReadInStreamHeaders textHeaders = await visionClient.ReadInStreamAsync(fs);
                    // ���o Read API �ާ@�A�Ȱϰ��}
                    string operationLocation= textHeaders.OperationLocation;
                    // ����3�� �H�Q���o Read API �ާ@�A�Ȱϰ��}
                    Thread.Sleep(3000);

                    // ���o Read API �� operationId �@�~�ѧO�X(ID)
                    int numberOfCharsInOperationId = 36;
                    string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);
                    richTextBox1.Text = $"Read API �A�Ȧ�}�G{operationLocation}\n\n" + $"�@�~�ѧO�X(ID)�G{operationId}\n\n"; ;

                    // ���o�v���������媫��
                    ReadOperationResult results = await visionClient.GetReadResultAsync(Guid.Parse(operationId));

                    // �N��쪺�奻���e�v�@���w�� str
                    string str = "�v�����e��r�G";

                    IList<ReadResult> textUriFileResults = results.AnalyzeResult.ReadResults;
                    foreach (ReadResult page in textUriFileResults)
                    {
                        foreach (Line line in page.Lines)
                        {
                            str += line.Text + "\n";
                        }
                    }
                    // richTextBox1 ��ܼv���������夺�e
                    richTextBox1.Text += str;

                    // pictureBox1 ��ܫ��w���Ϥ�
                    pictureBox1.Image = new Bitmap(imagePath);
                    // ����v����y�귽
                    fs.Close();
                    fs.Dispose();
                    GC.Collect();

                }
            }
            catch (Exception ex)
            {
                richTextBox1.Text = $"���~�T��:{ex.Message}";
            }

        }
    }
}