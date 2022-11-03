#r "nuget: MetadataExtractor, 2.7.2"

using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

readonly string[] Month = {
	"Januar", "Februar", "März", "April",
	"Mai", "Juni", "Juli", "August",
	"September", "Oktober", "November", "Dezember"
};

var files = System.IO.Directory.GetFiles(".", "*.jpeg").ToList();
files.AddRange(System.IO.Directory.GetFiles(".", "*.jpg"));
files.AddRange(System.IO.Directory.GetFiles(".", "*.mov"));

foreach (var file in files)
{
	var directories = ImageMetadataReader.ReadMetadata(file);
	foreach (var dir in directories)
		foreach (var tag in dir.Tags)
			Console.WriteLine($"{dir.Name} - {tag.Name} = {tag.Description}");
}