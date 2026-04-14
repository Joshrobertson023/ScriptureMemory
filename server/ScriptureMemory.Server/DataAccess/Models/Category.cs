using Pgvector;
using System;

namespace DataAccess.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Vector? Embedding { get; set; }
    public DateTime CreatedAt { get; set; }

    public string GetEmbeddingText()
    {
        return $"{Name} - {Description}";
    }
}



