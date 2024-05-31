using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace programming_project
{
    public partial class MainWindow : Window
    {
        private List<Data> WordData;
        private Random random;
        private int currentWordIndex = -1;
        private int SequenceIndex = 0;
        private int currentNumber = 1; // 시작 번호를 1로 설정

        IFirebaseConfig config = new FirebaseConfig
        {
            BasePath = "https://vocalist-ef97c-default-rtdb.firebaseio.com/",
            AuthSecret = "mXcAu7GqGQ0nXRaWkzypmgLRuwBOIR1DjSkzTuoo"
        };
        IFirebaseClient client;

        public MainWindow()
        {
            InitializeComponent();
            client = new FireSharp.FirebaseClient(config);
            if (client == null)
            {
                MessageBox.Show("연결 실패!");
                return;
            }

            random = new Random();
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("Voca/");
                Data[] wordData = response.ResultAs<Data[]>();

                if (wordData == null || wordData.Length == 0)
                {
                    MessageBox.Show("No data found.");
                    return;
                }
                WordData = wordData.ToList();

                currentWordIndex = 0;

                DisplayWord(currentWordIndex);

                Data selectedWord = null;

                if (rbRandom.IsChecked == true)
                {
                    int index = random.Next(wordData.Length);
                    selectedWord = wordData[index];
                }
                else if (rbSequence.IsChecked == true)
                {
                    selectedWord = wordData[SequenceIndex % wordData.Length];
                    SequenceIndex++;
                }

                if (selectedWord != null)
                {
                    if (rbBoth.IsChecked == true)
                    {
                        tbWord.Text = selectedWord.Word;
                        tbMean.Text = selectedWord.Mean;
                    }
                    else if (rbMean.IsChecked == true)
                    {
                        tbWord.Text = "";
                        tbMean.Text = selectedWord.Mean;
                    }
                    else if (rbWord.IsChecked == true)
                    {
                        tbWord.Text = selectedWord.Word;
                        tbMean.Text = "";
                    }

                    tbNum.Text = $"No. {currentNumber}"; // currentNumber를 표시

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (WordData == null || WordData.Count == 0)
            {
                MessageBox.Show("단어 데이터가 없습니다.");
                return;
            }

            currentWordIndex++;
            if (currentWordIndex >= WordData.Count)
            {
                currentWordIndex = 0; // 다음 단어가 없으므로 처음으로 돌아감
            }

            currentNumber++; // 번호를 1씩 증가시킴

            DisplayWord(currentWordIndex);

            // 번호 업데이트
            tbNum.Text = $"No. {currentNumber}";
        }

        private void DisplayWord(int index)
        {
            if (WordData == null || WordData.Count == 0)
            {
                MessageBox.Show("No word data available.");
                return;
            }

            Data selectedWord = null;

            if (index >= 0 && index < WordData.Count)
            {
                selectedWord = WordData[index];
            }
            else
            {
                index = 0; // 다음 단어가 없으므로 처음으로 돌아감
                selectedWord = WordData[index];
            }

            if (selectedWord != null)
            {
                if (rbBoth.IsChecked == true)
                {
                    tbWord.Text = selectedWord.Word;
                    tbMean.Text = selectedWord.Mean;
                }
                else if (rbMean.IsChecked == true)
                {
                    tbWord.Text = "";
                    tbMean.Text = selectedWord.Mean;
                }
                else if (rbWord.IsChecked == true)
                {
                    tbWord.Text = selectedWord.Word;
                    tbMean.Text = "";
                }
                tbNum.Text = $"No. {currentNumber}"; // 현재 번호를 표시
            }
        }

        //private void btnHiragana_Click(object sender, RoutedEventArgs e)
        //{
        //    Window HP = new HiraganaWindow();
        //    HP.Show();
        //}
    }

    public class Data
    {
        public int No { get; set; }
        public string Word { get; set; }
        public string Mean { get; set; }
    }
}
