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
                    // 建立FileStream物件fs開啟圖檔
                    FileStream fs = File.Open(imagePath, FileMode.Open);

                    // 建立電腦視覺辨識物件， 同時指定電腦視覺辨識的雲端服務Key
                    ComputerVisionClient visionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(cvApiKey),
                        new System.Net.Http.DelegatingHandler[] { });

                    // 電腦視覺辨識物件指定雲端服務Api位址
                    visionClient.Endpoint = cvApiUrl;

                    // 執行 ReadInStreamAsync() 方法傳送圖檔發送請求
                    ReadInStreamHeaders textHeaders = await visionClient.ReadInStreamAsync(fs);
                    // 取得 Read API 操作服務區域位址
                    string operationLocation= textHeaders.OperationLocation;
                    // 等待3秒 以利取得 Read API 操作服務區域位址
                    Thread.Sleep(3000);

                    // 取得 Read API 的 operationId 作業識別碼(ID)
                    int numberOfCharsInOperationId = 36;
                    string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);
                    richTextBox1.Text = $"Read API 服務位址：{operationLocation}\n\n" + $"作業識別碼(ID)：{operationId}\n\n"; ;

                    // 取得影像中的本文物件
                    ReadOperationResult results = await visionClient.GetReadResultAsync(Guid.Parse(operationId));

                    // 將找到的文本內容逐一指定給 str
                    string str = "影像內容文字：";

                    IList<ReadResult> textUriFileResults = results.AnalyzeResult.ReadResults;
                    foreach (ReadResult page in textUriFileResults)
                    {
                        foreach (Line line in page.Lines)
                        {
                            str += line.Text + "\n";
                        }
                    }
                    // richTextBox1 顯示影像中的本文內容
                    richTextBox1.Text += str;

                    // pictureBox1 顯示指定的圖片
                    pictureBox1.Image = new Bitmap(imagePath);
                    // 釋放影像串流資源
                    fs.Close();
                    fs.Dispose();
                    GC.Collect();

                }
            }
            catch (Exception ex)
            {
                richTextBox1.Text = $"錯誤訊息:{ex.Message}";
            }

        }
    }
}