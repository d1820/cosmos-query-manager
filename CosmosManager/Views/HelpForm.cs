using Markdig;
using System;
using System.IO;
using System.Reflection;
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
                var fileContents = LoadHelpMarkdown();
                var html = Markdown.ToHtml(fileContents, pipeline);
                webBrowser1.Navigate("about:blank");
                if (webBrowser1.Document != null)
                {
                    webBrowser1.Document.Write(string.Empty);
                }
                webBrowser1.DocumentText = WrapInDarkTheme(html);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to load help. Error {ex.Message}", "Error Loading help");
                Close();
            }
        }

        private static string LoadHelpMarkdown()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("CosmosManager.help.md"))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }

            // Fallback for environments where the file sits next to the executable.
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "help.md");
            return File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;
        }

        private static string WrapInDarkTheme(string bodyHtml)
        {
            return "<html><head><meta http-equiv='X-UA-Compatible' content='IE=edge'><style>" +
                   "body{background-color:#1e1e1e;color:#dcdcdc;font-family:'Segoe UI',Arial,sans-serif;margin:16px;}" +
                   "h1,h2,h3,h4{color:#ffffff;}" +
                   "a{color:#4ea8ee;}" +
                   "code{background-color:#2d2d2d;color:#dcdcdc;padding:1px 4px;border-radius:3px;}" +
                   "pre{background-color:#2d2d2d;color:#dcdcdc;padding:10px;border-radius:4px;overflow:auto;}" +
                   "table{border-collapse:collapse;}th,td{border:1px solid #444;padding:4px 8px;}" +
                   "th{background-color:#2d2d2d;}" +
                   "</style></head><body>" + bodyHtml + "</body></html>";
        }
    }
}
