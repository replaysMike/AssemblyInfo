using MetadataExtractor;
using MetadataExtractor.Formats.Bmp;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.FileType;
using MetadataExtractor.Formats.Gif;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Png;
using MetadataExtractor.Formats.QuickTime;
using System.Linq;
using System.Text;

namespace AssemblyInfo
{
    public class MediaInspector
    {
        public AssemblyData InspectMedia(AssemblyData data, string filename)
        {
            var sb = new StringBuilder();
            var directories = ImageMetadataReader.ReadMetadata(filename);
            var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            var dateTime = subIfdDirectory?.GetDescription(ExifDirectoryBase.TagDateTime);
            var creditDirectory = directories.OfType<IptcDirectory>().FirstOrDefault();
            var credit = creditDirectory?.GetDescription(IptcDirectory.TagCredit);
            var types = directories.Select(x => x.GetType()).ToList();
            var movieHeaderDirectory = (MetadataExtractor.Directory)directories.OfType<QuickTimeMovieHeaderDirectory>().Where(x => x.Name == "QuickTime Movie Header").FirstOrDefault();
            var fileTypeDirectory = (MetadataExtractor.Directory)directories.OfType<FileTypeDirectory>().Where(x => x.Name == "File Type").FirstOrDefault();
            var codec = fileTypeDirectory?.Tags.Where(x => x.Name == "Detected File Type Name").Select(x => x.Description).FirstOrDefault();
            var durationTime = movieHeaderDirectory?.Tags.Where(x => x.Name == "Duration").Select(x => x.Description).FirstOrDefault();
            var createdOn = movieHeaderDirectory?.Tags.Where(x => x.Name == "Created").Select(x => x.Description).FirstOrDefault();
            var dimensionDirectory = (MetadataExtractor.Directory)directories.OfType<PngDirectory>().Where(x => x.Name == "PNG-IHDR").FirstOrDefault()
                ?? (MetadataExtractor.Directory)directories.OfType<JpegDirectory>().Where(x => x.Name == "JPEG").FirstOrDefault()
                ?? (MetadataExtractor.Directory)directories.OfType<GifHeaderDirectory>().Where(x => x.Name == "GIF Header").FirstOrDefault()
                ?? (MetadataExtractor.Directory)directories.OfType<BmpHeaderDirectory>().Where(x => x.Name == "BMP Header").FirstOrDefault()
                ?? (MetadataExtractor.Directory)directories.OfType<QuickTimeTrackHeaderDirectory>().Where(x => x.Name == "QuickTime Track Header").FirstOrDefault();
            var width = dimensionDirectory?.Tags.Where(x => x.Name == "Image Width").Select(x => x.Description).FirstOrDefault()
                ?? dimensionDirectory?.Tags.Where(x => x.Name == "Width").Select(x => x.Description).FirstOrDefault();
            var height = dimensionDirectory?.Tags.Where(x => x.Name == "Image Height").Select(x => x.Description).FirstOrDefault()
                ?? dimensionDirectory?.Tags.Where(x => x.Name == "Height").Select(x => x.Description).FirstOrDefault();
            var dimensions = $"{width} x {height}";
            var duration = $"{durationTime}";
            foreach (var directory in directories)
            {
                foreach (var tag in directory.Tags)
                {
                    sb.AppendLine($"[{directory.Name}] {tag.Name}={tag.Description}");
                }
            }
            data.Metadata = sb.ToString();
            data.InformationalVersion = dateTime?.ToString();
            data.Copyright = credit;
            data.ProductName = dimensions;
            data.FileDescription = duration;
            data.Description = createdOn;
            data.InformationalVersion = codec;
            return data;
        }
    }
}
