using System;
using System.Collections.Generic;

namespace Floatly.Models;

public partial class Queue
{
    public int Id { get; set; }

    public string? ExtId { get; set; }

    public string? Title { get; set; }

    public string? Music { get; set; }

    public string? Lyrics { get; set; }

    public string? Cover { get; set; }

    public string? Banner { get; set; }

    public string? MoviePath { get; set; }

    public string? HdmoviePath { get; set; }

    public string? UploadedBy { get; set; }

    public string? SongLength { get; set; }

    public string? PlayCount { get; set; }

    public string? ArtistId { get; set; }

    public string? ArtistName { get; set; }

    public string? ArtistCover { get; set; }

    public int? Status { get; set; }

    public DateTime? CreatedAt { get; set; }
}
