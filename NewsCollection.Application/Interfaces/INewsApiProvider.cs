using System;

namespace NewsCollection.Application.Interfaces;

public interface INewsApiProvider
{
    Task<List<(string Headline, string Summary, string Url, DateTime PublicationDate)>> GetTopHeadlinesAsync(string category);
}
