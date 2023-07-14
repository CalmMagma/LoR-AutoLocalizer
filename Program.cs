using System;
using System.IO;
using System.Xml.Serialization;
using LOR_DiceSystem;
using LOR_XML;
using System.Collections.Generic;

namespace LoR_AutoLocalize
{
    internal class Program
    {
        private static string h = "";

        static string[] langList = new string[4] { "cn", "jp", "kr", "en" };

        static void Main()
        {
            while (true)
            {
                Console.WriteLine("Insert the language of the exported folder (en, jp, kr, cn): ");
                h = Console.ReadLine().ToLower();
                if (langList.Contains(h))
                    break;
                else
                    continue;
            }
            switch (h)
            {
                case "jp":
                    Directory.CreateDirectory("jp");
                    break;

                case "cn":
                    Directory.CreateDirectory("cn");
                    break;

                case "en":
                    Directory.CreateDirectory("en");
                    break;

                case "kr":
                    Directory.CreateDirectory("kr");
                    break;
            }
            Console.WriteLine("Creating folders...");
            Directory.CreateDirectory(h + "/BattleDialogues");
            Directory.CreateDirectory(h + "/BattlesCards");
            Directory.CreateDirectory(h + "/Books");
            Directory.CreateDirectory(h + "/CharacterName");
            Directory.CreateDirectory(h + "/Dropbooks");
            Directory.CreateDirectory(h + "/PassiveDesc");
            Directory.CreateDirectory(h + "/StageName");

            Console.WriteLine("Creating localization XMLs...");
            FileStream[] xmlList = new FileStream[7] {
                File.Create(h + "/BattleDialogues/Dialogs.xml"),
                File.Create(h + "/BattlesCards/CardNames.xml"),
                File.Create(h + "/Books/BookDesc.xml"),
                File.Create(h + "/CharacterName/Characters.xml"),
                File.Create(h + "/Dropbooks/Dropbook.xml"),
                File.Create(h + "/PassiveDesc/Passives.xml"),
                File.Create(h + "/StageName/Stages.xml")
            };

            Console.WriteLine("Reading data XMLs...");
            var dialogData = new XmlSerializer(typeof(BattleDialogRoot)).Deserialize(File.OpenRead("Combat_Dialog.xml")) as BattleDialogRoot;
            var cardData = new XmlSerializer(typeof(DiceCardXmlRoot)).Deserialize(File.OpenRead("CardInfo.xml")) as DiceCardXmlRoot;
            var bookDescData = new XmlSerializer(typeof(BookDescRoot)).Deserialize(File.OpenRead("BookStory.xml")) as BookDescRoot;
            var enemyUnitData = new XmlSerializer(typeof(EnemyUnitClassRoot)).Deserialize(File.OpenRead("EnemyUnitInfo.xml")) as EnemyUnitClassRoot;
            var dropbookData = new XmlSerializer(typeof(BookUseXmlRoot)).Deserialize(File.OpenRead("Dropbook.xml")) as BookUseXmlRoot;
            var keypageData = new XmlSerializer(typeof(BookXmlRoot)).Deserialize(File.OpenRead("EquipPage_Librarian.xml")) as BookXmlRoot;
            var stageData = new XmlSerializer(typeof(StageXmlRoot)).Deserialize(File.OpenRead("StageInfo.xml")) as StageXmlRoot;
            var passiveData = new XmlSerializer(typeof(PassiveXmlRoot)).Deserialize(File.OpenRead("PassiveList.xml")) as PassiveXmlRoot;

            Console.WriteLine("Converting XMLs...");
            for (int i = 0; i < 7; i++)
            {
                Console.WriteLine("> Localizing " + xmlList[i].Name);
                switch (i)
                {
                    case 0:
                        new XmlSerializer(typeof(BattleDialogRoot)).Serialize(xmlList[i], dialogData);
                        break;

                    case 1:
                        var localizeCards = new BattleCardDescRoot() { cardDescList = new List<BattleCardDesc>() };
                        foreach (var card in cardData.cardXmlList)
                        {
                            localizeCards.cardDescList.Add(new BattleCardDesc() { cardID = card._id, cardName = card.workshopName});
                        }
                        new XmlSerializer(typeof(BattleCardDescRoot)).Serialize(xmlList[i], localizeCards);
                        break;

                    case 2:
                        foreach (var guy in keypageData.bookXmlList)
                        {
                            var feller = bookDescData.bookDescList.Find(x => x.bookID == guy._id);
                            if (feller != null) feller.bookName = guy.InnerName;
                        }
                        new XmlSerializer(typeof(BookDescRoot)).Serialize(xmlList[i], bookDescData);
                        break;

                    case 3:
                        var localizeChars = new CharactersNameRoot() { nameList = new List<CharacterName>() };
                        foreach (var character in enemyUnitData.list)
                        {
                            localizeChars.nameList.Add(new CharacterName() { ID = character._id, name = character.name});
                        }
                        new XmlSerializer(typeof(CharactersNameRoot)).Serialize(xmlList[i], localizeChars);
                        break;

                    case 4:
                        var localizeBooks = new CharactersNameRoot() { nameList = new List<CharacterName>() };
                        foreach (var book in dropbookData.bookXmlList)
                        {
                            localizeBooks.nameList.Add(new CharacterName() { ID = book._id, name = book.workshopName });
                        }
                        new XmlSerializer(typeof(CharactersNameRoot)).Serialize(xmlList[i], localizeBooks);
                        break;

                    case 5:
                        var localizePassives = new PassiveDescRoot() { descList = new List<PassiveDesc>() };
                        foreach (var passive in passiveData.list)
                        {
                            localizePassives.descList.Add(new PassiveDesc() { _id = passive._id, desc = passive.desc });
                        }
                        new XmlSerializer(typeof(PassiveDescRoot)).Serialize(xmlList[i], localizePassives);
                        break;

                    case 6:
                        var localizeStage = new CharactersNameRoot() { nameList = new List<CharacterName>() };
                        foreach (var stage in stageData.list)
                        {
                            localizeStage.nameList.Add(new CharacterName() { ID = stage._id, name = stage.stageName });
                        }
                        new XmlSerializer(typeof(CharactersNameRoot)).Serialize(xmlList[i], localizeStage);
                        break;
                }
                Console.WriteLine("! Localized " + xmlList[i].Name);
                xmlList[i].Close();
            }
            Console.WriteLine("Finished auto-localization! Press ENTER to close this program...");
            Console.ReadLine();
        }
    }
}
