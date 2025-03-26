Table of Contents

Introduction
Demo
Inspiration
What It Does
How We Built It
Challenges We Faced
How to Run
Tech Stack
Introduction

This application is a text classification system designed to categorize banking-related customer inquiries into predefined request types and sub-request types. It leverages machine learning (ML.NET) for classification and integrates with email processing (MimeKit) and OCR (Tesseract) to extract and analyze text from emails, attachments (PDFs, Word documents, and images), and other sources.

Demo

Input: Emails with attachments (e.g., "How do I apply for a loan?" or "My debit card transaction was declined.").
Output: Predicted categories (e.g., RequestType: Loans, SubRequestType: Loan Application) along with confidence scores.
Example:
Text: I want to check my account balance.
Predicted RequestType: Account Services
Predicted SubRequestType: Balance Inquiry
Inspiration

The project was inspired by the need to automate customer support ticket categorization in banking systems. By classifying inquiries into hierarchical categories, the system can streamline routing, reduce response times, and improve service efficiency.

What It Does

Text Classification:
Uses ML.NET to classify text into RequestType (e.g., "Loans," "Account Services") and SubRequestType (e.g., "Loan Application," "Balance Inquiry").
Email Processing:
Extracts text from .eml files, including attachments (PDFs, Word documents, images).
OCR Integration:
Extracts text from images (Tesseract) and PDFs (iText).
Hierarchical Prediction:
Two ML models: One for primary (RequestType) and one for secondary (SubRequestType) classification.
How We Built It

Data Preparation:
Created a labeled dataset of banking inquiries (e.g., {"Text": "I need to reset my password", "RequestType": "Account Services", "SubRequestType": "Password Reset"}).
Machine Learning Pipeline:
Trained two multiclass classification models using SdcaMaximumEntropy (ML.NET).
Features: Text featurization (TF-IDF, n-grams).
Email and Attachment Processing:
Used MimeKit to parse emails and attachments.
Integrated Tesseract for OCR and iText for PDF text extraction.
Prediction:
Combined outputs of both models for hierarchical classification.
Challenges We Faced

Data Scarcity: Limited labeled data for training required careful augmentation.
Attachment Handling:
Complexities in processing diverse formats (images, PDFs, Word).
Memory management for large attachments.
Model Accuracy:
Fine-tuning featurization and hyperparameters to improve predictions.
COM Interop for Word:
Cleanup of COM objects (Marshal.ReleaseComObject) to avoid memory leaks.
How to Run

Prerequisites

.NET 6+ SDK.
Tesseract OCR (eng.traineddata in tessdata folder).
Microsoft Office (for Word Interop, if processing .docx files).
Steps

Train Models:
Uncomment CreateModelsAndSave() in Main() to generate and save ML models.
Predict:
Place .eml files in MailMessages folder.
Run PredictData() to classify emails and attachments.
Output:
Predictions are logged to the console with confidence scores.
Tech Stack

Component	Technology/Tool
Machine Learning	ML.NET
Email Processing	MimeKit
OCR	Tesseract
PDF Extraction	iText
Word Extraction	Microsoft Word Interop
Language	C# (.NET 6)