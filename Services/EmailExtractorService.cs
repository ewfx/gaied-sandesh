using GenAIED_Sandesh.Interfaces;
using Microsoft.Office.Interop.Word;
using MimeKit;
using System.Runtime.InteropServices;
using Tesseract;
using Application = Microsoft.Office.Interop.Word.Application;

namespace GenAIED_Sandesh.Services
{
    public class EmailExtractorService:IEmailExtractorService
    {
        string path =string.Empty;
        private readonly IWebHostEnvironment _env;

        public EmailExtractorService(IWebHostEnvironment env)
        {
            _env = env;
             path= _env.ContentRootPath;
        }
        public string ExtractTextFromImages(MemoryStream image)
        {
            //string imagePath = "path/to/your/image.png";

            // Path to the tessdata folder (containing eng.traineddata)
            string tessDataPath = Path.Combine(path, "tessdata");

            // Initialize Tesseract engine
            using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
            {
                // Load the image
                using (var img = Pix.LoadFromMemory(image.ToArray()))
                {
                    // Perform OCR
                    using (var page = engine.Process(img))
                    {
                        // Get the extracted text
                        string extractedText = page.GetText();
                        Console.WriteLine("Extracted Text:");
                        Console.WriteLine(extractedText);
                        return extractedText;
                        // Get confidence level (optional)
                        //Console.WriteLine($"Confidence: {page.GetMeanConfidence()}");
                    }
                }
            }

        }

        public List<string> ReadEmailsAndAttachment()
        {
            string folderPath = Path.Combine(path, "MailMessages"); // Change to your folder path

            return ReadEmlFilesFromFolder(folderPath);
        }

        public List<string> ReadEmlFilesFromFolder(string folderPath)
        {
            var emailTexts = new List<string>();
            var emlFiles = Directory.GetFiles(folderPath, "*.eml");

            foreach (var emlFile in emlFiles)
            {
                string emailContent = ReadEmlContent(emlFile);
                emailTexts.Add(emailContent);
            }

            return emailTexts;
        }

        public string ReadEmlContent(string filePath)
        {
            try
            {
                var message = MimeMessage.Load(filePath);
                var emailText = message.TextBody ?? message.HtmlBody ?? "";

                foreach (var attachment in message.Attachments)
                {
                    if (attachment is MimePart part && part.IsAttachment)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            string attachmentText = "";
                            part.Content.DecodeTo(memoryStream);

                            if (part.ContentType.MimeType.ToLower().Contains("image"))
                            {
                                attachmentText = ExtractTextFromImages(memoryStream);
                            }
                            else if (part.ContentType.MimeType.ToLower().Contains("word"))
                            {
                                attachmentText = ExtractTextFromWord(memoryStream);
                            }
                            else
                            {
                                attachmentText = ExtractTextFromAttachment(memoryStream, part.ContentType.MimeType);
                            }
                            emailText += "\n\n" + attachmentText;
                        }
                    }
                }

                return emailText;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading email: " + ex.Message);
                return "";
            }
        }

        public string ExtractTextFromAttachment(Stream attachmentStream, string mimeType)
        {
            try
            {
                if (mimeType == "text/plain")
                {
                    using (var reader = new StreamReader(attachmentStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
                else if (mimeType == "application/pdf")
                {
                    return ExtractTextFromPdf(attachmentStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error extracting text: " + ex.Message);
            }
            return "";
        }

        public string ExtractTextFromPdf(Stream pdfStream)
        {
            try
            {
                pdfStream.Position = 0; // Ensure stream starts from the beginning

                using (var reader = new iText.Kernel.Pdf.PdfReader(pdfStream))
                {
                    // Handle encrypted PDFs
                    //if (reader.IsEncrypted())
                    //{
                    //    Console.WriteLine("PDF is encrypted and cannot be read.");
                    //    return "";
                    //}

                    using (var document = new iText.Kernel.Pdf.PdfDocument(reader))
                    {
                        string extractedText = "";
                        int numberOfPages = document.GetNumberOfPages();

                        for (int i = 1; i <= numberOfPages; i++) // Pages start from 1
                        {
                            extractedText += iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(document.GetPage(i)) + "\n";
                        }

                        return extractedText;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading PDF: " + ex.Message);
                return "";
            }
        }
        public string ExtractTextFromWord(MemoryStream memoryStream)
        {
            string tempFilePath = Path.GetTempFileName() + ".docx"; // Ensure correct extension

            try
            {
                // Step 1: Save MemoryStream to a temporary file
                File.WriteAllBytes(tempFilePath, memoryStream.ToArray());

                // Step 2: Initialize Word Interop
                Microsoft.Office.Interop.Word.Application wordApp = new Application();
                Document doc = null;
                string extractedText = "";

                try
                {
                    object missing = Type.Missing;
                    object readOnly = true;
                    object filePathObj = tempFilePath;

                    // Step 3: Open the document
                    doc = wordApp.Documents.Open(ref filePathObj, ref missing, ref readOnly,
                                                 ref missing, ref missing, ref missing,
                                                 ref missing, ref missing, ref missing,
                                                 ref missing, ref missing, ref missing,
                                                 ref missing, ref missing, ref missing, ref missing);

                    // Step 4: Extract text
                    extractedText = doc.Content.Text;
                }
                finally
                {
                    // Step 5: Cleanup COM objects
                    doc?.Close(false);
                    wordApp.Quit();
                    Marshal.ReleaseComObject(doc);
                    Marshal.ReleaseComObject(wordApp);
                }

                return extractedText;
            }
            finally
            {
                // Step 6: Delete temporary file
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

    }
}
