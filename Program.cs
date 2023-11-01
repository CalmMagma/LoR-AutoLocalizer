using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using LOR_DiceSystem;
using LOR_XML;
using Workshop;
using System.Linq;
using System.Reflection;


namespace LoR_AutoLocalize
{
    internal class Program
    {
        private static string h = "";

        private static NormalInvitation stageModInfo = new NormalInvitation();

        static void Main()
        {

            Console.WriteLine("Insert the language of the exported folder (en, jp, kr, cn, trcn, etc...): ");
            h = "./Localize/" + Console.ReadLine().ToLower();

            Directory.CreateDirectory(h);
            Console.WriteLine("Reading data XMLs...");
            
            if (File.Exists("StageModInfo.xml"))
            {
                stageModInfo = new XmlSerializer(typeof(NormalInvitation)).Deserialize(File.Open("StageModInfo.xml", FileMode.Open)) as NormalInvitation;
            } else
            {
                Console.WriteLine("StageModInfo.xml was not found! Please put the program in the mod's root directory.\nPress ENTER to close the program.");
                Console.ReadLine();
                return;
            }

            try
            {
                if (stageModInfo.fileInfo.dialogFile.isFileExist)
                {
                    var dialogData = new XmlSerializer(typeof(BattleDialogRoot)).Deserialize(File.OpenRead("." + stageModInfo.fileInfo.dialogFile.relativePath)) as BattleDialogRoot;
                    Directory.CreateDirectory(h + "/BattleDialogues");
                    FileStream file = File.Create(h + "/BattleDialogues/Dialogs.xml");
                    Console.WriteLine("> Localizing " + file.Name);

                    new XmlSerializer(typeof(BattleDialogRoot)).Serialize(file, dialogData);

                    Console.WriteLine("- Localized " + file.Name);
                    file.Close();
                } else Console.WriteLine("! Combat_Dialog.xml was not found (it will not be localized)");


                if (stageModInfo.fileInfo.combatPageFile.isFileExist)
                {
                    var cardData = new XmlSerializer(typeof(DiceCardXmlRoot)).Deserialize(File.OpenRead("." + stageModInfo.fileInfo.combatPageFile.relativePath)) as DiceCardXmlRoot;
                    Directory.CreateDirectory(h + "/BattlesCards");
                    var file = File.Create(h + "/BattlesCards/CardNames.xml");
                    Console.WriteLine("> Localizing " + file.Name);

                    var localizeCards = new BattleCardDescRoot() { cardDescList = new List<BattleCardDesc>() };
                    foreach (var card in cardData.cardXmlList)
                    {
                        localizeCards.cardDescList.Add(new BattleCardDesc() { cardID = card._id, cardName = card.workshopName });
                    }
                    new XmlSerializer(typeof(BattleCardDescRoot)).Serialize(file, localizeCards);

                    Console.WriteLine("- Localized " + file.Name);
                    file.Close();
                } else Console.WriteLine("! CardInfo.xml was not found (it will not be localized)");


                bool storyExists = stageModInfo.fileInfo.bookStoryFile.isFileExist;
                bool librarianEquipExists = stageModInfo.fileInfo.librarianEquipPage.isFileExist;
                bool enemyEquipExists = stageModInfo.fileInfo.enemyEquipPage.isFileExist;
                if (storyExists || librarianEquipExists || enemyEquipExists)
                {
                    var bookDescData = storyExists ? new XmlSerializer(typeof(BookDescRoot)).Deserialize(File.OpenRead("." + stageModInfo.fileInfo.bookStoryFile.relativePath)) as BookDescRoot : new BookDescRoot();
                    Directory.CreateDirectory(h + "/Books");
                    var file = File.Create(h + "/Books/BookDesc.xml");

                    Console.WriteLine("> Localizing " + file.Name);

                    if (librarianEquipExists)
                    {
                        var keypageData = new XmlSerializer(typeof(BookXmlRoot)).Deserialize(File.OpenRead("." + stageModInfo.fileInfo.librarianEquipPage.relativePath)) as BookXmlRoot;
                        foreach (var guy in keypageData.bookXmlList)
                        {
                            var feller = bookDescData.bookDescList.Find(x => x.bookID == guy._id);
                            if (feller != null)
                            {
                                feller.bookName = guy.InnerName;
                            }
                            else
                            {
                                feller = new BookDesc { bookID = guy._id, bookName = guy.InnerName, passives = new List<string>(), texts = new List<string>() };
                                bookDescData.bookDescList.Add(feller);
                            }
                        }
                    } else Console.WriteLine("! EquipPage_Librarian.xml not found- librarian key page names (if they exist) may not be localized properly.");

                    if (enemyEquipExists)
                    {
                        var keypageData = new XmlSerializer(typeof(BookXmlRoot)).Deserialize(File.OpenRead("." + stageModInfo.fileInfo.enemyEquipPage.relativePath)) as BookXmlRoot;
                        foreach (var guy in keypageData.bookXmlList)
                        {
                            var feller = bookDescData.bookDescList.Find(x => x.bookID == guy._id);
                            if (feller != null)
                            {
                                feller.bookName = guy.InnerName;
                            }
                            else
                            {
                                feller = new BookDesc { bookID = guy._id, bookName = guy.InnerName, passives = new List<string>(), texts = new List<string>() };
                                bookDescData.bookDescList.Add(feller);
                            }
                        }
                    } else Console.WriteLine("! EquipPage_Enemy.xml not found- enemy key page names (if they exist) may not be localized properly.");

                    new XmlSerializer(typeof(BookDescRoot)).Serialize(file, bookDescData);

                    Console.WriteLine("- Localized " + file.Name);
                    file.Close();
                } else Console.WriteLine("! BookStory.xml, EquipPage_Librarian.xml and EquipPage_Enemy.xml were all not found (key pages will not be localized)");


                if (stageModInfo.fileInfo.enemyUnitFile.isFileExist)
                {
                    Directory.CreateDirectory(h + "/CharactersName");
                    var enemyUnitData = new XmlSerializer(typeof(EnemyUnitClassRoot)).Deserialize(File.OpenRead("." + stageModInfo.fileInfo.enemyUnitFile.relativePath)) as EnemyUnitClassRoot;
                    var file = File.Create(h + "/CharactersName/Characters.xml");
                    Console.WriteLine("> Localizing " + file.Name);

                    var localizeChars = new CharactersNameRoot() { nameList = new List<CharacterName>() };
                    foreach (var character in enemyUnitData.list)
                    {
                        localizeChars.nameList.Add(new CharacterName() { ID = character._id, name = character.name });
                    }
                    new XmlSerializer(typeof(CharactersNameRoot)).Serialize(file, localizeChars);

                    Console.WriteLine("- Localized " + file.Name);
                    file.Close();
                } else Console.WriteLine("! EnemyUnitInfo.xml was not found (it will not be localized)");


                if (stageModInfo.fileInfo.dropBookFile.isFileExist)
                {
                    Directory.CreateDirectory(h + "/Dropbooks");
                    var dropbookData = new XmlSerializer(typeof(BookUseXmlRoot)).Deserialize(File.OpenRead("." + stageModInfo.fileInfo.dropBookFile.relativePath)) as BookUseXmlRoot;
                    var file = File.Create(h + "/Dropbooks/Dropbook.xml");
                    Console.WriteLine("> Localizing " + file.Name);

                    var localizeBooks = new CharactersNameRoot() { nameList = new List<CharacterName>() };
                    foreach (var book in dropbookData.bookXmlList)
                    {
                        localizeBooks.nameList.Add(new CharacterName() { ID = book._id, name = book.workshopName });
                    }
                    new XmlSerializer(typeof(CharactersNameRoot)).Serialize(file, localizeBooks);

                    Console.WriteLine("- Localized " + file.Name);
                    file.Close();
                } else Console.WriteLine("! Dropbook.xml was not found (it will not be localized)");


                if (stageModInfo.fileInfo.stageFile.isFileExist)
                {
                    Directory.CreateDirectory(h + "/StageName");
                    var stageData = new XmlSerializer(typeof(StageXmlRoot)).Deserialize(File.OpenRead("." + stageModInfo.fileInfo.stageFile.relativePath)) as StageXmlRoot;
                    var file = File.Create(h + "/StageName/Stages.xml");
                    Console.WriteLine("> Localizing " + file.Name);

                    var localizeStage = new CharactersNameRoot() { nameList = new List<CharacterName>() };
                    foreach (var stage in stageData.list)
                    {
                        localizeStage.nameList.Add(new CharacterName() { ID = stage._id, name = stage.stageName });
                    }
                    new XmlSerializer(typeof(CharactersNameRoot)).Serialize(file, localizeStage);

                    Console.WriteLine("- Localized " + file.Name);
                    file.Close();
                } else Console.WriteLine("! StageInfo.xml was not found (it will not be localized)");


                if (stageModInfo.fileInfo.passiveFile.isFileExist)
                {
                    Directory.CreateDirectory(h + "/PassiveDesc");
                    var passiveData = new XmlSerializer(typeof(PassiveXmlRoot)).Deserialize(File.OpenRead("." + stageModInfo.fileInfo.passiveFile.relativePath)) as PassiveXmlRoot;
                    var file = File.Create(h + "/PassiveDesc/Passives.xml");
                    Console.WriteLine("> Localizing " + file.Name);

                    var localizePassives = new PassiveDescRoot() { descList = new List<PassiveDesc>() };
                    foreach (var passive in passiveData.list)
                    {
                        localizePassives.descList.Add(new PassiveDesc() { _id = passive._id, desc = passive.desc, name = passive.name });
                    }
                    new XmlSerializer(typeof(PassiveDescRoot)).Serialize(file, localizePassives);

                    Console.WriteLine("- Localized " + file.Name);
                    file.Close();
                } else Console.WriteLine("! PassiveList.xml was not found (it will not be localized)");

            } catch (Exception e)
            {
                Console.WriteLine("There was an error while attempting to localize XMLs: \n" + e.Message + "\n\nPress ENTER to close the program.");
                Console.ReadLine();
            }

            try
            {
                List<string> doNotBotherOpeningTbh = new List<string>
                {
                    "0Harmony.dll",
                    "1SMotion-Loader.dll",
                    "CustomMapUtility.dll",
                    "Mono.Cecil.dll",
                    "Mono.Cecil.Pdb.dll",
                    "Mono.Cecil.Mdb.dll",
                    "Mono.Cecil.Rocks.dll",
                    "MonoMod.Common.dll",
                    "MonoMod.RuntimeDetour.dll",
                    "MonoMod.Utils.dll",
                    "NAudio.dll"
                };

                Directory.CreateDirectory(h + "/BattleCardAbilities");
                FileStream cardAbilities = File.Create(h + "/BattleCardAbilities/CardAbilities.xml");
                FileStream diceAbilities = File.Create(h + "/BattleCardAbilities/DiceAbilities.xml");
                var localizeCardAbilities = new BattleCardAbilityDescRoot() { cardDescList = new List<BattleCardAbilityDesc>() };
                var localizeDiceAbilities = new BattleCardAbilityDescRoot() { cardDescList = new List<BattleCardAbilityDesc>() };
                Console.WriteLine("> Localizing card/dice abilities");
                var assemblyList = Directory.GetFiles(Directory.GetCurrentDirectory() + "/Assemblies").ToList().FindAll(x => x.EndsWith(".dll"));

                foreach (var assembly in assemblyList)
                {
                    Assembly foile;
                    try
                    {
                        foile = Assembly.LoadFile(assembly);
                        if (foile == null || doNotBotherOpeningTbh.Contains(foile.FullName)) return;
                    
                        foreach (var type in foile.DefinedTypes)
                        {
                            if (type.Name.StartsWith("DiceCardAbility_") && type.IsSubclassOf(typeof(DiceCardAbilityBase)))
                            {
                                localizeDiceAbilities.cardDescList.Add(new BattleCardAbilityDesc
                                {
                                    desc = type.GetField("Desc", BindingFlags.Static | BindingFlags.Public) is FieldInfo field && field != null ? new List<string> { field.GetValue(null) as string } : new List<string>(),
                                    id = type.Name.Substring("DiceCardAbility_".Length)
                                });
                                Console.WriteLine($"- Localized dice ability '{type.Name.Substring("DiceCardAbility_".Length)}'");
                            }
                            else if (type.Name.StartsWith("DiceCardSelfAbility_") && type.IsSubclassOf(typeof(DiceCardSelfAbilityBase)))
                            {
                                localizeCardAbilities.cardDescList.Add(new BattleCardAbilityDesc
                                {
                                    desc = type.GetField("Desc", BindingFlags.Static | BindingFlags.Public) is FieldInfo field && field != null ? new List<string> { field.GetValue(null) as string } : new List<string>(),
                                    id = type.Name.Substring("DiceCardSelfAbility_".Length)
                                });
                                Console.WriteLine($"- Localized card ability '{type.Name.Substring("DiceCardSelfAbility_".Length)}'");
                            }
                        } 
                    }
                    catch (ReflectionTypeLoadException)
                    {
                    }
                }
                new XmlSerializer(typeof(BattleCardAbilityDescRoot)).Serialize(cardAbilities, localizeCardAbilities);
                new XmlSerializer(typeof(BattleCardAbilityDescRoot)).Serialize(diceAbilities, localizeDiceAbilities);
                cardAbilities.Close();
                diceAbilities.Close();
            }
            
            catch (Exception e)
            {
                Console.WriteLine("There was an error while attempting to localize assemblies: \n" + e.Message + "\n\nPress ENTER to close the program.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Finished auto-localization! Press ENTER to close this program...");
            Console.ReadLine();
        }
    }
}
