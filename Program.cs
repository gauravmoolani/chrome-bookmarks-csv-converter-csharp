using System;
using System.IO;
using System.Collections.Generic;
using HtmlAgilityPack;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

public class BookMark
{

    public string Title { get; set; }
    public string Url { get; set; }
    public string Category { get; set; }

}
public sealed class BookMarkMap : ClassMap<BookMark>
{
    public BookMarkMap()
    {
        AutoMap(CultureInfo.InvariantCulture);
        Map(m => m.Title).Name("Title");
        Map(m => m.Url).Name("URL");
    }
}
public class Program
{
    private static void Main(string[] args)
    {

        string filepath = "C:/Users/gaurav/Desktop/bookmarks_4_1_23.html";
        var bookmarks = ReadChromeBookmarks(filepath);
        // foreach (var bookmark in bookmarks)
        // {
        //     Console.WriteLine($"Title:{bookmark.Title}\n");
        // }
        Console.WriteLine("Hello, World!");
        string csvFilePath = "C:/Users/gaurav/Desktop/bookmarks.csv";
        SaveBookmarksToCsv(bookmarks, csvFilePath);
    }

    static List<BookMark> ReadChromeBookmarks(string filepath)
    {
        var bookmarks = new List<BookMark>();
        if (File.Exists(filepath))
        {

            var doc = new HtmlDocument();
            doc.Load(filepath);
            var links = doc.DocumentNode.SelectNodes("//a");
            WebClient client = new WebClient();

            foreach (var link in links)
            {
                var category = "";
                string htmlContent = "";
                var title = link.InnerText;
                var url = link.GetAttributeValue("href", string.Empty);
                category = IdentifyCategorybyContent(url);
                if (string.IsNullOrEmpty(category))
                {
                    try
                    {
                        htmlContent = client.DownloadString(url);
                    }
                    catch
                    {

                    }
                    if (htmlContent.Any())
                        category = IdentifyCategorybyMeta(htmlContent) ?? IdentifyCategorybyContent(htmlContent);
                }

                Console.WriteLine(@"Catgory found -- " + category);
                bookmarks.Add(new BookMark { Title = title, Url = url, Category = category });

            }



        }
        return bookmarks;
    }
    static string IdentifyCategorybyMeta(string htmlContent)
    {
        // Load the HTML content into an HTML document object
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        // Find the HTML element that indicates the category of the website
        HtmlNode categoryNode = doc.DocumentNode.SelectSingleNode("//meta[@name='category']");

        if (categoryNode != null)
        {
            // Extract the category value from the HTML element
            string categoryValue = categoryNode.GetAttributeValue("content", "").ToLower();

            // Map the category value to a more readable category name
            switch (categoryValue)
            {
                case "programming":
                    return "Programming";
                case "architecture":
                    return "Architecture";
                case "sql":
                    return "SQL";
                case "python":
                    return "Python";
                case "csharp":
                    return "C#";
                case "java":
                    return "Java";
                case "designpatterns":
                    return "Design Patterns";
                case "systemdesign":
                    return "System Design";
                case "concepts":
                    return "Concepts";
                default:
                    return "Other";
            }
        }
        else
        {
            return null;
        }
    }
    static string IdentifyCategorybyContent(string htmlContent)
    {
        // Define regular expressions to identify different programming-related categories
        // Regex programmingRegex = new Regex(@"programming|coding|code", RegexOptions.IgnoreCase);
        Regex architectureRegex = new Regex(@"architecture|design patterns|scalability", RegexOptions.IgnoreCase);
        Regex sqlRegex = new Regex(@"sql|tsql|mysql|postgres", RegexOptions.IgnoreCase);
        Regex pythonRegex = new Regex(@"python|django|flask|numpy", RegexOptions.IgnoreCase);
        Regex cSharpRegex = new Regex(@"c#|dotnet|asp\.net|winforms|wpf", RegexOptions.IgnoreCase);
        Regex javaRegex = new Regex(@"java|spring|hibernate|jvm", RegexOptions.IgnoreCase);
        Regex designPatternsRegex = new Regex(@"design patterns", RegexOptions.IgnoreCase);
        Regex systemDesignRegex = new Regex(@"system design|scalability|distributed systems", RegexOptions.IgnoreCase);
        Regex conceptsRegex = new Regex(@"algorithms|data structures|computer science", RegexOptions.IgnoreCase);

        // Check if the HTML content matches any of the regular expressions and return the corresponding category
        // if (programmingRegex.IsMatch(htmlContent))
        // {
        //     return "programming";
        // }
        if (architectureRegex.IsMatch(htmlContent))
        {
            return "architecture";
        }
        else if (sqlRegex.IsMatch(htmlContent))
        {
            return "sql";
        }
        else if (pythonRegex.IsMatch(htmlContent))
        {
            return "python";
        }
        else if (cSharpRegex.IsMatch(htmlContent))
        {
            return "c#";
        }
        else if (javaRegex.IsMatch(htmlContent))
        {
            return "java";
        }
        else if (designPatternsRegex.IsMatch(htmlContent))
        {
            return "design patterns";
        }
        else if (systemDesignRegex.IsMatch(htmlContent))
        {
            return "system design";
        }
        else if (conceptsRegex.IsMatch(htmlContent))
        {
            return "concepts";
        }
        else
        {
            return null;
        }
    }

    static void SaveBookmarksToCsv(List<BookMark> bookmarks, string csvFilePath)
    {
        var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";", Encoding = Encoding.UTF8, HasHeaderRecord = true };
        using (var writer = new StreamWriter(csvFilePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {

            //  csv.Configuration.AutoMap<BookMark>();
            csv.WriteHeader<BookMark>();
            csv.NextRecord();
            csv.WriteRecords(bookmarks);
        }
    }

}


