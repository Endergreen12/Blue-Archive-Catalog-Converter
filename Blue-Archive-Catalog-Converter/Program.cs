using Blue_Archive_Classes;
using MemoryPack;
using System.Collections;
using System.Text.Json;
using static Utils.Ask;

var newLineStr = Environment.NewLine;

if (args.Length >= 1 && args[0] == "--help")
{
    Console.WriteLine("Blue-Archive-Catalog-Converter [Catalog Type] [Convert Type] [Output File Path]");
    Console.WriteLine("Example: Blue-Archive-Catalog-Converter MediaCatalog Json .\\MediaCatalog.json");
    Console.WriteLine("         Blue-Archive-Catalog-Converter 0 1 .\\MediaCatalog.json");
    Console.WriteLine();
    Console.WriteLine("The catalogs that can be read and the types" + newLineStr + "that can be converted can be checked by starting the program without any arguments");
    return;
}

var specifiedCatalogType = CatalogType.MediaCatalog;
var specifiedConvertType = ConvertTargetType.Json;
var inputFilePath = "";
var outputFilePath = "";

var typedExtension = new Dictionary<ConvertTargetType, string>()
{
    {ConvertTargetType.MemoryPack, "bytes" },
    {ConvertTargetType.Json, "json" }
};

// i want to be able to do these processes with generic types
if (args.Length == 0 || args.Length >= 1 && !Enum.TryParse(args[0], out specifiedCatalogType))
{
    while (true)
    {
        Console.WriteLine("Catalog List:");
        foreach (var catalog in Enum.GetValues<CatalogType>())
        {
            Console.WriteLine((int)catalog + ": " + catalog); // "0: MediaCatalog"
        }
        Console.WriteLine();
        var specifiedCatalogTypeStr = AskStringForUser("Please enter the Catalog you wish to convert" + newLineStr + "Please select from the Catalog List above" + newLineStr + "You can select by number, but you can also type the name as it is");
        if (Enum.TryParse(specifiedCatalogTypeStr, out specifiedCatalogType) && (int)specifiedCatalogType < Enum.GetValues<CatalogType>().Length)
            break;

        Console.WriteLine("Parse to CatalogType failed" + newLineStr + "Please enter it again");
        Console.WriteLine();
    }
}

Console.WriteLine();

if (args.Length == 0 || args.Length >= 2 && !Enum.TryParse(args[1], out specifiedConvertType))
{
    while (true)
    {
        Console.WriteLine("Convert Type List:");
        foreach (var catalog in Enum.GetValues<ConvertTargetType>())
        {
            Console.WriteLine((int)catalog + ": " + catalog); // "0: MemoryPack"
        }
        Console.WriteLine();
        var specifiedConvertTypeStr = AskStringForUser("Specify what you want it to be converted to (MemoryPack is .bytes file)" + newLineStr + "For example, if the source file is bytes and you want to convert it to json, select Json here" + newLineStr + "Please select from the Convert Type List above" + newLineStr + "You can select by number, but you can also type the name as it is");
        if (Enum.TryParse(specifiedConvertTypeStr, out specifiedConvertType) && (int)specifiedConvertType < Enum.GetValues<ConvertTargetType>().Length)
            break;

        Console.WriteLine("Parse to ConvertTargetType failed" + newLineStr + "Please enter it again");
        Console.WriteLine();
    }
}

Console.WriteLine();
inputFilePath = AskAndValidPath(true, "Enter the path to " + GetTypedFileName(specifiedCatalogType, specifiedConvertType == ConvertTargetType.MemoryPack ? ConvertTargetType.Json : ConvertTargetType.MemoryPack), 2, args);
Console.WriteLine();
outputFilePath = ParseStrFromArgOrAsk(3, "Specify the path to the output file, including the file name" + newLineStr + "If left blank, the output will be placed in the current directory with the default name", args);

// i know that nested switch is suck but idk how to do this better
switch(specifiedCatalogType)
{
    case CatalogType.MediaCatalog:
        switch (specifiedConvertType)
        {
            case ConvertTargetType.MemoryPack:
                ConvertJsonToBytes<MediaCatalog>(inputFilePath, specifiedCatalogType, specifiedConvertType);
                break;

            case ConvertTargetType.Json:
                ConvertBytesToJson<MediaCatalog>(inputFilePath, specifiedCatalogType, specifiedConvertType);
                break;
        }
        break;

    case CatalogType.TableCatalog:
        switch (specifiedConvertType)
        {
            case ConvertTargetType.MemoryPack:
                ConvertJsonToBytes<TableCatalog>(inputFilePath, specifiedCatalogType, specifiedConvertType);
                break;

            case ConvertTargetType.Json:
                ConvertBytesToJson<TableCatalog>(inputFilePath, specifiedCatalogType, specifiedConvertType);
                break;
        }
        break;
}

Console.WriteLine("Done" + newLineStr + "Press enter key to exit...");
Console.ReadLine();

string GetTypedFileName(CatalogType catalogType, ConvertTargetType convertTargetType)
{
    return catalogType.ToString() + "." + typedExtension[convertTargetType];
}

void ConvertBytesToJson<SourceCatalogType>(string path, CatalogType catalogType, ConvertTargetType convertTargetType)
{
    var catalog = MemoryPackSerializer.Deserialize<SourceCatalogType>(File.ReadAllBytes(path));
    var options = new JsonSerializerOptions { WriteIndented = true };
    File.WriteAllText(outputFilePath == "" ? GetTypedFileName(catalogType, convertTargetType) : outputFilePath, JsonSerializer.Serialize(catalog, options));
}

void ConvertJsonToBytes<SourceCatalogType>(string path, CatalogType catalogType, ConvertTargetType convertTargetType)
{
    var catalog = JsonSerializer.Deserialize<SourceCatalogType>(File.ReadAllText(path));
    File.WriteAllBytes(outputFilePath == "" ? GetTypedFileName(catalogType, convertTargetType) : outputFilePath, MemoryPackSerializer.Serialize(catalog));
}

enum CatalogType
{
    MediaCatalog,
    TableCatalog
}

enum ConvertTargetType
{
    MemoryPack,
    Json
}