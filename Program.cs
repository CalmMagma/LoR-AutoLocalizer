using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using LOR_DiceSystem;
using LOR_XML;


namespace LoR_AutoLocalize
{
    internal class Program
    {
        private static string h = "";

        static void Main()
        {

            Console.WriteLine("Insert the language of the exported folder (en, jp, kr, cn, trcn, etc...): ");
            h = Console.ReadLine().ToLower();

            Directory.CreateDirectory(h);
            Console.WriteLine("Reading data XMLs...");

            try
            {
                if (File.Exists("Combat_Dialog.xml"))
                {
                    var dialogData = new XmlSerializer(typeof(BattleDialogRoot)).Deserialize(File.OpenRead("Combat_Dialog.xml")) as BattleDialogRoot;
                    Directory.CreateDirectory(h + "/BattleDialogues");
                    FileStream file = File.Create(h + "/BattleDialogues/Dialogs.xml");
                    Console.WriteLine("> Localizing " + file.Name);

                    new XmlSerializer(typeof(BattleDialogRoot)).Serialize(file, dialogData);

                    Console.WriteLine("- Localized " + file.Name);
                    file.Close();
                } else Console.WriteLine("! Combat_Dialog.xml was not found (it will not be localized)");


                if (File.Exists("CardInfo.xml"))
                {
                    var cardData = new XmlSerializer(typeof(DiceCardXmlRoot)).Deserialize(File.OpenRead("CardInfo.xml")) as DiceCardXmlRoot;
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


                bool storyExists = File.Exists("BookStory.xml");
                bool librarianEquipExists = File.Exists("EquipPage_Librarian.xml");
                bool enemyEquipExists = File.Exists("EquipPage_Enemy.xml");
                if (storyExists || librarianEquipExists || enemyEquipExists)
                {
                    var bookDescData = storyExists ? new XmlSerializer(typeof(BookDescRoot)).Deserialize(File.OpenRead("BookStory.xml")) as BookDescRoot : new BookDescRoot();
                    Directory.CreateDirectory(h + "/Books");
                    var file = File.Create(h + "/Books/BookDesc.xml");

                    Console.WriteLine("> Localizing " + file.Name);

                    if (librarianEquipExists)
                    {
                        var keypageData = new XmlSerializer(typeof(BookXmlRoot)).Deserialize(File.OpenRead("EquipPage_Librarian.xml")) as BookXmlRoot;
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
                        var keypageData = new XmlSerializer(typeof(BookXmlRoot)).Deserialize(File.OpenRead("EquipPage_Enemy.xml")) as BookXmlRoot;
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


                if (File.Exists("EnemyUnitInfo.xml"))
                {
                    Directory.CreateDirectory(h + "/CharacterName");
                    var enemyUnitData = new XmlSerializer(typeof(EnemyUnitClassRoot)).Deserialize(File.OpenRead("EnemyUnitInfo.xml")) as EnemyUnitClassRoot;
                    var file = File.Create(h + "/CharacterName/Characters.xml");
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


                if (File.Exists("Dropbook.xml"))
                {
                    Directory.CreateDirectory(h + "/Dropbooks");
                    var dropbookData = new XmlSerializer(typeof(BookUseXmlRoot)).Deserialize(File.OpenRead("Dropbook.xml")) as BookUseXmlRoot;
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


                if (File.Exists("StageInfo.xml"))
                {
                    Directory.CreateDirectory(h + "/StageName");
                    var stageData = new XmlSerializer(typeof(StageXmlRoot)).Deserialize(File.OpenRead("StageInfo.xml")) as StageXmlRoot;
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


                if (File.Exists("PassiveList.xml"))
                {
                    Directory.CreateDirectory(h + "/PassiveDesc");
                    var passiveData = new XmlSerializer(typeof(PassiveXmlRoot)).Deserialize(File.OpenRead("PassiveList.xml")) as PassiveXmlRoot;
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
                Console.WriteLine("There was an error while attempting to localize the XMLs: \n" + e.Message + "\n\nPress ENTER to close the program.");
                Console.ReadLine();
            }

            /*
            for (int i = 0; i < 7; i++)
            {
                
                switch (i)
                {
                    case 0:
                        
                        break;

                    case 1:
                        
                        break;

                    case 2:
                        
                        break;

                    case 3:
                        

                    case 4:

                        break;

                    case 5:
                        
                        break;

                    case 6:
                       
                        break;
                }
                
            }
            */
            Console.WriteLine("Finished auto-localization! Press ENTER to close this program...");
            Console.ReadLine();
        }
    }
}
