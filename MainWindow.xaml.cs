using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleAsyncDemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private List<string> PrepData()
        {
            List<string> output;

            output= new List<string>();

            this.resultsWindow.Text = "";

            output.Add("https://www.yahoo.com");
            output.Add("https://www.google.com");
            output.Add("https://www.microsoft.com");
            output.Add("https://www.cnn.com");
            output.Add("https://www.codeproject.com");
            output.Add("https://www.stackoverflow.com");

            return output;
        }

        private WebsiteDataModel DownloadWebsite(string websiteURL)
        {
            WebClient client;
            string websiteContent;

            client = new WebClient();
            websiteContent = client.DownloadString(websiteURL);

            return new WebsiteDataModel(websiteURL, websiteContent);
        }
        private async Task <WebsiteDataModel> DownloadWebsiteAsync(string websiteURL)
        {
            WebClient client;
            string websiteContent;

            client = new WebClient();
            websiteContent = await client.DownloadStringTaskAsync(websiteURL);

            return new WebsiteDataModel(websiteURL, websiteContent);
        }
        private void ReportWebsiteInfo(WebsiteDataModel data)
        {
            resultsWindow.Text += $"{ data.WebsiteUrl } downloaded: { data.WebsiteData.Length } characters long. { Environment.NewLine }";
        }


        private void RunDownloadSync()
        {
            List<string> websites;

            websites = this.PrepData();

            foreach (string website in websites)
            {
                WebsiteDataModel result;
                result = this.DownloadWebsite(website);
                ReportWebsiteInfo(result);
            }
        }

        private async Task RunDownloadAsync()
        {
            List<string> websites;

            websites = this.PrepData();

            foreach (string website in websites)
            {
                WebsiteDataModel result;
                result = await Task.Run(() => this.DownloadWebsite(website));
                ReportWebsiteInfo(result);
            }
        }
        private async Task RunDownloadParallelAsync()
        {
            List<string> websites;
            List<Task<WebsiteDataModel>> tasks;
            WebsiteDataModel[] results;

            websites = this.PrepData();
            tasks = new List<Task<WebsiteDataModel>>();

            foreach (string website in websites)
            {
                Task<WebsiteDataModel> task;
                
                task = this.DownloadWebsiteAsync(website);
                tasks.Add(task);
            }

            results = await Task.WhenAll(tasks);
            foreach (WebsiteDataModel result in results)
            {
                ReportWebsiteInfo(result);
            }
        }

        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch watch;
            long elapsedMs;

            watch = Stopwatch.StartNew();
            this.RunDownloadSync();
            watch.Stop();

            elapsedMs = watch.ElapsedMilliseconds;
            this.resultsWindow.Text += $"Total execution time: { elapsedMs } ms";
        }

        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch watch;
            long elapsedMs;

            watch = Stopwatch.StartNew();
            await this.RunDownloadParallelAsync();
            watch.Stop();

            elapsedMs = watch.ElapsedMilliseconds;
            this.resultsWindow.Text += $"Total execution time: { elapsedMs } ms";
        }
    }
}
