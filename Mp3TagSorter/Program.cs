using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Mp3TagSorter
{
    class Program
    {
        static void Main(string[] args)
        {

            //var tlib = TagLib.File.Create(@"D:\Users\adria\Music\Queen\Greatest Hits I\01 Bohemian Rhapsody.mp3");

            //var tag = tlib.Tag;


            //Process();
            try
            {
                Process();
            }
            catch (Mp3TagSorterException e)
            {
                System.Console.WriteLine("Mp3TagException " + e.Message);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.GetType().Name + " - " + e.Message + "\n------\nSTACK TRACE\n------\n" + e.StackTrace);
            }
        }
        static string target, destination;
        static bool findRecursive = false;

        static void Process()
        {

            GetDirectories();
            var files = GetFilesOf(target);
            var actions = ArrangeFiles(files);
            if (actions == null)
            {
                Console.WriteLine("Bye!");
                return;
            }
            var tasks = actions.Select<Action<string>, Action>(x => () => x.Invoke(destination)).ToList();
            
            

            //tasks.ForEach((action) => action());
            Task.WaitAll(tasks.Select(x => Task.Run(x)).ToArray());
            Console.Clear();
            Console.WriteLine("Completed, press any key to exit...");
            Console.ReadKey(true);
        }

        static List<Action<string>> ArrangeFiles(List<string> files) {

            var query = files
                .Select(x => new {Path = x, TagLib.File.Create(x).Tag})
                .Select(x => new {x.Path, x.Tag, x.Tag.Album, Artist = x.Tag.FirstAlbumArtist, x.Tag.Year})
                .GroupBy(x => x.Album)
                .GroupBy(x => x.First().Artist)
                .ToList();

            List<Node> artists = new List<Node>();
            List<Action<string>> actions = new List<Action<string>>();
            foreach (var artist in query)
            {
                Node artistNode = new Node
                {
                    Name = artist.Key
                };
                foreach (var album in artist)
                {
                    Node albumNode = new Node() { Name = $"[{album.First().Tag.Year}] {album.Key}" };
                    var sorted = album.OrderBy(x => x.Tag.Disc).ThenBy(x => x.Tag.Track);
                    foreach (var song in sorted)
                    {
                        albumNode.AddChildren(new Node() { Name = song.Tag.Track.ToString("00")+ ". " + song.Tag.Title });
                        actions.Add((target) =>
                        {
                            string artistfolder = Path.Combine(target,
                            artist.Key);
                            Array.ForEach(Path.GetInvalidPathChars(), (letter) => artistfolder = artistfolder.Replace("" + letter, ""));

                            Directory.CreateDirectory(artistfolder);
                            string albumFolderName = $"[{song.Year}] {album.Key}";
                            albumFolderName = String.Join(" - ", albumFolderName.Split(':').Select(x => x.Trim()).ToArray());
                            string albumfolder = Path.Combine(
                            artistfolder, albumFolderName
                            );

                            Array.ForEach(Path.GetInvalidPathChars(), (letter) => albumfolder = albumfolder.Replace("" + letter, ""));

                            Directory.CreateDirectory(albumfolder);

                            if (song.Tag.DiscCount > 1)
                            {
                                albumfolder = Path.Combine(albumfolder, "Disk " + song.Tag.Disc);
                                Directory.CreateDirectory(albumfolder);

                            }


                            string filename = (song.Tag.Track.ToString("00") + ". " + song.Tag.Title + " - " + song.Artist + Path.GetExtension(song.Path));

                            Array.ForEach(Path.GetInvalidFileNameChars(), (letter) => filename = filename.Replace("" + letter, ""));

                            string filepath = Path.Combine(albumfolder, filename);
                            Array.ForEach(Path.GetInvalidPathChars(), (letter) => filepath = filepath.Replace("" + letter, ""));
                            File.Copy(song.Path, filepath);
                            Console.WriteLine(target + " -> " + filepath);
                        }); 
                    }
                    artistNode.AddChildren(albumNode);
                }
                artists.Add(artistNode);
            }

            Node n = new Node { Name = "(destination folder)", Children = artists };

            n.PrintPretty();

            Console.Write("Is that ok? (Y/N)");
            bool ok = Console.ReadKey(true).Key == ConsoleKey.Y;

            return ok ? actions : null;
        }

        static List<string> GetFilesOf(string path, string extension = ".mp3") {
            string[] files = Directory.GetFiles(path);

            List<string> resultFiles = new List<string>();

            foreach (var file in files)
            {
                if (Path.GetExtension(file) == extension) {
                    resultFiles.Add(file);
                } 
            }

            if (findRecursive)
            {
                var directories = Directory.GetDirectories(path);
                Array.ForEach(directories, (dir) => resultFiles.AddRange(GetFilesOf(dir, extension)));
            }

            return resultFiles;
        }

        static void GetDirectories()
        {
            System.Console.Write("Write folder to organize: ");
            target = Console.ReadLine();
            if (!Directory.Exists(target))
            {
                throw new Mp3TagSorterException("Target folder dont exists");
            }
            System.Console.Write("Write destination folder: ");
            destination = Console.ReadLine();

            if (!Directory.Exists(destination))
            {
                try
                {
                    Path.GetFullPath(destination);
                }
                catch (System.Exception)
                {
                    throw new Mp3TagSorterException(destination + " not a valid path");
                }
                System.Console.WriteLine("Creating folder " + destination + "...");
                Directory.CreateDirectory(destination);
            }
            System.Console.Write("Search folders recursively? (Y/N): ");
            findRecursive = Console.ReadKey(true).Key == ConsoleKey.Y;



        }
    }

    class Node
    {
        public string Name { get; set; }
        public List<Node> Children { get; set; } = new List<Node>();

        public void AddChildren(Node n)
        {
            Children.Add(n);
        }

        public void PrintPretty(string indent = "", bool last = true)
        {
            Console.Write(indent);
            if (last)
            {
                Console.Write("+-");
                indent += "  ";
            }
            else
            {
                Console.Write("|-");
                indent += "| ";
            }
            Console.WriteLine(Name);

            if (last && Children.Count == 0)
            {
                Console.WriteLine(indent);
            }

            for (int i = 0; i < Children.Count; i++)
                Children[i].PrintPretty(indent, i == Children.Count - 1);
        }
    }

    [System.Serializable]
    public class Mp3TagSorterException : System.Exception
    {
        public Mp3TagSorterException() { }
        public Mp3TagSorterException(string message) : base(message) { }
        public Mp3TagSorterException(string message, System.Exception inner) : base(message, inner) { }
        protected Mp3TagSorterException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
