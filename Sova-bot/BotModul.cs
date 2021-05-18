using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Sova_bot
{
    class BotModul
    {
        public void explorer(MessageEventArgs e, string[] ID_Message, ref ITelegramBotClient botClient, List<LibraryExplorer> explorer, string name_explorer)
        {

            if (e.Message.Text.ToLower() != "/выход" && e.Message.Text.ToLower() != "/help")
            {
                try
                {
                    string ID_File = @".\Bot_Sound\" + randID_audio().ToString() + ".ogg";
                    string[] Definition = new string[] { };
                    string[] link = new string[] { };
                    Random AssignmentID = new Random();
                    int t = AssignmentID.Next(0, Definition.Length);
                    for (int i = 0; i < explorer.Count; i++)
                    {
                        if (explorer[i].Name.Split('_')[0].ToLower() == e.Message.Text.ToLower() && explorer[i].Name.Split('_')[1].ToLower() == name_explorer)
                        {
                            Array.Resize(ref Definition, Definition.Length + 1);
                            Definition[Definition.Length - 1] = explorer[i].Definition;
                            Array.Resize(ref link, link.Length + 1);
                            link[link.Length - 1] = explorer[i].link;
                        }
                    }
                    VoiceBot(ID_File, Definition[t]);
                    botClient.SendTextMessageAsync(e.Message.Chat, Definition[t] + "\n\nСсылка на источник:" + link[t], replyToMessageId: e.Message.MessageId);
                    botClient.SendVoiceAsync(chatId: e.Message.Chat, voice: System.IO.File.OpenRead(ID_File));

                }
                catch
                {
                    botClient.SendTextMessageAsync(e.Message.Chat, "Я ничего не нашел, повторите запрос", replyToMessageId: e.Message.MessageId);
                }
            }
        }

        public void VoiceBot(string ID_File, string info)
        {
            Directory.CreateDirectory(@".\Bot_Sound");
            SpeechSynthesizer speaker = new SpeechSynthesizer();
            speaker.Volume = 80;
            speaker.Rate = 0;
            try
            {
                speaker.SelectVoice("Anna");
            }
            catch
            {
                speaker.SelectVoice("Irina");
            }
            speaker.SetOutputToWaveFile(ID_File);
            speaker.Speak(info);
            speaker.Dispose();

        }

        public int randID_audio()
        {
            Random AssignmentID = new Random();
            int ID_File = AssignmentID.Next();
            Console.WriteLine(ID_File);
            return ID_File;
        }

        public async Task return_Branch(List<Branch> info)
        {
            JsonTextReader reader = new JsonTextReader(new StreamReader(@".\json\Branch.json"));
            reader.SupportMultipleContent = true;
            while (true)
            {
                if (!reader.Read())
                {
                    break;
                }
                else
                {
                    Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                    Branch obj = serializer.Deserialize<Branch>(reader);
                    info.Add(obj);
                }
            }
        }

        public async Task return_Journal(List<Journal> journal, ComboBox JournalJson)
        {
            JsonTextReader reader = new JsonTextReader(new StreamReader(@".\json\Journal.json"));
            reader.SupportMultipleContent = true;
            while (true)
            {
                if (!reader.Read())
                {
                    break;
                }
                else
                {
                    Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                    Journal obj = serializer.Deserialize<Journal>(reader);
                    journal.Add(obj);
                    for (int i = 0; i < journal.Count; i++)
                    {
                        JournalJson.Items.Add("Имя записи: " + journal[i].Name + "\nПуть к файлу записи: " + journal[i].Road + "\nДата и время записи: " + journal[i].Time);
                    }
                    journal.Clear();
                }
            }
        }

        public async Task return_Function(List<Functions> function)
        {
            JsonTextReader reader = new JsonTextReader(new StreamReader(@".\json\Functions.json"));
            reader.SupportMultipleContent = true;
            while (true)
            {
                if (!reader.Read())
                {
                    break;
                }
                else
                {
                    Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                    Functions obj = serializer.Deserialize<Functions>(reader);
                    function.Add(obj);
                }
            }
        }

        public void check(ref string[] checking, ref string[] file_list, List<LibraryExplorer> explorer, List<LibarySection> sections, List<LibaryTutorial> tutorial)
        {
            List<string> clearName = new List<string> { "\\", "Explorer", ".", "json" };
            string[] section_p = new string[] { };

            for (int t = 0; t < checking.Length; t++)
            {
                for (int i = 0; i < clearName.Count; i++)
                {
                    checking[t] = checking[t].Replace(clearName[i], string.Empty);
                }
            }
            for (int t = 0; t < checking.Length; t++)
            {
                if (checking[t].Split('_')[0].ToLower() == "section")
                {
                    Array.Resize(ref section_p, section_p.Length + 1);
                    section_p[section_p.Length - 1] = checking[t].Split('_')[1].ToLower();
                    checking[t] = checking[t].Split('_')[0].ToLower();
                }
                else if (checking[t].Split('_')[0].ToLower() == "tutorial")
                {
                    Array.Resize(ref section_p, section_p.Length + 1);
                    section_p[section_p.Length - 1] = checking[t].Split('_')[1].ToLower();
                    checking[t] = checking[t].Split('_')[0].ToLower();
                }
                else if (checking[t].Split('_')[0].ToLower() == "branch")
                {
                    checking[t] = checking[t].Split('_')[0].ToLower();
                }
                else if (checking[t].Split('_')[0].ToLower() == "journal")
                {
                    checking[t] = checking[t].Split('_')[0].ToLower();
                }
                else if (checking[t].Split('_')[0].ToLower() == "functions")
                {
                    checking[t] = checking[t].Split('_')[0].ToLower();
                }
                else
                {
                    checking[t] = checking[t].Split('_')[1].ToLower();
                }
                Console.WriteLine("в массиве:" + checking[t]);
            }
            int q = 0;
            for (int i = 0; i < file_list.Length; i++)
            {
                bool t = false;
                bool s = false;
                bool u = false;
                if (checking[i].ToLower() == "branch")
                {
                    t = true;
                }
                else if (checking[i].ToLower() == "journal")
                {
                    t = true;
                }
                else if (checking[i].ToLower() == "functions")
                {
                    t = true;
                }
                else if (checking[i].ToLower() == "section")
                {
                    t = true;
                    s = true;
                    checking[i] = section_p[q];
                    q++;

                }
                else if (checking[i].ToLower() == "tutorial")
                {
                    t = true;
                    u = true;
                    checking[i] = section_p[q];
                    q++;

                }
                if (t != true)
                {
                    JsonTextReader reader = new JsonTextReader(new StreamReader(file_list[i]));
                    reader.SupportMultipleContent = true;
                    while (true)
                    {
                        if (!reader.Read())
                        {
                            break;
                        }
                        else
                        {
                            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                            LibraryExplorer obj = serializer.Deserialize<LibraryExplorer>(reader);
                            explorer.Add(obj);
                        }
                    }
                }
                else if (s == true)
                {
                    JsonTextReader reader = new JsonTextReader(new StreamReader(file_list[i]));
                    reader.SupportMultipleContent = true;
                    while (true)
                    {
                        if (!reader.Read())
                        {
                            break;
                        }
                        else
                        {
                            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                            LibarySection obj = serializer.Deserialize<LibarySection>(reader);
                            sections.Add(obj);
                        }
                    }

                }
                else if (u == true)
                {
                    JsonTextReader reader = new JsonTextReader(new StreamReader(file_list[i]));
                    reader.SupportMultipleContent = true;
                    while (true)
                    {
                        if (!reader.Read())
                        {
                            break;
                        }
                        else
                        {
                            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                            LibaryTutorial obj = serializer.Deserialize<LibaryTutorial>(reader);
                            tutorial.Add(obj);
                        }
                    }
                }
            }
        }

        public async Task Recording_JSON(ComboBox fileJson, TextBox pageHtml, TextBox Name_JSON, TextBox URL_adr, string[] file_list, string[] checking, ComboBox JournalJson, List<LibraryExplorer> explorer, List<LibarySection> sections, List<Journal> journal, List<Functions> function, List<LibaryTutorial> tutorial)
        {
            Directory.CreateDirectory(@".\json");
            List<string> clearName = new List<string> { "\\", ".", "json" };
            string file = fileJson.Text;
            foreach (string i in clearName)
            {
                file = file.Replace(i, string.Empty);
            }
            char[] clear = { '*', '.', ' ', ':', '\"', '\'' };
            if (file.Split('_')[0].ToLower() == "explorer")
            {
                if (Name_JSON.Text == string.Empty)
                {
                    MessageBox.Show("Имя json-объекта пустое!", "Уведомление");
                }
                else if (pageHtml.Text.Length >= 4096)
                {
                    MessageBox.Show("Превышен лимит записи!", "Уведомление");
                }
                else
                {
                    try
                    {
                        if (pageHtml.Text.Trim(clear) != string.Empty)
                        {
                            int t = 0;
                            for (; t < file_list.Length; t++)
                            {
                                if (file_list[t] == fileJson.Text)
                                {
                                    break;
                                }
                            }
                            if (MessageBox.Show("Внимание! Вы уверены, что хотите записать в " + fileJson.Text + "\nДаную запись c именем " + Name_JSON.Text.ToLower(), "Уведомление", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                            {
                                using (FileStream fs = new FileStream(@"" + fileJson.Text, FileMode.Append))
                                {
                                    LibraryExplorer Title = new LibraryExplorer() { Name = Name_JSON.Text.ToLower() + "_" + checking[t], Definition = pageHtml.Text, link = URL_adr.Text };
                                    await System.Text.Json.JsonSerializer.SerializeAsync<LibraryExplorer>(fs, Title);
                                    explorer.Add(Title);
                                    MessageBox.Show("Запись добавлена", "Уведомление");
                                }
                                using (FileStream fs = new FileStream(@".\json\Journal.json", FileMode.Append))
                                {
                                    Journal Title = new Journal() { Name = Name_JSON.Text.ToLower() + "_" + checking[t], Road = fileJson.Text, Time = DateTime.Now.ToString() };
                                    await System.Text.Json.JsonSerializer.SerializeAsync<Journal>(fs, Title);
                                    journal.Add(Title);
                                    JournalJson.Items.Add("Имя записи: " + journal[journal.Count - 1].Name + "\nПуть к файлу записи: " + journal[journal.Count - 1].Road + "\nДата и время записи: " + journal[journal.Count - 1].Time);
                                    journal.Clear();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Ошибка запись не была добавлена по причине отмены", "Уведомление");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Ошибка запись не была добавлена", "Уведомление");
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка, возникло исключение!", "Уведомление");
                    }
                }
            }
            else if (file.ToLower() == "branch")
            {
                if (Name_JSON.Text == string.Empty)
                {
                    MessageBox.Show("Имя json-объекта пустое!", "Уведомление");
                }
                else
                {
                    try
                    {
                        using (FileStream fs = new FileStream(@"" + fileJson.Text, FileMode.Append))
                        {
                            Branch Title = new Branch() { Name = Name_JSON.Text.ToLower() };
                            await System.Text.Json.JsonSerializer.SerializeAsync<Branch>(fs, Title);
                            MessageBox.Show("Запись добавлена", "Уведомление");
                        }
                        using (FileStream fs = new FileStream(@".\json\Journal.json", FileMode.Append))
                        {
                            Journal Title = new Journal() { Name = Name_JSON.Text.ToLower(), Road = fileJson.Text, Time = DateTime.Now.ToString() };
                            await System.Text.Json.JsonSerializer.SerializeAsync<Journal>(fs, Title);
                            journal.Add(Title);
                            JournalJson.Items.Add("Имя записи: " + journal[journal.Count - 1].Name + "\nПуть к файлу записи: " + journal[journal.Count - 1].Road + "\nДата и время записи: " + journal[journal.Count - 1].Time);
                            journal.Clear();
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка, возникло исключение!", "Уведомление");
                    }
                }
            }
            else if (file.ToLower() == "functions")
            {
                if (Name_JSON.Text == string.Empty)
                {
                    MessageBox.Show("Имя json-объекта пустое!", "Уведомление");
                }
                else if (pageHtml.Text.Length >= 4096)
                {
                    MessageBox.Show("Превышен лимит записи!", "Уведомление");
                }
                else
                {
                    try
                    {
                        using (FileStream fs = new FileStream(@"" + fileJson.Text, FileMode.Append))
                        {
                            Functions Title = new Functions() { Name = Name_JSON.Text.ToLower(), Definition = pageHtml.Text };
                            await System.Text.Json.JsonSerializer.SerializeAsync<Functions>(fs, Title);
                            function.Add(Title);
                            MessageBox.Show("Запись добавлена", "Уведомление");
                        }
                        using (FileStream fs = new FileStream(@".\json\Journal.json", FileMode.Append))
                        {
                            Journal Title = new Journal() { Name = Name_JSON.Text.ToLower(), Road = fileJson.Text, Time = DateTime.Now.ToString() };
                            await System.Text.Json.JsonSerializer.SerializeAsync<Journal>(fs, Title);
                            journal.Add(Title);
                            JournalJson.Items.Add("Имя записи: " + journal[journal.Count - 1].Name + "\nПуть к файлу записи: " + journal[journal.Count - 1].Road + "\nДата и время записи: " + journal[journal.Count - 1].Time);
                            journal.Clear();
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка, возникло исключение!", "Уведомление");
                    }
                }
            }
            else if (file.Split('_')[0].ToLower() == "section")
            {
                try
                {
                    int number = 1;
                    if (pageHtml.Text.Trim(clear) != string.Empty)
                    {
                        int t = 0;
                        for (; t < file_list.Length; t++)
                        {
                            if (file_list[t] == fileJson.Text)
                            {
                                break;
                            }
                        }
                        if (MessageBox.Show("Внимание! Вы уверены, что хотите записать в " + fileJson.Text + " даную запись", "Уведомление", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            for (int i = 0; i < sections.Count; i++)
                            {
                                if (sections[i].Name.Split('_')[1] == checking[t].Split(' ')[0])
                                {
                                    number++;
                                }
                            }
                            try
                            {
                                if (checking[t].Split(' ')[1] == "true")
                                {
                                    checking[t] = checking[t].Split(' ')[0];
                                }
                            }
                            catch { };
                            using (FileStream fs = new FileStream(@"" + fileJson.Text, FileMode.Append))
                            {
                                LibarySection Title = new LibarySection() { Name = "section" + "_" + checking[t] + "_" + number, Definition = pageHtml.Text };
                                await System.Text.Json.JsonSerializer.SerializeAsync<LibarySection>(fs, Title);
                                sections.Add(Title);
                            }
                            using (FileStream fs = new FileStream(@".\json\Journal.json", FileMode.Append))
                            {
                                Journal Title = new Journal() { Name = "section" + "_" + checking[t] + "_" + number, Road = fileJson.Text, Time = DateTime.Now.ToString() };
                                await System.Text.Json.JsonSerializer.SerializeAsync<Journal>(fs, Title);
                                journal.Add(Title);
                                JournalJson.Items.Add("Имя записи: " + journal[journal.Count - 1].Name + "\nПуть к файлу записи: " + journal[journal.Count - 1].Road + "\nДата и время записи: " + journal[journal.Count - 1].Time);
                                checking[t] += " true";
                                journal.Clear();
                            }
                            MessageBox.Show("Запись добавлена", "Уведомление");
                        }
                        else
                        {
                            MessageBox.Show("Ошибка запись не была добавлена по причине отмены", "Уведомление");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ошибка запись не была добавлена", "Уведомление");
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка, возникло исключение!", "Уведомление");
                }
            }
            else if (file.Split('_')[0].ToLower() == "tutorial")
            {
                try
                {
                    int number = 1;
                    if (pageHtml.Text.Trim(clear) != string.Empty)
                    {
                        int t = 0;
                        for (; t < file_list.Length; t++)
                        {
                            if (file_list[t] == fileJson.Text)
                            {
                                break;
                            }
                        }
                        if (MessageBox.Show("Внимание! Вы уверены, что хотите записать в " + fileJson.Text + " даную запись", "Уведомление", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            for (int i = 0; i < tutorial.Count; i++)
                            {
                                if (tutorial[i].Name.Split('_')[1] == checking[t].Split(' ')[0])
                                {
                                    number++;
                                }
                            }
                            try
                            {
                                if (checking[t].Split(' ')[1] == "true")
                                {
                                    checking[t] = checking[t].Split(' ')[0];
                                }
                            }
                            catch
                            {

                            };
                            using (FileStream fs = new FileStream(@"" + fileJson.Text, FileMode.Append))
                            {
                                LibaryTutorial Title = new LibaryTutorial() { Name = "tutorial" + "_" + checking[t] + "_" + number, Definition = pageHtml.Text };
                                await System.Text.Json.JsonSerializer.SerializeAsync<LibaryTutorial>(fs, Title);
                                MessageBox.Show("Запись добавлена", "Уведомление");
                                tutorial.Add(Title);
                            }
                            using (FileStream fs = new FileStream(@".\json\Journal.json", FileMode.Append))
                            {
                                Journal Title = new Journal() { Name = "tutorial" + "_" + checking[t] + "_" + number, Road = fileJson.Text, Time = DateTime.Now.ToString() };
                                await System.Text.Json.JsonSerializer.SerializeAsync<Journal>(fs, Title);
                                journal.Add(Title);
                                JournalJson.Items.Add("Имя записи: " + journal[journal.Count - 1].Name + "\nПуть к файлу записи: " + journal[journal.Count - 1].Road + "\nДата и время записи: " + journal[journal.Count - 1].Time);
                                checking[t] += " true";
                                journal.Clear();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Ошибка запись не была добавлена по причине отмены", "Уведомление");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ошибка запись не была добавлена", "Уведомление");
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка, возникло исключение!", "Уведомление");
                }
            }
            else
            {
                MessageBox.Show("В данный файл нельзя записать данные!", "Уведомление");
            }
        }

        public void splitting_message(ITelegramBotClient botClient, CallbackQueryEventArgs ev, string old_message, string[] text, string ID_File)
        {
            if (text.Length < 501)
            {
                botClient.SendTextMessageAsync(ev.CallbackQuery.Message.Chat, old_message);
                botClient.SendVoiceAsync(chatId: ev.CallbackQuery.Message.Chat, voice: System.IO.File.OpenRead(ID_File));
            }
            else
            {
                int limit = 200;
                string limit_text = "";

                try
                {
                    for (int q = 0; 0 < text.Length; q++)
                    {
                        limit_text += " " + text[q];
                        if (q == limit)
                        {
                            limit = limit + q;
                            botClient.SendTextMessageAsync(ev.CallbackQuery.Message.Chat, limit_text);
                            limit_text = "";
                        }
                    }
                }
                catch
                {
                    botClient.SendTextMessageAsync(ev.CallbackQuery.Message.Chat, limit_text);
                    botClient.SendVoiceAsync(chatId: ev.CallbackQuery.Message.Chat, voice: System.IO.File.OpenRead(ID_File));
                }
            }
        }
    }
}
