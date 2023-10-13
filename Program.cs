// Inspired by https://github.com/shalzuth/BG3HighlightEverything

using System.Xml;
using System.Collections;
using System.IO.Compression;
using LSLib.LS;
using System.Text.RegularExpressions;
using LSLib.LS.Enums;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;
using System.Runtime.InteropServices;

class HighlightEverythingRevised
{
    public static string modPackageName = "Highlight Everything Revised";
    public static string modFolderName = "HighlightEverythingRevised";
    public static string modOutputFilename = "HighlightEverythingRevised"; // Final filename excluding file extensions of LSX or LSF
    public static string modName = "Highlight Everything Revised";    
    public static string modDescription = "Highlight all possible items and objects";
    public static string modUUID = "c2b696cf-e2d3-4c21-a84c-a70b3a0338cf";
    public static string modAuthor = "Lu5ck";
    public static string publishedVersion = "0.1.0.0";
    public static string currentVersion = "0.3.1.7";
    public static bool debug = true;

    static void Main(string[] args)
    {
        List<string> excludeMapKeyList = new List<string>
        {
            "4cc75168-a81e-4a5c-85cd-1bab8d7bb641", // Helper_Invisible_A

            // Mindflyer Colony Act 2
            "73e3aed3-4188-4fd9-83ea-35599970a579", // S_COL_ElderBrainADs_RapidSpeaker1
            "d9b52ba9-ec8c-47b5-ae93-3d7952745b0c", // S_COL_ElderBrainADs_RapidSpeaker2
            "6ba9c33e-b2c7-452b-9673-bb77238a8b47", // S_COL_ElderBrainADs_RapidSpeaker3
            "5ea4bd09-1720-4d07-9f1a-6b0ea0d574ad", // S_COL_ElderBrainADs_RapidSpeaker4
            "b9abe21f-65ec-434f-a772-00a477fd2296", // S_COL_ElderBrainADs_RapidSpeaker5
            "908287b7-4eff-4fbf-a503-789e22fb3a1a", // S_COL_ElderBrainADs_RapidSpeaker6

            "de0000ef-9e44-b17b-5d45-4420d0251fb2", // BASE_STATIC_Helper_converted
            "603bdf09-f184-4a03-b24e-a43c20d96fde", // Wine Rack overlay
            "a7882d0e-2c3b-47b2-b08e-aea07ae6e72f", // LTS_DUN_Chandelier_Chain_A
            "c8aa8cdd-4e6b-4d5b-adc2-206227766054", // LTS_DUN_Chandelier_Chain_A_Gold
            "b442f5f2-28aa-4b83-a658-88f21d5134dc", // LTS_DUN_Hanging_Bowls_Chain_A
            "1bdf275a-10e1-a0aa-8716-ac1cb121fa19", // DEC_GEN_Statue_Ketheric_A_converted, statue near gyrmforge entrance waypoint
            "943726d7-cc04-4bc8-9f7b-8f8a536fbe10" // DEC_SCL_Shadow_Barrier_A, the statue at Reithwin Town palza
        };

        string gamePath = @"D:\Program Files (x86)\Steam\steamapps\common\Baldurs Gate 3\Data\";
        // We sort it by creation date, the patch files will have newer creation date thus loaded last
        string[] pakFilesList = Directory.GetFiles(gamePath).Select(path => new FileInfo(path)).OrderBy(fileInfo => fileInfo.CreationTime).Select(fileInfo => fileInfo.FullName).ToArray();

        // Value Key Pair that respect insertion order
        // To make sure when we loop through the files, the latest override will be loaded last
        var dicFilesList = new Dictionary<string, AbstractFileInfo>();
        foreach (string pakFile in pakFilesList)
        {
            
            // Skip all Texture pak files
            if (pakFile.Contains("Texture")) continue;
            var curPakFiles = new PackageReader(pakFile).Read();

            // All files of interests are in either RootTemplate or Items, running without this will make the script look through entire game paks
            foreach (var file in curPakFiles.Files.Where(f => f.Name.Contains("RootTemplate") || f.Name.Contains("Items")))
            {
                if (!file.Name.EndsWith(".lsf")) continue;

                var lsfStream = file.MakeStream();
                // Blank override file, used to invalid entire file
                if (lsfStream.Length == 0)
                {
                    if (dicFilesList.ContainsKey(file.Name)) dicFilesList.Remove(file.Name);
                    file.ReleaseStream();
                    continue;
                }
                using (var lsfReader = new LSFReader(lsfStream))
                {
                    var lsf = lsfReader.Read();
                    var tempRegions = lsf.Regions.FirstOrDefault(r => r.Key == "Templates");
                    if (tempRegions.Value == null) continue;
                    var gameObjects = tempRegions.Value.Children.Where(c => c.Key == "GameObjects");

                    foreach (var nodes in gameObjects)
                    {
                        bool toAdd = false;
                        // This is a collection of gameObjects, we loop each of them
                        for (int i = 0; i < nodes.Value.Count(); i++)
                        {
                            var node = nodes.Value[i];

                            // If it is not an item, we skip to next gameObject
                            var itemType = node.Attributes.First(f => f.Key == "Type").Value.Value;
                            if (itemType.ToString() != "item") continue;

                            // If it is a parent template, we add
                            var ParentTemplateId = node.Attributes.FirstOrDefault(f => f.Key == "ParentTemplateId");
                            if (ParentTemplateId.Key != null) if (ParentTemplateId.Value.Value.ToString().Trim() == "")  toAdd = true;

                            // If it contain tooltip, we need to remove it thus we add
                            var tooltip = node.Attributes.FirstOrDefault(f => f.Key == "Tooltip");
                            if (tooltip.Key != null) toAdd = true;

                            // If it is excluded map key, we need to disable tooltip thus we add
                            var mapKey = node.Attributes.FirstOrDefault(f => f.Key == "MapKey");
                            if (mapKey.Key != null)
                            {
                                foreach (string exMapKey in excludeMapKeyList)
                                {
                                    if (mapKey.Value.Value.ToString().Trim() == exMapKey) toAdd = true;
                                }
                            }
                            if (toAdd) break; // No need to check further as this file is needed
                        }
                        // Finally we add the file in if it contain what we need
                        if (toAdd)
                        {
                            if (dicFilesList.ContainsKey(file.Name)) dicFilesList.Remove(file.Name);
                            dicFilesList.Add(file.Name, file);
                        }
                    }
                }
                file.ReleaseStream();
            }
        }

        // Release variables for memory, we dealing with potential thousands of files so any bits count
        // Unless you got good computer
        pakFilesList = null;
        
        //Console.WriteLine(dicFilesList.Count());
        //foreach (var key in dicFilesList.Keys) Console.WriteLine(key);

        // Reminder: At this point, dicFilesList contains only lsx latest files with type item after all overridings etc
        // We will convert them to XML because I need to create a new XML containing only the affected items
        // During this loop, we also will extract the modified GameObjects
        var dicXMLNodes = new Dictionary<string, List<XmlNode>>();
        foreach (var key in dicFilesList.Keys)
        {
            var lsfStream = dicFilesList[key].MakeStream();
            MemoryStream memStream = new MemoryStream();
            var loadParams = ResourceLoadParameters.FromGameVersion(Game.BaldursGate3);
            Resource resource = ResourceUtils.LoadResource(lsfStream, ResourceFormat.LSF, loadParams);
            var conversionParams = ResourceConversionParameters.FromGameVersion(Game.BaldursGate3);
            var writer = new LSXWriter(memStream)
            {
                Version = conversionParams.LSX,
                PrettyPrint = conversionParams.PrettyPrint
            };
            conversionParams.ToSerializationSettings(writer.SerializationSettings);
            writer.Write(resource);
            XmlDocument curDoc = new XmlDocument();
            memStream.Position = 0;
            curDoc.Load(memStream);
            dicFilesList[key].ReleaseStream();
            memStream.Dispose();

            XmlNodeList gameObjects = curDoc.SelectNodes("/save/region/node/children/node");

            foreach (XmlNode gameObject in gameObjects)
            {
                // Make sure indeed is gameObject
                if (!((XmlElement)gameObject).GetAttribute("id").Equals("GameObjects")) continue;
                bool toAdd, isParent, isExcludedMapKey;
                toAdd = isParent = isExcludedMapKey = false;
                int itemPos = 0;
                for (int i = 0; i < gameObject.ChildNodes.Count; i++)
                {
                    if (((XmlElement)gameObject.ChildNodes[i]).GetAttribute("id").Trim().Equals("Type"))
                    {
                        if (!((XmlElement)gameObject.ChildNodes[i]).GetAttribute("value").Trim().Equals("item"))
                        {
                            toAdd = false;
                            break;
                        }
                        else itemPos = i;
                    }
                    if (((XmlElement)gameObject.ChildNodes[i]).GetAttribute("id").Trim().Equals("ParentTemplateId"))
                    {
                        if (((XmlElement)gameObject.ChildNodes[i]).GetAttribute("value").Trim().Equals(""))
                        {
                            isParent = true;
                            toAdd = true;
                        }
                    }
                    if (((XmlElement)gameObject.ChildNodes[i]).GetAttribute("id").Trim().Equals("MapKey"))
                    {
                        foreach (string exMapKey in excludeMapKeyList)
                        {
                            if (((XmlElement)gameObject.ChildNodes[i]).GetAttribute("value").Trim().Equals(exMapKey))
                            {
                                isExcludedMapKey = true;
                                toAdd = true;
                            }
                        }
                    }
                    // We do not need this GameObject
                    if (((XmlElement)gameObject.ChildNodes[i]).GetAttribute("id").Trim().Equals("Name"))
                    {
                        string name = ((XmlElement)gameObject.ChildNodes[i]).GetAttribute("value").Trim();
                        if (name.StartsWith("TimelineTemplate_") || name.StartsWith("CINE_") || name.StartsWith("DEBUG_") || name.StartsWith("TEMP_Cinematic_DONOTUSE_"))
                        {
                            toAdd = false;
                            break;
                        }
                    }
                    if (((XmlElement)gameObject.ChildNodes[i]).GetAttribute("id").Trim().Equals("Tooltip"))
                    {
                        toAdd = true;
                        gameObject.RemoveChild(gameObject.ChildNodes[i]);
                        i--;
                    }
                }

                if (toAdd)
                {
                    if (isParent)
                    {
                        XmlElement newTooltip = gameObject.OwnerDocument.CreateElement("attribute");
                        newTooltip.SetAttribute("id", "Tooltip");
                        newTooltip.SetAttribute("type", "uint8");
                        newTooltip.SetAttribute("value", isExcludedMapKey ? "0" : "2");
                        if (itemPos > 0) gameObject.InsertBefore(newTooltip, gameObject.ChildNodes[itemPos]);
                        else gameObject.AppendChild(newTooltip);
                    }
                    else if (isExcludedMapKey)
                    {
                        XmlElement newTooltip = gameObject.OwnerDocument.CreateElement("attribute");
                        newTooltip.SetAttribute("id", "Tooltip");
                        newTooltip.SetAttribute("type", "uint8");
                        newTooltip.SetAttribute("value", "0");
                        if (itemPos > 0) gameObject.InsertBefore(newTooltip, gameObject.ChildNodes[itemPos]);
                        else gameObject.AppendChild(newTooltip);
                    }
                    if (!dicXMLNodes.ContainsKey(key)) dicXMLNodes[key] = new List<XmlNode> {};
                    dicXMLNodes[key].Add(gameObject);
                }
            }
        }

        dicFilesList = null;

        //Console.WriteLine(dicXMLNodes.Count());
        //foreach (var key in dicXMLNodes.Keys) Console.WriteLine(key + ": " + dicXMLNodes[key].Count());

        // Put together the XML
        var dicXMLDocuments = new Dictionary<string, XmlDocument>();
        foreach (var key in dicXMLNodes.Keys)
        {
            XmlDocument outputDoc;
            XmlElement outChildrenElement;
            if (dicXMLDocuments.ContainsKey(getModPath(key)))
            {
                outputDoc = dicXMLDocuments[getModPath(key)];
                outChildrenElement = outputDoc.SelectSingleNode("/save/region/node/children") as XmlElement;
            }
            else
            {
                // Setup the structure for the XML
                outputDoc = new XmlDocument();
                XmlElement outRootElement = outputDoc.CreateElement("save");
                outputDoc.AppendChild(outRootElement);
                XmlElement outVersionElement = outputDoc.CreateElement("version");
                outVersionElement.SetAttribute("build", "0");
                outVersionElement.SetAttribute("lslib_meta", "v1,bswap_guids");
                outVersionElement.SetAttribute("major", "4");
                outVersionElement.SetAttribute("minor", "0");
                outVersionElement.SetAttribute("revision", "9");
                outRootElement.AppendChild(outVersionElement);
                XmlElement outRegionElement = outputDoc.CreateElement("region");
                outRegionElement.SetAttribute("id", "Templates");
                outRootElement.AppendChild(outRegionElement);
                XmlElement outNodeElement = outputDoc.CreateElement("node");
                outNodeElement.SetAttribute("id", "Templates");
                outRegionElement.AppendChild(outNodeElement);
                outChildrenElement = outputDoc.CreateElement("children");
                outNodeElement.AppendChild(outChildrenElement);
            }
            // Add all the GameObjects into it
            foreach (XmlNode node in dicXMLNodes[key]) outChildrenElement.AppendChild(outputDoc.ImportNode(node, true));
            dicXMLDocuments[getModPath(key)] = outputDoc;
        }

        dicXMLNodes = null;
        //Console.WriteLine(dicXMLDocuments.Count);
        //foreach (var key in dicXMLDocuments.Keys) Console.WriteLine(key);

        XmlWriterSettings xmlSettings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  "
        };

        if (Directory.Exists(modName)) Directory.Delete(modName, true);

        // Create meta.lsx
        Directory.CreateDirectory(modName + "/Mods/" + modFolderName);
        string metaTemplate = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><save><version major=\"4\" minor=\"0\" revision=\"6\" build=\"5\" /><region id=\"Config\"><node id=\"root\"><children><node id=\"Dependencies\"/><node id=\"ModuleInfo\"><attribute id=\"Author\" type=\"LSString\" value=\"\"/><attribute id=\"CharacterCreationLevelName\" type=\"FixedString\" value=\"\"/><attribute id=\"Description\" type=\"LSString\" value=\"\"/><attribute id=\"Folder\" type=\"LSString\" value=\"\"/><attribute id=\"GMTemplate\" type=\"FixedString\" value=\"\"/><attribute id=\"LobbyLevelName\" type=\"FixedString\" value=\"\"/><attribute id=\"MD5\" type=\"LSString\" value=\"\"/><attribute id=\"MainMenuBackgroundVideo\" type=\"FixedString\" value=\"\"/><attribute id=\"MenuLevelName\" type=\"FixedString\" value=\"\"/><attribute id=\"Name\" type=\"FixedString\" value=\"\"/><attribute id=\"NumPlayers\" type=\"uint8\" value=\"4\"/><attribute id=\"PhotoBooth\" type=\"FixedString\" value=\"\"/><attribute id=\"StartupLevelName\" type=\"FixedString\" value=\"\"/><attribute id=\"Tags\" type=\"LSString\" value=\"\"/><attribute id=\"Type\" type=\"FixedString\" value=\"Add-on\"/><attribute id=\"UUID\" type=\"FixedString\" value=\"\"/><attribute id=\"Version\" type=\"int64\" value=\"\"/><children><node id=\"PublishVersion\"><attribute id=\"Version\" type=\"int64\" value=\"\"/></node><node id=\"Scripts\"/><node id=\"TargetModes\"><children><node id=\"Target\"><attribute id=\"Object\" type=\"FixedString\" value=\"Story\"/></node></children></node></children></node></children></node></region></save>";
        XmlDocument metaDoc = new XmlDocument();
        metaDoc.LoadXml(metaTemplate);
        metaDoc.SelectSingleNode("//node[@id='ModuleInfo']/attribute[@id='Author']").Attributes["value"].Value = modAuthor;
        metaDoc.SelectSingleNode("//node[@id='ModuleInfo']/attribute[@id='Description']").Attributes["value"].Value = modDescription;
        metaDoc.SelectSingleNode("//node[@id='ModuleInfo']/attribute[@id='Folder']").Attributes["value"].Value = modFolderName;
        metaDoc.SelectSingleNode("//node[@id='ModuleInfo']/attribute[@id='Name']").Attributes["value"].Value = modName;
        metaDoc.SelectSingleNode("//node[@id='ModuleInfo']/attribute[@id='UUID']").Attributes["value"].Value = modUUID;
        metaDoc.SelectSingleNode("//node[@id='ModuleInfo']/attribute[@id='Version']").Attributes["value"].Value = calculateVersion(currentVersion).ToString();
        metaDoc.SelectSingleNode("//node[@id='ModuleInfo']/children/node[@id='PublishVersion']/attribute[@id='Version']").Attributes["value"].Value = calculateVersion(publishedVersion).ToString();

        using (XmlWriter writer = XmlWriter.Create(Path.GetFullPath(modName + "/Mods/" + modFolderName + "/meta.lsx"), xmlSettings)) metaDoc.Save(writer);

        if (debug)
        {
            if (Directory.Exists(modName + "_debug")) Directory.Delete(modName + "_debug", true);
            Directory.CreateDirectory(modName + "_debug/Mods/" + modFolderName);
            using (XmlWriter writer = XmlWriter.Create(Path.GetFullPath(modName + "_debug/Mods/" + modFolderName + "/meta.lsx"), xmlSettings)) metaDoc.Save(writer);
        }

        // Convert and save these files to temporary mod folders
        foreach (var key in dicXMLDocuments.Keys)
        {
            Directory.CreateDirectory(modName + "/" + key);
            MemoryStream memStream = new MemoryStream();
            dicXMLDocuments[key].Save(memStream);
            memStream.Position = 0;
            var finalloadParams = ResourceLoadParameters.FromGameVersion(Game.BaldursGate3);
            Resource finalResource = ResourceUtils.LoadResource(memStream, ResourceFormat.LSX, finalloadParams);
            var finalConversionParams = ResourceConversionParameters.FromGameVersion(Game.BaldursGate3);
            ResourceUtils.SaveResource(finalResource, Path.GetFullPath(modName + "/" + key + "/" + modOutputFilename + ".lsf"), finalConversionParams);

            if (debug)
            {
                Directory.CreateDirectory(modName + "_debug/" + key);
                using (XmlWriter writer = XmlWriter.Create(Path.GetFullPath(modName + "_debug/" + key + "/" + modOutputFilename + ".lsx"), xmlSettings))
                {
                    dicXMLDocuments[key].Save(writer);
                }
            }
        }

        // Package the temporary mod folder
        var pakMod = new Package();
        var pakModFiles = Directory.GetFiles(modName, "*", SearchOption.AllDirectories);
        foreach (var file in pakModFiles)
        {
            using (var fsFile = FilesystemFileInfo.CreateFromEntry(file, file.Replace(modName + "\\", "")))
                pakMod.Files.Add(fsFile);
        }
        // The temporary pak will be created at this script root folder
        using (var pakFile = new PackageWriter(pakMod, modFolderName + ".pak")) pakFile.Write();

        // Create info.json for the mod zip
        string jsonString = "{\"Mods\":[{\"Author\":\"\",\"Name\":\"\",\"Folder\":\"\",\"Version\":\"\",\"Description\":\"\",\"UUID\":\"\",\"Created\":\"\",\"Dependencies\":[],\"Group\":\"\"}],\"MD5\":\"\"}";
        jsonData json = JsonSerializer.Deserialize<jsonData>(jsonString);
        jsonMeta modmod = json.Mods[0];
        modmod.Author = modAuthor;
        modmod.Name = modName;
        modmod.Folder = modFolderName;
        modmod.Version = calculateVersion(currentVersion).ToString();
        modmod.Description = modDescription;
        modmod.UUID = modUUID;
        DateTime dateTime = DateTime.Now;
        string formattedDateTime = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.ffffffzzz");
        modmod.Created = formattedDateTime;
        json.MD5 = CalculateMD5(modFolderName + ".pak");
        string modifiedJsonString = JsonSerializer.Serialize(json);
        modifiedJsonString = modifiedJsonString.Replace(@"\u002B", "+");
        File.WriteAllText("info.json", modifiedJsonString);
        if (debug) File.WriteAllText("info_debug.json", modifiedJsonString);

        // Zip the mod
        if (File.Exists(modPackageName + ".zip")) File.Delete(modPackageName + ".zip");
        using (var zip = ZipFile.Open(modPackageName + ".zip", ZipArchiveMode.Create))
        {
            zip.CreateEntryFromFile(modFolderName + ".pak", modFolderName + ".pak");
            zip.CreateEntryFromFile("info.json", "info.json");
        }

        // Delete all the temporary files and folders
        if (File.Exists(modFolderName + ".pak")) File.Delete(modFolderName + ".pak");
        if (File.Exists("info.json")) File.Delete("info.json");
        if (Directory.Exists(modName)) Directory.Delete(modName, true);
    }

    public static string getModPath (string path)
    {
        string directoryPath = Path.GetDirectoryName(path);
        string[] parts = directoryPath.Split('\\');

        // Check if the second folder needs to be renamed
        if (parts.Length >= 2)
        {
            if (parts[1] == "GustavDev" || parts[1] == "Gustav" || parts[1] == "Shared" || parts[1] == "SharedDev")
            {
                parts[1] = modFolderName;
            }
        }

        string updatedPath = string.Join("/", parts);
        return updatedPath;
    }

    public static long calculateVersion (string strVersion)
    {
        string[] splitString = strVersion.Split('.');
        if (splitString.Length == 4)
        {
            long major = long.Parse(splitString[0]);
            long minor = long.Parse(splitString[1]);
            long revision = long.Parse(splitString[2]);
            long build = long.Parse(splitString[3]);

            return (major << 55) + (minor << 47) + (revision << 31) + build;
        }
        else
        {
            Console.WriteLine("Invalid Version");
            return (1 << 55) + (0 << 47) + (0 << 31) + 0;
        }
    }

    public class jsonMeta
    {
        public string Author { get; set; }
        public string Name { get; set; }
        public string Folder { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string UUID { get; set; }
        public string Created { get; set; }
        public string[] Dependencies { get; set; }
        public string Group { get; set; }
    }

    public class jsonData
    {
        public jsonMeta[] Mods { get; set; }
        public string MD5 { get; set; }
    }

    static string CalculateMD5(string filePath)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = md5.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}