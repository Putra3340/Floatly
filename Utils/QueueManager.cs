using Floatly.Api;
using Floatly.Models.Database;
using Floatly.Models.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floatly.Utils
{
    public static class QueueManager
    {
        public static readonly FloatlyClientContext db = new FloatlyClientContext();
        public const int MaxQueueSize = 50;
        public enum QueueStatus
        {
            Previous,
            Current,
            Next,
            NextGenerated // this means queue added randomly,can be from local or server side TODO
        }

        public static async Task AddSongToQueue(Queue song)
        {
            if (song.Status == (int)QueueStatus.Current) // means play now and clear current
            {
                var currentSong = db.Queue.FirstOrDefault(q => q.Status == (int)QueueStatus.Current);
                if (currentSong != null)
                {
                    if (currentSong.Music == song.Music) // prevent duplicate
                        return;
                    currentSong.Status = (int)QueueStatus.Previous;
                }
            }
            db.Queue.Add(song);
            db.SaveChanges();
        }
        public static async Task<Queue> GetCurrentSong()
        {
            return db.Queue.FirstOrDefault(q => q.Status == (int)QueueStatus.Current);
        }
        public static async Task<Queue> PlayPrevious()
        {
            var currentSong = db.Queue.FirstOrDefault(q => q.Status == (int)QueueStatus.Current);
            if (currentSong != null)
            {
                currentSong.Status = (int)QueueStatus.Next;
            }
            var previousSong = db.Queue.OrderByDescending(q => q.Id).FirstOrDefault(q => q.Status == (int)QueueStatus.Previous);
            if (previousSong != null)
            {
                previousSong.Status = (int)QueueStatus.Current;
            }
            return previousSong;
        }
        public static async Task ClearQueue() // when startup ?
        {
            db.Queue.RemoveRange(db.Queue.ToList());
            db.SaveChanges();
        }
        public static async Task<List<Queue>> GetQueueList()
        {
            return db.Queue.OrderBy(q => q.Id).ToList();
        }
        public static async Task FetchGeneratedQueue()
        {
            List<Song> que = await ApiLibrary.GetNextQueue();
            foreach (var song in que)
            {
                Queue q = new Queue()
                {
                    Title = song.Title,
                    Artist = song.ArtistName,
                    ArtistId    = int.Parse(song.ArtistId),
                    Banner = song.Banner,
                    Cover = song.Cover,
                    SongLength = song.SongLength,
                    Lyrics = song.Lyrics,
                    Music = song.Music,
                    Status = (int)QueueStatus.NextGenerated,
                    CreatedAt = DateTime.Now
                };
                db.Queue.Add(q);
            }
        }
    }
}
