﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dotnet5BackendProject.Domain;

namespace dotnet5BackendProject.Services
{
    public interface IDocumentService
    {
        Task<List<Document>> GetDocumentsAsync();

        Task<Document> GetDocumentByIdAsync(Guid documentId);

        Task<bool> CreateDocumentAsync(Document document);

        Task<bool> UpdateDocumentAsync(Document documentToUpdate);

        Task<bool> DeleteDocumentAsync(Guid documentId);
        
        Task<bool> UserOwnsDocumentAsync(Guid documentId, string userId);
    }
}