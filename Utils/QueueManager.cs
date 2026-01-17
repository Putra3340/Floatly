using Floatly.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

public static class QueueManager
{
    public enum QueueStatus
    {
        Previous,
        Current,
        Next,
        //NextGenerated // this means queue added randomly,can be from local or server side TODO
        // fuck it with security, just make proper media player
        // 17 January 2026
    }
    private static FloatlyClientContext CreateDb()
        => new FloatlyClientContext();

    public static async Task AddToQueue(Floatly.Models.Form.Song song)
    {
        using var db = CreateDb();

        var queueItem = MapSong(song, QueueStatus.Next);
        await db.Queue.AddAsync(queueItem);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        var count = await db.Queue.CountAsync();

        Debug.WriteLine("Queue count = " + count);
        Debug.WriteLine(db.Database.GetDbConnection().Database);
        Debug.WriteLine(db.Database.GetDbConnection().DataSource);

    }

    public static async Task WriteHistory(Floatly.Models.Form.Song song)
    {
        using var db = CreateDb();

        var oldCurrent = await db.Queue
            .Where(q => q.Status == (int)QueueStatus.Current)
            .ToListAsync();

        foreach (var q in oldCurrent)
            q.Status = (int)QueueStatus.Previous;

        await db.SaveChangesAsync();

        var queueItem = MapSong(song, QueueStatus.Current);
        await db.Queue.AddAsync(queueItem);
        await db.SaveChangesAsync();
    }

    public static async Task ClearNext()
    {
        using var db = CreateDb();

        var nextSongs = await db.Queue
            .Where(q => q.Status == (int)QueueStatus.Next)
            .ToListAsync();

        if (nextSongs.Count == 0)
            return;

        db.Queue.RemoveRange(nextSongs);
        await db.SaveChangesAsync();
    }

    public static async Task<Floatly.Models.Form.Song?> GetNextSong()
    {
        using var db = CreateDb();

        var song = await db.Queue
            .Where(q => q.Status == (int)QueueStatus.Next)
            .OrderBy(q => q.Id)
            .FirstOrDefaultAsync();

        if (song == null)
            return null;

        db.Queue.Remove(song);
        await db.SaveChangesAsync();

        return MapQueueToSong(song);
    }

    public static async Task<bool> IsThereNextSong()
    {
        using var db = CreateDb();
        return await db.Queue.AnyAsync(q => q.Status == (int)QueueStatus.Next);
    }

    public static async Task<Floatly.Models.Form.Song?> RestoreCurrent()
    {
        using var db = CreateDb();

        var song = await db.Queue
            .Where(q => q.Status == (int)QueueStatus.Current)
            .OrderByDescending(q => q.CreatedAt)
            .FirstOrDefaultAsync();

        return song == null ? null : MapQueueToSong(song);
    }

    // ---------- helpers ----------

    private static Floatly.Models.Queue MapSong(
        Floatly.Models.Form.Song song,
        QueueStatus status)
    {
        return new Floatly.Models.Queue
        {
            ExtId = song.Id,
            Title = song.Title,
            Music = song.Music,
            Lyrics = song.Lyrics,
            Cover = song.Cover,
            Banner = song.Banner,
            MoviePath = song.MoviePath,
            HdmoviePath = song.HDMoviePath,
            UploadedBy = song.UploadedBy,
            SongLength = song.SongLength,
            PlayCount = song.PlayCount,
            ArtistId = song.ArtistId,
            ArtistName = song.ArtistName,
            ArtistCover = song.ArtistCover,
            Status = (int)status,
            CreatedAt = DateTime.Now
        };
    }

    private static Floatly.Models.Form.Song MapQueueToSong(
        Floatly.Models.Queue q)
    {
        return new Floatly.Models.Form.Song
        {
            Id = q.ExtId,
            Title = q.Title,
            Music = q.Music,
            Lyrics = q.Lyrics,
            Cover = q.Cover,
            Banner = q.Banner,
            MoviePath = q.MoviePath,
            HDMoviePath = q.HdmoviePath,
            UploadedBy = q.UploadedBy,
            SongLength = q.SongLength,
            PlayCount = q.PlayCount,
            ArtistId = q.ArtistId,
            ArtistName = q.ArtistName,
            ArtistCover = q.ArtistCover
        };
    }
}