namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore.Internal;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
            string result = ExportSongsAboveDuration(context, 4);
            Console.WriteLine(result);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder output = new StringBuilder();

            var albums = context
                .Albums
                .ToArray()
                .Where(a => a.ProducerId == producerId)
                .OrderByDescending(a => a.Price)
                .Select(a => new
                {
                    a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    ProducerName = a.Producer.Name,
                    Price = $"{a.Price:F2}",
                    Songs = a.Songs
                        .Select(s => new
                        {
                            s.Name,
                            Price = $"{s.Price:F2}",
                            Writer = s.Writer.Name
                        })
                        .OrderByDescending(s => s.Name)
                        .ThenBy(s => s.Writer)
                        .ToList()
                })
                .ToList();

            foreach (var a in albums)
            {
                output.AppendLine($"-AlbumName: {a.Name}");
                output.AppendLine($"-ReleaseDate: {a.ReleaseDate}");
                output.AppendLine($"-ProducerName: {a.ProducerName}");
                output.AppendLine("-Songs:");

                foreach (var s in a.Songs)
                {
                    output.AppendLine($"---#{a.Songs.IndexOf(s) + 1}");
                    output.AppendLine($"---SongName: {s.Name}");
                    output.AppendLine($"---Price: {s.Price}");
                    output.AppendLine($"---Writer: {s.Writer}");
                }

                output.AppendLine($"-AlbumPrice: {a.Price}");
            }

            return output.ToString().Trim();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder output = new StringBuilder();

            var songs = context
                .Songs
                .ToArray()
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    s.Name,
                    Performer = s.SongPerformers
                        .Select(sp => $"{sp.Performer.FirstName} {sp.Performer.LastName}")
                        .FirstOrDefault(),
                    Writer = s.Writer.Name,
                    Producer = s.Album
                        .Producer
                        .Name,
                    Duration=s.Duration
                        .ToString("c")
                })
                .OrderBy(s=>s.Name)
                .ThenBy(s=>s.Writer)
                .ThenBy(s=>s.Performer);

            foreach (var s in songs)
            {
                output.AppendLine($"-Song #{songs.IndexOf(s) + 1}");
                output.AppendLine($"---SongName: {s.Name}");
                output.AppendLine($"---Writer: {s.Writer}");
                output.AppendLine($"---Performer: {s.Performer}");
                output.AppendLine($"---AlbumProducer: {s.Producer}");
                output.AppendLine($"---Duration: {s.Duration}");
            }

            return output.ToString()
                .Trim();
        }
    }
}
