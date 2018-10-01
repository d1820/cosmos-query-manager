using Markdig;
using System;
using System.IO;
using System.Windows.Forms;

namespace CosmosManager
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
            try
            {
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                var fileContents = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/help.md");
                var html = Markdown.ToHtml(fileContents);
                webBrowser1.Navigate("about:blank");
                if (webBrowser1.Document != null)
                {
                    webBrowser1.Document.Write(string.Empty);
                }
                webBrowser1.DocumentText = html;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to load help. Error {ex.Message}", "Error Loading help");
                Close();
            }
        }
    }
}
