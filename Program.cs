using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Drawing;

string pdfFilePath = "file:///C:/Gazette%20A23.pdf";

List<string> rollNumbers = new List<string>(); // List to store roll numbers

using (PdfReader reader = new PdfReader(pdfFilePath))
{
    int record = 0;
    for (int page = 36; page <= 1282; page++)
    {
        // Extract text from the current page
        string pageText = PdfTextExtractor.GetTextFromPage(reader, page, new SimpleTextExtractionStrategy());

        // Process the extracted text to extract Roll No and Student Name
        ProcessPageText(pageText, ref record, rollNumbers);
    }
}

// Display the list of roll numbers
foreach (string rollNumber in rollNumbers)
{
    Console.WriteLine(rollNumber);
    SaveImage(rollNumber);
}


Console.WriteLine("All tasks completed.");
Console.ReadLine();
    

    static void ProcessPageText(string pageText, ref int record, List<string> rollNumbers)
{
    // Split the page text into lines
    string[] lines = pageText.Split('\n');



    // Process each line
    foreach (var line in lines)
    {
        // Check if the line contains relevant information (Roll No and Student Name)
        if (IsRelevantInformation(line))
        {
            // Extract and process Roll No and Student Name
            ExtractAndProcessInfo(line, ref record, rollNumbers);
        }
    }
}

static bool IsRelevantInformation(string line)
{
    // Implement your logic to determine if the line contains relevant information
    // For example, check if the line contains Roll No and Student Name
    // You can use regular expressions or other string matching methods

    // Example:
    return line.ToLower().Contains("StudentName") || line.ToLower().Contains("StudentNameOtherNameMayBe");
}

static void ExtractAndProcessInfo(string line, ref int record, List<string> rollNumbers)
{
    // Extract Roll No and Student Name from the line
    // Implement your logic to extract the required information

    // Example:
    string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

    foreach (var item in parts)
    {
        if (IsIntegerWithLength5(item))
        {
            // Store the roll number in the list
            rollNumbers.Add(item);
            Console.WriteLine($"Record {record}, Roll No: {item}");
        }
    }

    record++;
}

static bool IsIntegerWithLength5(string str)
{
    int result;
    return int.TryParse(str, out result) && str.Length == 6;
}
static  void SaveImage(string rollNumber)
{
    IWebDriver driver = new ChromeDriver();

    try
    {
        driver.Navigate().GoToUrl("http://result.biselahore.com/");

        // Find the text field and enter text into it
        IWebElement textField = driver.FindElement(By.XPath("//*[@id='txtFormNo']"));
        textField.SendKeys(rollNumber);

        // Find the radio button and click it
        IWebElement radioInput = driver.FindElement(By.XPath("//*[@id='rdlistCourse_1']"));
        IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
        executor.ExecuteScript("arguments[0].click();", radioInput);

        // Find the button and click it
        IWebElement button = driver.FindElement(By.XPath("//*[@id='Button1']"));
        button.Click();

        // Wait for a few seconds (adjust as needed) to ensure the PDF loads
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        IWebElement imageElement = wait.Until(driver =>
        {
            IWebElement element = driver.FindElement(By.XPath("//*[@id='imgDisplay']"));
            return element.Displayed ? element : null;
        });

        // Get the location and size of the image element
        Point location = imageElement.Location;
        Size size = imageElement.Size;

        // Take a screenshot of the entire page
        Screenshot screenshot = ((ITakesScreenshot)imageElement).GetScreenshot();

        // Convert the screenshot to a .NET Bitmap
        screenshot.SaveAsFile($"{rollNumber}.png", ScreenshotImageFormat.Png);

        // You can continue with the rest of your automation steps, such as waiting for the PDF download and saving it.
    }
    finally
    {
        // Don't forget to close the WebDriver when you're done
        driver.Quit();
    }
}
