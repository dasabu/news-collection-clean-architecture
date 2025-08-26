namespace NewsCollection.Domain.Entities;

public class CollectionArticle
{
    public int CollectionId { get; set; }
    public Collection? Collection { get; set; }
    public int ArticleId { get; set; }
    public Article? Article { get; set; }
    public bool IsDeleted { get; set; } = false; // for soft-delete
    public DateTime CreatedAt { get; set; } // for tracking time added into DB
}