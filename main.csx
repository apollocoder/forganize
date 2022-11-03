#r "nuget: MetadataExtractor, 2.7.2"

using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.QuickTime;

readonly string[] Month = {
	"Januar", "Februar", "März", "April",
	"Mai", "Juni", "Juli", "August",
	"September", "Oktober", "November", "Dezember"
};

void Move(string file, DateTime? date)
{
	var path = $"{date.Value.Year}/{Month[date.Value.Month - 1]}";
	System.IO.Directory.CreateDirectory(path);
	File.Move(file, $"{path}/{file}");
}

DateTime? GetDate<T>(
	string file,
	IReadOnlyList<Directory> directories,
	int tagType) where T : Directory
{
	DateTime? date = null;
	foreach (var dir in directories)
	{
		var exif = directories.OfType<T>().FirstOrDefault();
		date = exif?.GetDateTime(tagType);
		if (date.HasValue) break;
	}

	return date;
}

void ProcessJpegs()
{
	var files = System.IO.Directory.GetFiles(".", "*.jpeg");
	foreach (var file in files)
	{
		var directories = ImageMetadataReader.ReadMetadata(file);
		var date = GetDate<ExifSubIfdDirectory>(
			file, directories, ExifDirectoryBase.TagDateTimeOriginal);

		if (!date.HasValue) continue;
		Move(file, date);
	}
}

void ProcessMovs()
{
	var files = System.IO.Directory.GetFiles(".", "*.mov");
	foreach (var file in files)
	{
		var directories = new List<Directory>();
		using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
			directories.AddRange(QuickTimeMetadataReader.ReadMetadata(stream));

		var date = GetDate<QuickTimeMetadataHeaderDirectory>(
			file, directories, QuickTimeMetadataHeaderDirectory.TagCreationDate);

		if (!date.HasValue) continue;
		Move(file, date);
	}
}

// START
ProcessJpegs();
ProcessMovs();