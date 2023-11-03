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
using LSLib.LS.Story;

class HighlightEverythingRevised
{

    public static bool debug = true;
    public static string gamePath = @"D:\Program Files (x86)\Steam\steamapps\common\Baldurs Gate 3\Data\";
    static void Main(string[] args)
    {
        List<string> mainModExcludeMapKeyList = new List<string>
        {
            "4cc75168-a81e-4a5c-85cd-1bab8d7bb641", // Helper_Invisible_A, some labels in Grymforge
            "863f03cf-0a6c-41ae-99da-8c5dcd0a23b3", // Helper_Invisible_B, "NOT FOUND" labels in camps
            //"da1a2696-5ee1-4874-b17b-455adc0a9f10", // Helper_Invisible_D
            //"0a0c1e0f-60a9-493f-a98b-d761187c7d38", // Helper_Invisible_H
            //"8277a64b-9046-4d0b-9d89-834ff13526a4", // Helper_Invisible_BlackHole
            //"49cb4cda-ec8c-47d2-bb51-fef980da6a70", // Helper_Invisible_Summon
            //"90ec2f64-e124-474c-8b27-a56bba6f6c88", // BASE_PUZ
            //"06f96d65-0ee5-4ed5-a30a-92a3bfe3f708", // BASE_HELP_Item
            //"6ba37df5-ee1d-4066-9725-a1432991a3b0", // BASE_DEC_Small
            //"fc1a8257-5a1a-4dc5-9fe7-cbb46d242e76", // BASE_DEC_Medium
            //"115d2399-a009-4a63-82d0-f6c9ef6afdb0", // BASE_DEC_Medium_Immovable
            //"33a7b36e-6888-49ac-ada5-d77ad7e70dee", // BASE_DEC_Large
            //"5912447d-0772-43d2-a5f6-9d1b6d682e79", // BASE_DEC_Sittable
            //"f4ef6926-4f96-43fd-881a-4168297ad0fb", // BASE_BLD_Construction_Big
            //"de0000ef-9e44-b17b-5d45-4420d0251fb2", // BASE_STATIC_Helper_converted
            //"49aba79d-0be8-42f0-9302-3761b3527fa4", // BASE_CONT

            // Mindflyer Colony Act 2
            "73e3aed3-4188-4fd9-83ea-35599970a579", // S_COL_ElderBrainADs_RapidSpeaker1
            "d9b52ba9-ec8c-47b5-ae93-3d7952745b0c", // S_COL_ElderBrainADs_RapidSpeaker2
            "6ba9c33e-b2c7-452b-9673-bb77238a8b47", // S_COL_ElderBrainADs_RapidSpeaker3
            "5ea4bd09-1720-4d07-9f1a-6b0ea0d574ad", // S_COL_ElderBrainADs_RapidSpeaker4
            "b9abe21f-65ec-434f-a772-00a477fd2296", // S_COL_ElderBrainADs_RapidSpeaker5
            "908287b7-4eff-4fbf-a503-789e22fb3a1a", // S_COL_ElderBrainADs_RapidSpeaker6

            "603bdf09-f184-4a03-b24e-a43c20d96fde", // Wine Rack overlay
            "6dadcadc-ff9e-4bcd-8e50-4916264d3430", // BASE_DEC_BOOKSHELF
            //"16afba1f-58e1-46b7-9e6d-75e43ff0da41", // DEC_GEN_Kitchen_CupBoard_Empty_Common_A, "Wooden Shelf" label
            //"8175c653-3425-4b41-b046-6fc57b15ee09", // DEC_GEN_Kitchen_CupBoard_Empty_Common_B
            //"a92886d1-348b-49a9-b69f-c72bdc84b194", // DEC_GEN_Kitchen_CupBoard_Empty_Common_C
            //"1b7045f6-b8f7-4431-8b54-620e3e61cc6d", // DEC_GEN_Kitchen_CupBoard_Empty_Poor_A, "Wooden Shelf" label
            //"5ad8ecf0-cd91-4e95-b031-a794a5c71b31", // DEC_GEN_Kitchen_CupBoard_Empty_Poor_B
            //"09b1862b-6f24-43df-bf04-759e193a2870", // DEC_GEN_Kitchen_CupBoard_Empty_Poor_C
            "5e488d13-571e-40aa-9e1e-096e535ce221", // DEC_GEN_Market_Stall_A, "Tiered Shelves" label
            "49f01304-c744-476c-9144-a54237289c72", // DEC_Dungeon_Urn_Stone_A_Dynamic, "Stone Urn" label
            "a7882d0e-2c3b-47b2-b08e-aea07ae6e72f", // LTS_DUN_Chandelier_Chain_A
            "c8aa8cdd-4e6b-4d5b-adc2-206227766054", // LTS_DUN_Chandelier_Chain_A_Gold
            "b442f5f2-28aa-4b83-a658-88f21d5134dc", // LTS_DUN_Hanging_Bowls_Chain_A

            "943726d7-cc04-4bc8-9f7b-8f8a536fbe10", // DEC_SCL_Shadow_Barrier_A, the statue at Reithwin Town palza
            "ebb6d9ff-809c-450f-80f0-a90d3eb35280", // DEC_GEN_Statue_Shar_Huge_A_Plate_A, "Dais" label
            "d985d679-29e0-45f1-ba8b-58baebe5c99e", // BLD_Shar_Elevator_Platform_A, "Elevator" label at Gaunlet of Shar
            "729a095c-0524-4623-8a4b-1b86412c98d6", // BLD_Dungeon_SharTemple_Stone_Brick_Large_A, "Not Found" label, seems to be used as a visual block
            "d9b89f96-5ead-485d-aa73-b75d9d71d68e", // DEC_Brewery_Distillery_E, the distillery machine at Waning Moon

            // Goblin Camp
            "899736a3-984a-46fd-b54b-78bb55c42d80", // PUZ_GOB_Firebowl_A_Rope_12H_A, "Rope" that hang the lights
            "8074ac70-a8f5-43a8-a46e-c35cb15ce5c3", // PUZ_GOB_Firebowl_A_Rope_15H_A, "Rope" that hang the lights

            // Rosymorn Monastery
            "d7046d8f-9f23-457e-a0b8-e11a53727691", // BLD_CRE_Roof_Breakable_A_Base, a pillar at Rosymorn Monastery
            "cf2027d6-d51d-4aac-8907-73a218961096", // BLD_Lathander_Grating_C_4H_Portrait_A, Lathander secret chamber
            "5b3101eb-93ef-439e-a7d5-ad8996edca8a", // BLD_Lathander_Grating_C_4H_Portrait_B, Lathander secret chamber
            "e5beff0b-ff4c-42c8-8b51-3c87a5d59cd4", // BLD_Lathander_Grating_C_4H_Portrait_C, Lathander secret chamber
            "c7cfd415-aa94-4f78-8218-5a37c4993c2b", // PUZ_Lathander_Barrier_Generator_A, Lathander secret chamber 
            "e7457cb0-2062-410d-87ac-dcc6c12ee880", // PUZ_Lathander_Barrier_Generator_A_Hinges_A, Lathander secret chamber
            "e6277fd1-c9c9-4167-8760-ce77fa935978", // PUZ_Lathander_Barrier_Generator_A_Top_A, Lathander secret chamber
            "9dcd8c84-74d8-40f8-a9f8-078d87463435", // DEC_GEN_RoadSign_Milestone_A, "Milestone" label
            "80a5cfb4-f157-46a6-8c7d-d28ed41e9cee", // DEC_GEN_RoadSign_Milestone_B "Milestone" label
            "48a9ce05-5351-48a0-b6c4-e79ebbf2c357", // BLD_Lathander_Cable_Car_A, "Cable Car" label

            // "Statue" label, uninformative labels
            "1bdf275a-10e1-a0aa-8716-ac1cb121fa19", // DEC_GEN_Statue_Ketheric_A_converted, statue near gyrmforge entrance waypoint
            "8dbaa7a5-086d-072e-6d41-c62773dd8e27", // DEC_GEN_Statue_Guard_A_converted, the statue at Defiled Temple
            "e5d6d9c9-655e-e139-1190-dcce805aee1c", // DEC_GEN_Statue_King_A_converted
            "23d131c2-567f-b278-0d08-f5714050d8f2", // DEC_GEN_Statue_Wizard_A_converted
            "4aa70e55-f2e4-401c-a56f-49e58e679cac", // UND_StoneFigure
            "35249a14-25e8-4667-a3d7-b8ef5ed674d3", // DEC_GEN_Statue_Ketheric_A_Gold_Trim_A_Dynamic
            "ceebe440-fbe9-4b29-a8c4-f4e9a532cecd", //  DEC_GEN_Statue_Ketheric_B

            // "Tomb" label, uninformative labels
            "8eda0302-fa52-4397-b026-36d121f97ba9", // DEC_CEM_Tomb_Base_A_Shar, Gaunlet of Shar
            "9ab39146-bbe8-436b-bb91-af9475aaee9a", // DEC_CEM_Tomb_B_Base_A, Mausoleum
            "5ffcdfba-db7f-4d44-b9f1-24d480cd9318", // DEC_CEM_Tomb_B_Base_A_Broken_A
            "9f557cc8-a736-4df3-8688-4c8f87cad7dd", // DEC_CEM_Tomb_Base_A
            "44579102-e464-41ea-b56a-244677acd028", // DEC_CEM_Tomb_A_Base_A_Broken_A

            // "Sarcophagus" labels aka the tomb's lid will be left as it is as there are interactable ones

            // "Arcane Machinery Pipes" label, uninformative labels
            "73fd1af9-ba3e-4e9a-b99f-8cb2e227821b", // DEC_Underdark_WizardTower_Machinery_B
            "7862ad46-400d-4c8a-a5d0-9a9ca0ad6e48", // DEC_Underdark_WizardTower_Machinery_Pipes_Attachment_A_B_Off
            "6b3926b5-04bb-4ec8-89ff-7ce352be8b3f", // DEC_Underdark_WizardTower_Machinery_Pipes_Attachment_A_B_On
            "a79d5edb-a59f-4aec-a467-fded0be2ea6b", // DEC_Underdark_WizardTower_Machinery_Pipes_Attachment_A_B_On_Green
            "53df246b-2aa7-4b4d-bc45-615e7a5eb1ce", // DEC_Underdark_WizardTower_Machinery_Pipes_Attachment_A_E_Off
            "02f7abab-4867-47a2-ba60-a651b3b0fa3a", // DEC_Underdark_WizardTower_Machinery_Pipes_Attachment_A_E_On
            "2edd5cf2-7e08-4662-b370-f0321053ce93", // DEC_Underdark_WizardTower_Machinery_Pipes_Attachment_A_E_On_green
            "7d428a32-6ed1-4a83-9c9a-faa4008c96f8", // DEC_Underdark_WizardTower_Machinery_Pipes_GlassWindow_A_Off
            "59354793-f38f-4c59-be28-93de2d7e15a3", // DEC_Underdark_WizardTower_Machinery_Pipes_GlassWindow_A_On
            "854886b8-3380-4ef3-9996-d6b77ec90ff1", // DEC_Underdark_WizardTower_Machinery_Pipes_GlassWindow_A_On_Green
        };

        var mainModSettings = new Dictionary<string, string>
        {
            ["modPackageName"] = "Highlight Everything Revised",
            ["modFolderName"] = "HighlightEverythingRevised",
            ["modOutputFilename"] = "HighlightEverythingRevised",
            ["modName"] = "Highlight Everything Revised",
            ["modDescription"] = "Highlight all possible items and objects",
            ["modUUID"] = "c2b696cf-e2d3-4c21-a84c-a70b3a0338cf",
            ["modAuthor"] = "Lu5ck",
            ["publishedVersion"] = "0.1.0.0",
            ["currentVersion"] = "0.3.3.4"
        };

        List<string> disableRottenFoodExcludeMapKeyList = new List<string>
        {
            "5ab9094a-90ce-4981-afc7-5618093094b2", // CONS_GEN_Rotten_Food
        };

        var disableRottenFoodModSettings = new Dictionary<string, string>
        {
            ["modPackageName"] = "Highlight Everything Revised - Disable Rotten Food",
            ["modFolderName"] = "HighlightEverythingRevisedDisableRottenFood",
            ["modOutputFilename"] = "HighlightEverythingRevisedDisableRottenFood",
            ["modName"] = "Highlight Everything Revised - Disable Rotten Food",
            ["modDescription"] = "Disable Rotten Food Highlights",
            ["modUUID"] = "a798b86a-67ae-49c2-9c43-4fb9bd117b61",
            ["modAuthor"] = "Lu5ck",
            ["publishedVersion"] = "0.1.0.0",
            ["currentVersion"] = "0.1.0.2"
        };

        List<string> disableLuxuriesExcludeMapKeyList = new List<string>
        {
            "deb1c368-16c8-4dc4-8bb0-aaa1bd80cda2", // BASE_LOOT_Valuable
        };

        List<string> disableLuxuriesIncludeMapKeyList = new List<string>
        {
            "93380b9c-cf4f-413d-99a7-7e2897cdb94e", // BASE_LOOT_Gem
            "1a750a66-e5c2-40be-9f62-0a4bf3ddb403", // BASE_LOOT_Dye
            "8705b906-316d-4457-bc15-15a8ff568f67", // LOOT_GEN_AdamantineScrap_A
            "3d6e790a-6454-4926-a2b0-21286c8f67a2", // LOOT_GEN_AdamantineScrap_A_Piece
            "e2b52df4-c8f0-4c65-88c7-24339458a291", // UNI_PhilgravesMansion_CanopicJar_Brain
            "a05c2c63-23b4-4700-8a51-655053d832b5", // UNI_UND_WitheredSussurPetals
        };

        var disableLuxuriesModSettings = new Dictionary<string, string>
        {
            ["modPackageName"] = "Highlight Everything Revised - Disable Luxuries",
            ["modFolderName"] = "HighlightEverythingRevisedDisableLuxuries",
            ["modOutputFilename"] = "HighlightEverythingRevisedDisableLuxuries",
            ["modName"] = "Highlight Everything Revised - Disable Luxuries",
            ["modDescription"] = "Disable Luxuries Highlights",
            ["modUUID"] = "1011ec07-7be2-438d-aa3e-d093bea80181",
            ["modAuthor"] = "Lu5ck",
            ["publishedVersion"] = "0.1.0.0",
            ["currentVersion"] = "0.1.0.1"
        };

        List<string> disableKitchenwaresExcludeMapKeyList = new List<string>
        {
            //"98f13cd0-0069-466f-bb6e-5b8964b46e66", // DEC_GEN_Ink_Pot_Closed_A
            //"16610661-e160-4030-b06f-dc12c4284ba0", // DEC_GEN_Ink_Pot_Open_A
            //"ec151944-d911-477b-a620-f26defb3b727", // DEC_GEN_Ink_Pot_QuillAndPot_A

            "913341fe-1edc-4782-992f-606a3c3a493c", // CONS_Drink_Canteen_A
            "66ee4e64-a517-4570-8c37-dd7b42cab6ee", // CONS_GEN_Drink_Cup_Metal_A_Empty
            "5bf2bda3-613d-467a-9901-b468bb326856", // CONS_GEN_Drink_Cup_Metal_A_Flipped
            "9ab9c085-4d24-4b78-85ac-e16b19107ba7", // CONS_GEN_Drink_Mug_Metal_A_Empty
            "c57a5a9a-ec9c-4e03-a2ab-6a001fde925c", // DEC_GEN_Kitchen_Bucket_A
            "50d62849-db3d-43e3-ad39-601922cf7aec", // DEC_GEN_Kitchen_Pan_A
            "2804f278-9e4b-49c8-8fc3-3ebc64cf78a5", // DEC_GEN_Kitchen_Pan_B
            "3f2fc28f-a00e-460a-a5ed-23887173c9b6", // DEC_GEN_Kitchen_Pot_A
            "d40952f0-3e83-4161-8af8-646d11cd6f27", // DEC_GEN_Kitchen_Pot_B
            "f3a92079-ffe4-4c27-8ea8-d9fdbf5631fa", // DEC_GEN_Kitchen_Pot_C
            "676138d7-60ba-432b-8483-9665d1920007", // DEC_GEN_Kitchen_Pot_Cauldron_A
            "de2bb388-5863-4504-95aa-dd694f11f1a6", // DEC_GEN_Kitchen_Pot_Cauldron_B
            "2f9eb358-4cf3-49b0-97ab-2d6cabb9a2a5", // DEC_GEN_Kitchen_Pot_Cauldron_C
            "5d1731b3-1fd3-48db-bf48-eb5e57b4c1c9", // DEC_GEN_Kitchen_Cutlery_Glass_Poor_A
            "25f6866e-eaad-45a4-8b12-ff90ef1f8bd1", // DEC_GEN_Kitchen_Pot_Cauldron_Covered_A
            "b12303eb-6356-4f6f-98ea-3f11c5f5b3ce", // DEC_GEN_Kitchen_Pot_Cauldron_Covered_B
            "38caaf07-7dbc-4505-841b-15e53f80ca75", // DEC_GEN_Kitchen_Pot_Cauldron_Covered_C
            "d8356714-c76c-4fb3-a556-da69b6ed43bd", // DEC_GEN_Kitchen_Pot_Cauldron_Lid_A
            "08cd45a5-9a73-41ad-90a1-d24447b09a9", // DEC_GEN_Kitchen_Pot_Cauldron_Lid_B
            "eb31fae0-262e-4c27-9c39-93604cdac7cb", // DEC_GEN_Kitchen_Pot_Cauldron_Lid_C
            "77b5f8b1-e870-472d-84f9-429c8e1a4f34", // DEC_GEN_Kitchen_Cutlery_Plate_Poor_A
            "63aa1a80-4025-49e4-a9e2-ae52f7cfc2e4", // DEC_GEN_Kitchen_Pot_Covered_A
            "52197e68-26d6-4619-94dd-b23c31560913", // DEC_GEN_Kitchen_Pot_Covered_B
            "27f5723b-b495-4d58-8426-cb9e5d409ae1", // DEC_GEN_Kitchen_Pot_Covered_C
            "19ac9e34-6b6e-4433-9c6f-26cc2c12e7c9", // DEC_GEN_Kitchen_Pot_Lid_A
            "ee3cfe20-977a-48d4-88cc-ac9501580cd6", // DEC_GEN_Kitchen_CuttingBoard_A
            "278ec62a-d6c4-4870-8921-a482661fa18f", // DEC_GEN_Kitchen_Pot_Drum_A
            "cfa3fe45-b54d-4c52-aa5d-d79bc80b398e", // DEC_GEN_Kitchen_Pot_Drum_Covered_A
            "0343045a-9efc-4405-a0f9-b37b936f4b65", // DEC_GEN_Kitchen_Pot_Drum_Lid_A
            "b4f2f931-695d-46ae-96c0-8064232d0d4a", // DEC_GEN_Kitchen_Pot_MilkCan_A
            "9ac14527-37f5-4c4d-b3de-2c06b2e4b793", // DEC_GEN_Kitchen_Pot_MilkCan_B
            "3258852b-21eb-4242-80f5-ae9a6792ee4b", // DEC_GEN_Kitchen_Pot_MilkCan_Covered_A
            "b278764c-d7ab-4ccf-a75e-e235a6a56d87", // DEC_GEN_Kitchen_Pot_MilkCan_Covered_B
            "a8315769-3046-4711-b94f-4ca661f72924", // DEC_GEN_Kitchen_Pot_MilkCan_Lid_A
            "c96e0281-d3d5-48be-84f6-313fe9c91f9a", // DEC_GEN_Kitchen_Pot_MilkCan_Lid_B
            "18303ee3-884d-41ed-8bf9-3a1742f27a75", // DEC_GEN_KitchenInstrument_Bowl_A_Wood_A
            "ab081706-d6a4-4d6f-a148-8a774be32608", // DEC_GEN_KitchenInstrument_Cup_A_Porcelain_A
            "6aea565d-0132-4ea2-8bdf-53398a50b975", // DEC_GEN_KitchenInstrument_Cup_B_Porcelain_A
            "2add6f00-eb1c-4f86-bf7f-73e2c68ad5de", // DEC_GEN_KitchenInstrument_Cup_C_Metal_A
            "bf8738af-7637-45f3-ad2d-123d17def4f7", // DEC_GEN_KitchenInstrument_Cup_D_Metal_A
            "31d47753-7713-461f-a65b-da4339939cf2", // DEC_GEN_KitchenInstrument_Cup_E_Metal_A
            "b96500d2-3cef-4293-a02b-57ffc41d3a39", // DEC_GEN_KitchenInstrument_Horn_A_Bone_A
            "07f03f85-7eba-4f4e-ab8f-e51979a45ce7", // DEC_GEN_KitchenInstrument_Ladle_A_Copper_A
            "4b7c07ef-1f95-478d-859f-32f1ad98687b", // DEC_GEN_KitchenInstrument_MeatFork_A_Metal_A
            "95ed990a-f363-4eb1-908d-d48c353a3d62", // DEC_GEN_KitchenInstrument_Mortar_A_Stone_A
            "5f4cee38-81af-4879-bd8b-8ea75be5ba0b", // DEC_GEN_KitchenInstrument_Spoon_B_Wood_A
            "7af5ac38-ae9a-4181-a451-8fd5d4248de1", // DEC_GEN_KitchenInstrument_Strainer_A_Copper_A
            "750adfc9-e573-4bd4-b4b3-c6bef1aad1b6", // DEC_GEN_KitchenInstrument_Trivet_A_Cork_A
            "ccb0c26b-8502-450a-96ff-cfa22bba8150", // DEC_GEN_KitchenInstrument_Plate_A_Metal_A
            "c89c5461-12c6-441e-8643-7501fc61f8cf", // DEC_GEN_KitchenInstrument_Plate_B_Porcelain_A
            "768f2674-c274-4bd4-927a-66d060f154ec", // DEC_GEN_KitchenInstrument_Scale_A_Copper_A
            "95830031-4150-4a57-ade7-f80bcd9b45e2", // DEC_GEN_KitchenInstrument_Shears_A_Metal_A
            "b92c1b71-ecc3-45cc-b22d-f7a1ebc9e781", // DEC_GEN_KitchenInstrument_Spatula_A_Copper_A
            "1a466559-9518-4215-90e4-1f99264e028a", // LOOT_GEN_Kitchen_Bowl_Ceramic_Rich_A
            "adadcf9e-0a62-4abc-a9ee-c6a2b00a3394", // LOOT_GEN_Kitchen_Bowl_Ceramic_Rich_B
            "8c17a2e5-e742-4fac-a0d6-10289be28c72", // LOOT_GEN_Kitchen_Cutlery_Fork_Poor_A
            "e7fe4425-78c4-4270-a745-a2a96ec740cb", // LOOT_GEN_Kitchen_Cutlery_Fork_Rich_A
            "7d9a414b-2b5a-4672-87c5-65dd8cebe598", // LOOT_GEN_Kitchen_Cutlery_Knife_Poor_A
            "41f63854-1c50-454a-88c8-08dc696f7bc5", // LOOT_GEN_Kitchen_Cutlery_Knife_Rich_A
            "2e7a33f1-e1d3-4698-88f1-841e21d6ea2c", // LOOT_GEN_Kitchen_Cutlery_Plate_Rich_A
            "e83fd649-56ae-4204-93f5-55f3b112417c", // LOOT_GEN_Kitchen_Cutlery_Spoon_Poor_A
            "52028998-2e36-4c6f-801b-8fe202c5d0e8", // LOOT_GEN_Kitchen_Cutlery_Spoon_Rich_A
            "2c1541b5-18f9-4638-964b-6880e4f38bd6", // LOOT_GEN_Bowl_Wooden_A
            "3e10359a-3918-4341-83ee-069829b6d1f0", // LOOT_GEN_Pot_Ceramic_A
            "2308ae58-6ade-490c-89d9-312cf9c85cfc", // LOOT_GEN_Pot_Ceramic_B
            "f41f0ac9-afd2-442d-8d26-24ea0450f5cd", // LOOT_GEN_Pot_Ceramic_C
        };

        var disableKitchenwaresModSettings = new Dictionary<string, string>
        {
            ["modPackageName"] = "Highlight Everything Revised - Disable Kitchenwares",
            ["modFolderName"] = "HighlightEverythingRevisedDisableKitchenwares",
            ["modOutputFilename"] = "HighlightEverythingRevisedDisableKitchenwares",
            ["modName"] = "Highlight Everything Revised - Disable Kitchenwares",
            ["modDescription"] = "Disable Kitchenwares Highlights",
            ["modUUID"] = "3c78b66a-7741-4f44-af57-fde3db6c915d",
            ["modAuthor"] = "Lu5ck",
            ["publishedVersion"] = "0.1.0.0",
            ["currentVersion"] = "0.1.0.2"
        };

        List<string> disableBenchExcludeMapKeyList = new List<string>
        {
            "90d358e2-19f5-495a-844a-af81546c64ef", // BASE_DEC_BENCH
            "25dbdc1b-a0e7-46e0-80ed-9a0b2b4f003d", // BASE_DEC_Sittable_Immovable
        };

        var disableBenchModSettings = new Dictionary<string, string>
        {
            ["modPackageName"] = "Highlight Everything Revised - Disable Bench",
            ["modFolderName"] = "HighlightEverythingRevisedDisableBench",
            ["modOutputFilename"] = "HighlightEverythingRevisedDisableBench", // Final filename excluding file extensions of LSX or LSF
            ["modName"] = "Highlight Everything Revised - Disable Bench",
            ["modDescription"] = "Disable Bench Highlights",
            ["modUUID"] = "61462fba-ef18-4b3a-b537-8f0531d7a6d0",
            ["modAuthor"] = "Lu5ck",
            ["publishedVersion"] = "0.1.0.0",
            ["currentVersion"] = "0.1.0.1"
        };

        mainMod(mainModExcludeMapKeyList, mainModSettings);
        Console.WriteLine("Main Mod Done");
        disableMod(disableRottenFoodExcludeMapKeyList, new List<string>(), disableRottenFoodModSettings);
        Console.WriteLine("Disable Rotten Food Mod Done");
        disableMod(disableLuxuriesExcludeMapKeyList, disableLuxuriesIncludeMapKeyList, disableLuxuriesModSettings);
        Console.WriteLine("Disable Luxuries Mod Done");
        disableMod(disableKitchenwaresExcludeMapKeyList, new List<string>(), disableKitchenwaresModSettings);
        Console.WriteLine("Disable Kitchenwares Mod Done");
        disableMod(disableBenchExcludeMapKeyList, new List<string>(), disableBenchModSettings);
        Console.WriteLine("Disable Bench Mod Done");
    }

    public static void mainMod(List<string> excludeMapKeyList, Dictionary<string, string> modSettings)
    {
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
                        if (name.StartsWith("TimelineTemplate_") || name.StartsWith("CINE_") || name.StartsWith("DEBUG_") || name.StartsWith("TEMP_Cinematic_DONOTUSE_") || name.ToLower().EndsWith("_donotuse") || name.StartsWith("VFX_Script_"))
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
        finishMod(dicXMLNodes, modSettings);
    }

    public static void disableMod(List<string> excludeMapKeyList, List<string> includeMapKeyList, Dictionary<string, string> modSettings)
    {
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

                            // If it is excluded map key, we need to disable tooltip thus we add
                            var mapKey = node.Attributes.FirstOrDefault(f => f.Key == "MapKey");
                            if (mapKey.Key != null)
                            {
                                foreach (string exMapKey in excludeMapKeyList)
                                {
                                    if (mapKey.Value.Value.ToString().Trim() == exMapKey) toAdd = true;
                                }
                                foreach (string exMapKey in includeMapKeyList)
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
                bool toAdd, isExcludedMapKey, isIncludedMapKey;
                toAdd = isExcludedMapKey = isIncludedMapKey = false;
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
                        foreach (string exMapKey in includeMapKeyList)
                        {
                            if (((XmlElement)gameObject.ChildNodes[i]).GetAttribute("value").Trim().Equals(exMapKey))
                            {
                                isIncludedMapKey = true;
                                toAdd = true;
                            }
                        }
                    }
                    // We do not need this GameObject
                    if (((XmlElement)gameObject.ChildNodes[i]).GetAttribute("id").Trim().Equals("Name"))
                    {
                        string name = ((XmlElement)gameObject.ChildNodes[i]).GetAttribute("value").Trim();
                        if (name.StartsWith("TimelineTemplate_") || name.StartsWith("CINE_") || name.StartsWith("DEBUG_") || name.StartsWith("TEMP_Cinematic_DONOTUSE_") || name.ToLower().EndsWith("_donotuse") || name.StartsWith("VFX_Script_"))
                        {
                            toAdd = false;
                            break;
                        }
                    }
                    if (((XmlElement)gameObject.ChildNodes[i]).GetAttribute("id").Trim().Equals("Tooltip"))
                    {
                        gameObject.RemoveChild(gameObject.ChildNodes[i]);
                        i--;
                    }
                }

                if (toAdd)
                {
                    if (isExcludedMapKey)
                    {
                        XmlElement newTooltip = gameObject.OwnerDocument.CreateElement("attribute");
                        newTooltip.SetAttribute("id", "Tooltip");
                        newTooltip.SetAttribute("type", "uint8");
                        newTooltip.SetAttribute("value", "0");
                        if (itemPos > 0) gameObject.InsertBefore(newTooltip, gameObject.ChildNodes[itemPos]);
                        else gameObject.AppendChild(newTooltip);
                    } else if (isIncludedMapKey)
                    {
                        XmlElement newTooltip = gameObject.OwnerDocument.CreateElement("attribute");
                        newTooltip.SetAttribute("id", "Tooltip");
                        newTooltip.SetAttribute("type", "uint8");
                        newTooltip.SetAttribute("value", "2");
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
        finishMod(dicXMLNodes, modSettings);
        excludeMapKeyList = null;
        includeMapKeyList = null;
        modSettings = null;
    }

    public static void finishMod (Dictionary<string, List<XmlNode>> dicXMLNodes, Dictionary<string, string> modSettings)
    {
        // Put together the XML
        var dicXMLDocuments = new Dictionary<string, XmlDocument>();
        foreach (var key in dicXMLNodes.Keys)
        {
            XmlDocument outputDoc;
            XmlElement outChildrenElement;
            if (dicXMLDocuments.ContainsKey(getModPath(key, modSettings["modFolderName"])))
            {
                outputDoc = dicXMLDocuments[getModPath(key, modSettings["modFolderName"])];
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
            dicXMLDocuments[getModPath(key, modSettings["modFolderName"])] = outputDoc;
        }

        dicXMLNodes = null;
        //Console.WriteLine(dicXMLDocuments.Count);
        //foreach (var key in dicXMLDocuments.Keys) Console.WriteLine(key);

        XmlWriterSettings xmlSettings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  "
        };
        
        if (Directory.Exists(modSettings["modName"])) Directory.Delete(modSettings["modName"], true);

        // Create meta.lsx
        Directory.CreateDirectory(modSettings["modName"] + "/Mods/" + modSettings["modFolderName"]);
        string metaTemplate = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><save><version major=\"4\" minor=\"0\" revision=\"6\" build=\"5\" /><region id=\"Config\"><node id=\"root\"><children><node id=\"Dependencies\"/><node id=\"ModuleInfo\"><attribute id=\"Author\" type=\"LSString\" value=\"\"/><attribute id=\"CharacterCreationLevelName\" type=\"FixedString\" value=\"\"/><attribute id=\"Description\" type=\"LSString\" value=\"\"/><attribute id=\"Folder\" type=\"LSString\" value=\"\"/><attribute id=\"GMTemplate\" type=\"FixedString\" value=\"\"/><attribute id=\"LobbyLevelName\" type=\"FixedString\" value=\"\"/><attribute id=\"MD5\" type=\"LSString\" value=\"\"/><attribute id=\"MainMenuBackgroundVideo\" type=\"FixedString\" value=\"\"/><attribute id=\"MenuLevelName\" type=\"FixedString\" value=\"\"/><attribute id=\"Name\" type=\"FixedString\" value=\"\"/><attribute id=\"NumPlayers\" type=\"uint8\" value=\"4\"/><attribute id=\"PhotoBooth\" type=\"FixedString\" value=\"\"/><attribute id=\"StartupLevelName\" type=\"FixedString\" value=\"\"/><attribute id=\"Tags\" type=\"LSString\" value=\"\"/><attribute id=\"Type\" type=\"FixedString\" value=\"Add-on\"/><attribute id=\"UUID\" type=\"FixedString\" value=\"\"/><attribute id=\"Version\" type=\"int64\" value=\"\"/><children><node id=\"PublishVersion\"><attribute id=\"Version\" type=\"int64\" value=\"\"/></node><node id=\"Scripts\"/><node id=\"TargetModes\"><children><node id=\"Target\"><attribute id=\"Object\" type=\"FixedString\" value=\"Story\"/></node></children></node></children></node></children></node></region></save>";
        XmlDocument metaDoc = new XmlDocument();
        metaDoc.LoadXml(metaTemplate);
        metaDoc.SelectSingleNode("//node[@id='ModuleInfo']/attribute[@id='Author']").Attributes["value"].Value = modSettings["modAuthor"];
        metaDoc.SelectSingleNode("//node[@id='ModuleInfo']/attribute[@id='Description']").Attributes["value"].Value = modSettings["modDescription"];
        metaDoc.SelectSingleNode("//node[@id='ModuleInfo']/attribute[@id='Folder']").Attributes["value"].Value = modSettings["modFolderName"];
        metaDoc.SelectSingleNode("//node[@id='ModuleInfo']/attribute[@id='Name']").Attributes["value"].Value = modSettings["modName"];
        metaDoc.SelectSingleNode("//node[@id='ModuleInfo']/attribute[@id='UUID']").Attributes["value"].Value = modSettings["modUUID"];
        metaDoc.SelectSingleNode("//node[@id='ModuleInfo']/attribute[@id='Version']").Attributes["value"].Value = calculateVersion(modSettings["currentVersion"]).ToString();
        metaDoc.SelectSingleNode("//node[@id='ModuleInfo']/children/node[@id='PublishVersion']/attribute[@id='Version']").Attributes["value"].Value = calculateVersion(modSettings["publishedVersion"]).ToString();

        using (XmlWriter writer = XmlWriter.Create(Path.GetFullPath(modSettings["modName"] + "/Mods/" + modSettings["modFolderName"] + "/meta.lsx"), xmlSettings)) metaDoc.Save(writer);

        if (debug)
        {
            if (Directory.Exists(modSettings["modName"] + "_debug")) Directory.Delete(modSettings["modName"] + "_debug", true);
            Directory.CreateDirectory(modSettings["modName"] + "_debug/Mods/" + modSettings["modFolderName"]);
            using (XmlWriter writer = XmlWriter.Create(Path.GetFullPath(modSettings["modName"] + "_debug/Mods/" + modSettings["modFolderName"] + "/meta.lsx"), xmlSettings)) metaDoc.Save(writer);
        }

        // Convert and save these files to temporary mod folders
        foreach (var key in dicXMLDocuments.Keys)
        {
            Directory.CreateDirectory(modSettings["modName"] + "/" + key);
            MemoryStream memStream = new MemoryStream();
            dicXMLDocuments[key].Save(memStream);
            memStream.Position = 0;
            var finalloadParams = ResourceLoadParameters.FromGameVersion(Game.BaldursGate3);
            Resource finalResource = ResourceUtils.LoadResource(memStream, ResourceFormat.LSX, finalloadParams);
            var finalConversionParams = ResourceConversionParameters.FromGameVersion(Game.BaldursGate3);
            ResourceUtils.SaveResource(finalResource, Path.GetFullPath(modSettings["modName"] + "/" + key + "/" + modSettings["modOutputFilename"]+ ".lsf"), finalConversionParams);

            if (debug)
            {
                Directory.CreateDirectory(modSettings["modName"] + "_debug/" + key);
                using (XmlWriter writer = XmlWriter.Create(Path.GetFullPath(modSettings["modName"] + "_debug/" + key + "/" + modSettings["modOutputFilename"] + ".lsx"), xmlSettings))
                {
                    dicXMLDocuments[key].Save(writer);
                }
            }
        }

        // Package the temporary mod folder
        var pakMod = new Package();
        var pakModFiles = Directory.GetFiles(modSettings["modName"], "*", SearchOption.AllDirectories);
        foreach (var file in pakModFiles)
        {
            using (var fsFile = FilesystemFileInfo.CreateFromEntry(file, file.Replace(modSettings["modName"] + "\\", "")))
                pakMod.Files.Add(fsFile);
        }
        // The temporary pak will be created at this script root folder
        using (var pakFile = new PackageWriter(pakMod, modSettings["modFolderName"] + ".pak")) pakFile.Write();

        // Create info.json for the mod zip
        string jsonString = "{\"Mods\":[{\"Author\":\"\",\"Name\":\"\",\"Folder\":\"\",\"Version\":\"\",\"Description\":\"\",\"UUID\":\"\",\"Created\":\"\",\"Dependencies\":[],\"Group\":\"\"}],\"MD5\":\"\"}";
        jsonData json = JsonSerializer.Deserialize<jsonData>(jsonString);
        jsonMeta modmod = json.Mods[0];
        modmod.Author = modSettings["modAuthor"];
        modmod.Name = modSettings["modName"];
        modmod.Folder = modSettings["modFolderName"];
        modmod.Version = calculateVersion(modSettings["currentVersion"]).ToString();
        modmod.Description = modSettings["modDescription"];
        modmod.UUID = modSettings["modUUID"];
        DateTime dateTime = DateTime.Now;
        string formattedDateTime = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.ffffffzzz");
        modmod.Created = formattedDateTime;
        json.MD5 = CalculateMD5(modSettings["modFolderName"] + ".pak");
        string modifiedJsonString = JsonSerializer.Serialize(json);
        modifiedJsonString = modifiedJsonString.Replace(@"\u002B", "+");
        File.WriteAllText("info.json", modifiedJsonString);
        if (debug) File.WriteAllText("info_debug.json", modifiedJsonString);

        // Zip the mod
        if (File.Exists(modSettings["modPackageName"] + ".zip")) File.Delete(modSettings["modPackageName"] + ".zip");
        using (var zip = ZipFile.Open(modSettings["modPackageName"] + ".zip", ZipArchiveMode.Create))
        {
            zip.CreateEntryFromFile(modSettings["modFolderName"] + ".pak", modSettings["modFolderName"] + ".pak");
            zip.CreateEntryFromFile("info.json", "info.json");
        }

        // Delete all the temporary files and folders
        if (File.Exists(modSettings["modFolderName"] + ".pak")) File.Delete(modSettings["modFolderName"] + ".pak");
        if (File.Exists("info.json")) File.Delete("info.json");
        if (Directory.Exists(modSettings["modName"])) Directory.Delete(modSettings["modName"], true);
        dicXMLDocuments = null;
        modSettings = null;
    }

    public static string getModPath (string path, string modFolderName)
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