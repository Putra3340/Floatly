using System;
using System.Collections.Generic;

namespace Floatly.Models.Database;

public partial class DownloadedSong
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Artist { get; set; }

    public int? ArtistId { get; set; }

    public string ArtistBio { get; set; } = null!;

    public string? ArtistCover { get; set; }

    public string? Music { get; set; }

    public string? Lyrics { get; set; }

    public string? Cover { get; set; }

    public string? Banner { get; set; }

    public string? SongLength { get; set; }

    public DateTime? CreatedAt { get; set; }
}
