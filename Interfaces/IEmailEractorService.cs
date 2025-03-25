using GenAIED_Sandesh.Models;

namespace GenAIED_Sandesh.Interfaces
{
    public interface IEmailExtractorService
    {
        public string ExtractTextFromImages(MemoryStream image);
        public List<PredictionOutput> ReadEmailsAndAttachment();
        public List<PredictionOutput> ReadEmlFilesFromFolder(string folderPath);
        public (string, string) ReadEmlContent(string filePath);

        public string ExtractTextFromAttachment(Stream attachmentStream, string mimeType);
        public string ExtractTextFromWord(MemoryStream memoryStream);
        public string ExtractTextFromPdf(Stream pdfStream);

    }
}
