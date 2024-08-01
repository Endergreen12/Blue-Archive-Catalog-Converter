using Blue_Archive_Classes;
using MemoryPack;
using System.Text.Json;
using static Utils.Ask;
using static Utils.Utils;

DisplayHelpIfArgIsHelp(args, "Blue-Archive-Catalog-Converter [Catalog type] [Whether to convert json to bytes] [Input file path] [Output file path]" +
    "Example: Blue-Archive-Catalog-Converter MediaCatalog false .\\MediaCatalog.bytes .\\output\\MediaCatalog.json" +
    "         Blue-Archive-Catalog-Converter 0 true .\\MediaCatalog.json .\\output\\MediaCatalog.bytes" +
    "The catalogs that can be read and the types" + newLineStr + "that can be converted can be checked by starting the program without any arguments");

var specifiedCatalogType = CatalogType.MediaCatalog;
var jsonToBytes = false;
var inputFilePath = "";
var outputFilePath = "";

ParseOrAskENumValue<CatalogType>("Please enter the catalog you wish to convert" + newLineStr + "Please select from the catalog List above" + newLineStr +
    "You can select by number, but you can also type the name as it is",
    ref specifiedCatalogType, 0, args);
Console.WriteLine();

ParseOrAskBooleanValue("Convert json to bytes?(True/False)" + newLineStr + "For example, if you want to convert bytes to json, enter false here", ref jsonToBytes, 1, args);

Console.WriteLine();
inputFilePath = ParseOrAskAndValidPath(true, "Enter the path to " + GetTypedFileName(specifiedCatalogType, jsonToBytes));
Console.WriteLine();
outputFilePath = ParseStrFromArgOrAsk(3, "Specify the path to the output file, including the file name" + newLineStr +
    "If left blank, the output will be placed in the current directory with the default name", args);

var catalogType = typeof(MediaCatalog);
if(specifiedCatalogType == CatalogType.TableCatalog)
{
    catalogType = typeof(TableCatalog);
}

var outputPathArray = outputFilePath.Split(Path.DirectorySeparatorChar);
if(outputPathArray.Length > 1)
{
    outputPathArray = outputPathArray.SkipLast(1).ToArray();
    if(!Directory.Exists(Path.Combine(outputPathArray)))
    {
        Directory.CreateDirectory(Path.Combine(outputPathArray));
    }
}
ConvertCatalog(inputFilePath, outputFilePath, specifiedCatalogType, catalogType, jsonToBytes);

Console.WriteLine("Done" + newLineStr + "Press enter key to exit...");
Console.ReadLine();

string GetTypedFileName(CatalogType catalogType, bool isJson)
{
    return catalogType.ToString() + "." + (isJson ? "json" : "bytes");
}

void ConvertCatalog(string inputPath, string outputPath, CatalogType catalogType, Type type, bool convertJsonToBytes)
{
    if(convertJsonToBytes)
    {
        var catalog = JsonSerializer.Deserialize(File.ReadAllText(inputPath), type);
        File.WriteAllBytes(outputPath == "" ? GetTypedFileName(catalogType, !jsonToBytes) : outputPath, MemoryPackSerializer.Serialize(type, catalog));
    } else
    {
        var catalog = MemoryPackSerializer.Deserialize(type, File.ReadAllBytes(inputPath));
        File.WriteAllText(outputPath == "" ? GetTypedFileName(catalogType, !jsonToBytes) : outputPath, JsonSerializer.Serialize(catalog, new JsonSerializerOptions { WriteIndented = true }));
    }
}

enum CatalogType
{
    MediaCatalog,
    TableCatalog
}
