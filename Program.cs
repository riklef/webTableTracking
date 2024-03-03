using System;
using HtmlAgilityPack;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using DotNetEnv.Configuration;
using System.Text;

namespace webTableTracking
{
    internal class Program
    {
        public static string[,] ParseTable(string url)
        {
            // Load the HTML document from the URL
            HtmlWeb web = new HtmlWeb();
            web.OverrideEncoding = Encoding.UTF8;
            HtmlDocument doc = web.Load(url);

            // Select the table node (you may need to inspect the HTML source to find the appropriate XPath)
            HtmlNode tableHead = doc.DocumentNode.SelectSingleNode("//thead");

            if (tableHead == null)
            {
                Console.WriteLine("Table Head not found on the webpage.");
            }

            HtmlNode tableBody = doc.DocumentNode.SelectSingleNode("//tbody");

            if (tableBody == null)
            {
                Console.WriteLine("Table body not found on the webpage.");
                return null;
            }

            // Get all rows from the table head
            HtmlNodeCollection headRows = tableHead.SelectNodes("tr");

            // Determine the number of rows and columns in the table
            int headRowCount = headRows.Count;
            int headColumnCount = 0;

            foreach (HtmlNode row in headRows)
            {
                int currentColumnCount = row.SelectNodes("td|th").Count;
                if (currentColumnCount > headColumnCount)
                    headColumnCount = currentColumnCount;
            }


            HtmlNodeCollection bodyRows = tableBody.SelectNodes("tr");

            // Determine the number of rows and columns in the table
            int bodyRowCount = bodyRows.Count;
            int bodyColumnCount = 0;

            foreach (HtmlNode row in headRows)
            {
                int currentColumnCount = row.SelectNodes("td|th").Count;
                if (currentColumnCount > bodyColumnCount)
                    bodyColumnCount = currentColumnCount;
            }

            // Initialize a 2D array to hold the table data
            string[,] tableData = new string[headRowCount + bodyRowCount, headColumnCount + bodyColumnCount];

            // Iterate through each row and column, populating the table data for head
            for (int i = 0; i < headRowCount; i++)
            {
                HtmlNodeCollection columns = headRows[i].SelectNodes("td|th");

                for (int j = 0; j < headColumnCount; j++)
                {
                    if (columns != null && j < columns.Count)
                        tableData[i, j] = columns[j].InnerText.Trim();
                    else
                        tableData[i, j] = ""; // Fill empty cells with an empty string
                }
            }

            // Iterate through each row and column, populating the table data for body
            for (int i = 0; i < bodyRowCount; i++)
            {
                HtmlNodeCollection columns = bodyRows[i].SelectNodes("td|th");

                for (int j = 0; j < bodyColumnCount; j++)
                {
                    if (columns != null && j < columns.Count)
                        tableData[i, j] = columns[j].InnerText.Trim();
                    else
                        tableData[i, j] = ""; // Fill empty cells with an empty string
                }
            }

            return tableData;
        }

        static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder().AddUserSecrets<Program>();
            IConfigurationRoot configuration = configurationBuilder.Build();

            string url = configuration["WEBSITE"];
            string[,] tableData = ParseTable(url);

            if (tableData != null)
            {
                // Print the table data
                for (int i = 0; i < tableData.GetLength(0); i++)
                {
                    for (int j = 0; j < tableData.GetLength(1); j++)
                    {
                        Console.Write(tableData[i, j] + "\t");
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}