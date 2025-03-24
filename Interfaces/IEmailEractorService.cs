namespace GenAIED_Sandesh.Interfaces
{
    public interface IEmailExtractorService
    {
        public string ExtractTextFromImages(MemoryStream image);
        public List<string> ReadEmailsAndAttachment();
        public List<string> ReadEmlFilesFromFolder(string folderPath);
        public string ReadEmlContent(string emlFile);

        public string ExtractTextFromAttachment(Stream attachmentStream, string mimeType);
        public string ExtractTextFromWord(MemoryStream memoryStream);
        public string ExtractTextFromPdf(Stream pdfStream);

    }
}
