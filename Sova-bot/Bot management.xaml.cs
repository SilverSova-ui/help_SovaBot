using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using HtmlAgilityPack;
using System.Speech.Synthesis;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sova_bot
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    //добавить аудио функцию для учебника и туторила
    class LibraryExplorer
    {
        public string Name { get; set; }
        public string Definition { get; set; }
        public string link { get; set; }
    }

    class LibarySection
    {
        public string Name { get; set; }
        public string Definition { get; set; }
    }

    class LibaryTutorial
    {
        public string Name { get; set; }
        public string Definition { get; set; }
    }

    class Functions
    {
        public string Name { get; set; }
        public string Definition { get; set; }
    }

    class Journal
    {
        public string Name { get; set; }
        public string Road { get; set; }
        public string Time { get; set; }

    }

    class Branch
    {
        public string Name { get; set; }
    }


    public partial class MainWindow : Window
    {
        //ПЛАН: ФУНКЦИИ перекинуть в классы и циклы желательно тоже, прописать диалог, и наполнить JSON
        static ITelegramBotClient botClient;

        List<Branch> info = new List<Branch>();
        List<LibraryExplorer> explorer = new List<LibraryExplorer>();
        List<LibarySection> sections = new List<LibarySection>();
        List<LibaryTutorial> tutorial = new List<LibaryTutorial>();
        List<Journal> journal = new List<Journal>();
        List<Functions> function = new List<Functions>();

        string[] file_list = new string[] { };
        string[] checking = new string[] { };

        public MainWindow()
        {
            InitializeComponent();

            try
            { 
            file_list = Directory.GetFiles(@".\json", "*.json");
            checking = Directory.GetFiles(@".\json", "*.json");
            }
            catch 
            {
                Directory.CreateDirectory(@".\json");
                System.IO.File.Create(@".\json\Branch.json");
                System.IO.File.Create(@".\json\Journal.json");
                MessageBox.Show("Внимание! Была создана директория \"json\" и ключивые файлы \"Branch\" и \"Journal\"", "Уведомление");
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }

            BotModul transition = new BotModul();
            transition.return_Branch(info);
            transition.return_Journal(journal, JournalJson);
            transition.return_Function(function);
            transition.check(ref checking, ref file_list, explorer, sections, tutorial);

            fileJson.ItemsSource = file_list;


            Task.Factory.StartNew(() =>
            {
                int i = 0;
                while (true)
                {
                    Thread.Sleep(1200000);
                    string[] ID_Audio = Directory.GetFiles(@".\Bot_Sound", "*.ogg");
                    try
                    {
                        while (true)
                        {
                            System.IO.File.Delete(ID_Audio[i]);
                            i++;
                            break;
                        }
                    }
                    catch
                    {
                        i = 0;
                    }
                }
            });

        }

        bool switching = false;
        bool included = true;
        private void Bot_Enable(object sender, RoutedEventArgs e)
        {
            try
            {
                if (included == true) 
                {
                    botClient = new TelegramBotClient(Telegram_key.Text);
                    User me = botClient.GetMeAsync().Result;
                    botClient.OnMessage += Bot_OnMessage;
                    botClient.OnCallbackQuery += Bot_OnCallbackQuery;
                    botClient.StartReceiving();
                    switching = true;
                    included = false;
                    Turn_on.Content = "Выключить бот";
                    MessageBox.Show("Бот включен", "Уведомление");
                }
                else if (switching == true)
                {
                    switching = false;
                    MessageBox.Show("Бот выключен", "Уведомление");
                    Turn_on.Content = "Включить бот";
                }
                else if (switching == false)
                {
                    switching = true;
                    MessageBox.Show("Бот включен", "Уведомление");
                    Turn_on.Content = "Выключить бот";
                }
            }
            catch
            {
                MessageBox.Show("Ключ введен неправильно!", "Уведомление");
            }
        }

        string[] ID = new string[] {};
        string[] ID_Message = new string[] {};

        async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (switching == true)
            {
                string name_explorer = null;
                string mode = null;

                try
                {
                    if (ID_Message.Length > 0)
                    {
                        for (int i = 0; i < ID_Message.Length; i++)
                        {
                            if (e.Message.Chat.Id.ToString() == ID_Message[i].Split(' ')[0])
                            {
                                name_explorer = ID_Message[i].Split(' ')[1];
                                mode = ID_Message[i].Split(' ')[2];
                            }
                        }
                    }
                }
                catch
                {

                }

                if (!ID.Contains(e.Message.Chat.Id.ToString()))
                {
                    Array.Resize(ref ID, ID.Length + 1);
                    ID[ID.Length - 1] = e.Message.Chat.Id.ToString();
                    Array.Resize(ref ID_Message, ID_Message.Length + 1);
                    ID_Message[ID_Message.Length - 1] = e.Message.Chat.Id.ToString();
                }

                for (int i = 0; i < ID_Message.Length; i++)
                {
                    Console.WriteLine("В массиве есть:" + ID_Message[i]);
                }

                bool check_section = false;
                bool check_exp = false;
                string name = null;

                for (int q = 0; q < info.Count; q++)
                {
                    if (ID_Message.Contains(e.Message.Chat.Id.ToString() + " " + info[q].Name.ToLower() + " " + mode))
                    {
                        check_exp = true;
                        break;
                    }
                }
                for (int q = 0; q < info.Count; q++)
                {
                    if (ID_Message.Contains(e.Message.Chat.Id.ToString() + " " + info[q].Name.ToLower()))
                    {
                        check_section = true;
                        name = info[q].Name.ToLower();
                        break;
                    }
                }

                if (e.Message.Text.ToLower() == "/выход")
                {
                    string ID_exit = null;
                    string section = " ";
                    string name_r = null;
                    try
                    {
                        for (int i = 0; i < ID_Message.Length; i++)
                        {
                            if (ID_Message[i].Split(' ')[0] == e.Message.Chat.Id.ToString())
                            {
                                ID_exit = ID_Message[i].Split(' ')[0];
                                section += ID_Message[i].Split(' ')[1];
                                ID_exit += " " + ID_Message[i].Split(' ')[1];
                                section += " " + ID_Message[i].Split(' ')[2];
                                if (ID_Message.Contains(ID_Message[i]))
                                {
                                    name_r = ID_Message[i].Split(' ')[1];
                                    string r = ID_Message[i].Split(' ')[2];
                                    if (r == "режимУчебника")
                                    {
                                        r = "учебника";
                                    }
                                    else if (r == "поиск")
                                    {
                                        r = "поиск";
                                    }
                                    else if (r == "режимОбучения")
                                    {
                                        r = "обучения";
                                    }
                                    ID_Message[i] = ID_exit;
                                    ReplyKeyboardMarkup CustomKeyboard = new[]
                                    {
                                        new[] {"Поиск","Режим учебника","Режим обучения"},
                                    };
                                    await botClient.SendTextMessageAsync(e.Message.Chat, "Выхожу из режима " + " " + r + " в разделе " + name_r, replyToMessageId: e.Message.MessageId, replyMarkup: CustomKeyboard);
                                }
                            }
                        }
                    }
                    catch
                    {
                        try
                        {
                            for (int i = 0; i < ID_Message.Length; i++)
                            {
                                if (ID_Message[i].Split(' ')[0] == e.Message.Chat.Id.ToString())
                                {
                                    name_r = ID_Message[i].Split(' ')[1];
                                    Id_Module call = new Id_Module();
                                    await Task.Run(() => call.no_branching(e, section, ID_Message));
                                    await botClient.SendTextMessageAsync(e.Message.Chat, "Выхожу из раздела " + name_r, replyToMessageId: e.Message.MessageId, replyMarkup: new ReplyKeyboardRemove());
                                    break;
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                else if(e.Message.Text.ToLower() == "/help")
                {
                    await botClient.SendTextMessageAsync(e.Message.Chat, "Справочный бот Сова режим помощи\n\nДоступные основные команды:\n/start \n/начать \nрасскажи о себе \n\nДоступные команды в разделах языках программирования: \nпоиск \nрежим учебника \nрежим обучения\n\n Доступные команды помщи: \n / выход \n / help", replyToMessageId: e.Message.MessageId);
                    await botClient.SendTextMessageAsync(e.Message.Chat, "Режимы работы справочного бота Совы: \n\nРежим поиска - этот режим позволяет искать нужную вам информацию в языках программирования, если то, что искали было не найдено значит я ещё не владею данной информацией." +
                        " \n\nРежим учебника - в этом режиме я предоставляю информацию о языке программирование, которой вы выбрали. Просто выберите интересующую вас главу. \n\nРежим обучения - в этом режиме я попытаюсь обучить вас, выбранному языку программирования, просто выберите урок.", replyMarkup: new ReplyKeyboardRemove());
                }
                else if (ID_Message.Contains(e.Message.Chat.Id.ToString()))
                {
                    try
                    {
                        switch (e.Message.Text.ToLower())
                        {
                            case "/start":
                                {
                                    await botClient.SendTextMessageAsync(e.Message.Chat, "Справочный бот Сова к вашим услугам", replyToMessageId: e.Message.MessageId, replyMarkup: new ReplyKeyboardRemove());
                                    await botClient.SendTextMessageAsync(e.Message.Chat, "Доступные основные команды:\n- /start \n- /начать \n- расскажи о себе \n\nДоступные команды в разделах языках программирования: \n- поиск \n- режим учебника \n- режим обучения\n\n Доступные команды помщи: \n- /выход \n- /help");
                                    break;
                                }
                            case "/начать":
                                {
                                    InlineKeyboardButton[] Keyboard = new InlineKeyboardButton[] { };
                                    for (int i = 0; i < info.Count; i++)
                                    {
                                        Array.Resize(ref Keyboard, Keyboard.Length + 1);
                                        Keyboard[Keyboard.Length - 1] = InlineKeyboardButton.WithCallbackData(info[i].Name.ToLower(), info[i].Name.ToLower());
                                    }
                                    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[] { Keyboard });//переход к второму этапу
                                    await botClient.SendTextMessageAsync(e.Message.Chat, "Принято, начнем ", replyToMessageId: e.Message.MessageId, replyMarkup: new ReplyKeyboardRemove());
                                    await botClient.SendTextMessageAsync(e.Message.Chat, "Для начало, выберите язык программирования:", replyMarkup: inlineKeyboard);
                                    break;
                                }
                            case "расскажи о себе":
                                {
                                    InlineKeyboardButton[] Keyboard_1 = new InlineKeyboardButton[] { };
                                    InlineKeyboardButton[] Keyboard_2 = new InlineKeyboardButton[] { };
                                    int k = 0;
                                    for (int i = 0; i < function.Count; i++) {

                                        if (k <= 8) {
                                            Array.Resize(ref Keyboard_1, Keyboard_1.Length + 1);
                                            Keyboard_1[Keyboard_1.Length - 1] = InlineKeyboardButton.WithCallbackData(function[i].Name.ToLower(), "Функция " + function[i].Name.ToLower());
                                            k++;
                                        }
                                        else if (k >= 8)
                                        {
                                            Array.Resize(ref Keyboard_2, Keyboard_2.Length + 1);
                                            Keyboard_2[Keyboard_2.Length - 1] = InlineKeyboardButton.WithCallbackData(function[i].Name.ToLower(), "Функция " + function[i].Name.ToLower());
                                        }
                                    }
                                    InlineKeyboardMarkup inlineKeyboard_1 = new InlineKeyboardMarkup(new[] { Keyboard_1 });
                                    InlineKeyboardMarkup inlineKeyboard_2 = new InlineKeyboardMarkup(new[] { Keyboard_2 });
                                    await botClient.SendTextMessageAsync(e.Message.Chat, "А какой функции мне расказать?", replyMarkup: inlineKeyboard_1);
                                    if (k>=8)
                                    {
                                        await botClient.SendTextMessageAsync(e.Message.Chat, "Продолжение", replyMarkup: inlineKeyboard_2);
                                    }
                                    break;
                                }
                            default:
                                await botClient.SendTextMessageAsync(e.Message.Chat, "Я искал эту команду и не нашел", replyToMessageId: e.Message.MessageId);
                                break;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка, возникло исключение!", "Уведомление");
                    }
                }
                else if (check_exp == true)
                {
                    BotModul call = new BotModul();
                    await Task.Run(() => call.explorer(e, ID_Message, ref botClient, explorer, name_explorer));
                }
                else if (check_section == true)
                {
                    await botClient.SendTextMessageAsync(e.Message.Chat, "Вы в разделе " + name, replyMarkup: new ReplyKeyboardRemove(), replyToMessageId: e.Message.MessageId);
                    switch (e.Message.Text.ToLower())
                    {
                        case "поиск":
                            {
                                string section = " " + name + " поиск";
                                Id_Module call = new Id_Module();
                                await Task.Run(() => call.branching_section(e, ref section, ref ID_Message));
                                await botClient.SendTextMessageAsync(e.Message.Chat, "Режим поиска для языка программирование " + name + " запущен", replyToMessageId: e.Message.MessageId);
                                break;
                            }
                        case "режим учебника":
                            {
                                string section = " " + name + " режимУчебника";
                                Id_Module call = new Id_Module();
                                await Task.Run(() => call.branching_section(e, ref section, ref ID_Message));
                                InlineKeyboardButton[] Keyboard_1 = new InlineKeyboardButton[] { };
                                InlineKeyboardButton[] Keyboard_2 = new InlineKeyboardButton[] { };
                                bool p = false;
                                int k = 0;//развить идею
                                for (int i = 0; i < ID_Message.Length; i++)
                                {
                                    for (int t = 0; t < sections.Count; t++)
                                    {
                                        if (sections[t].Name.Split('_')[1].ToLower() == ID_Message[i].Split(' ')[1] && ID_Message[i].Split(' ')[0].ToLower() == e.Message.Chat.Id.ToString() && p == false)
                                        {
                                            Array.Resize(ref Keyboard_1, Keyboard_1.Length + 1);
                                            Keyboard_1[Keyboard_1.Length - 1] = InlineKeyboardButton.WithCallbackData("Глава " + sections[t].Name.ToLower().Split('_')[2], "Глава " + sections[t].Name.ToLower().Split('_')[2]);
                                            k++;
                                            if (k == 8)
                                            {
                                                p = true;
                                            }
                                        }
                                        else if(sections[t].Name.Split('_')[1].ToLower() == ID_Message[i].Split(' ')[1] && ID_Message[i].Split(' ')[0].ToLower() == e.Message.Chat.Id.ToString() && p == true)
                                        {
                                            Array.Resize(ref Keyboard_2, Keyboard_2.Length + 1);
                                            Keyboard_2[Keyboard_2.Length - 1] = InlineKeyboardButton.WithCallbackData("Глава " + sections[t].Name.ToLower().Split('_')[2], "Глава " + sections[t].Name.ToLower().Split('_')[2]);
                                        }
                                    }
                                }
                                InlineKeyboardMarkup inlineKeyboard_1 = new InlineKeyboardMarkup(new[] { Keyboard_1 });
                                InlineKeyboardMarkup inlineKeyboard_2 = new InlineKeyboardMarkup(new[] { Keyboard_2 });
                                await botClient.SendTextMessageAsync(e.Message.Chat, "Хорошо, я расскажу о языке программирование " + name + ". \nКакую главу мне открыть?", replyToMessageId: e.Message.MessageId, replyMarkup: inlineKeyboard_1);
                                if (p == true)
                                {
                                    await botClient.SendTextMessageAsync(e.Message.Chat, "Продолжение", replyToMessageId: e.Message.MessageId, replyMarkup: inlineKeyboard_2);
                                }
                                break;
                            }
                        case "режим обучения":
                            {
                                string section = " " + name + " режимОбучения";
                                Id_Module call = new Id_Module();
                                await Task.Run(() => call.branching_section(e, ref section, ref ID_Message));
                                InlineKeyboardButton[] Keyboard = new InlineKeyboardButton[] { };
                                for (int i = 0; i < ID_Message.Length; i++)
                                {
                                    for (int t = 0; t < tutorial.Count; t++)
                                    {
                                        if (tutorial[t].Name.Split('_')[1].ToLower() == ID_Message[i].Split(' ')[1] && ID_Message[i].Split(' ')[0].ToLower() == e.Message.Chat.Id.ToString())
                                        {
                                            Array.Resize(ref Keyboard, Keyboard.Length + 1);
                                            Keyboard[Keyboard.Length - 1] = InlineKeyboardButton.WithCallbackData("Урок " + tutorial[t].Name.ToLower().Split('_')[2], "Урок " + tutorial[t].Name.ToLower().Split('_')[2]);
                                        }
                                    }
                                }
                                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[] { Keyboard });
                                await botClient.SendTextMessageAsync(e.Message.Chat, "Я обучу вас языку программирование " + name + ". \nС какого урока мне начать?", replyToMessageId: e.Message.MessageId, replyMarkup: inlineKeyboard);// создание кнопок, переход к другой функции
                                break;
                            }
                        default:
                            await botClient.SendTextMessageAsync(e.Message.Chat, "Данной команды нет " + char.ConvertFromUtf32(0x1F616), replyToMessageId: e.Message.MessageId);
                            break;
                    }
                }
            }
        }

        public void Bot_OnCallbackQuery(object sc, Telegram.Bot.Args.CallbackQueryEventArgs ev)
        {
            if (ev.CallbackQuery.Data == ev.CallbackQuery.Data)
            {
                for (int t = 0; t<function.Count; t++) {
                    if ("Функция " + function[t].Name == ev.CallbackQuery.Data)
                    {

                        botClient.SendTextMessageAsync(ev.CallbackQuery.Message.Chat, function[t].Definition.ToLower());
                        break;
                    }
                }
            }
            if (ID_Message.Contains(ev.CallbackQuery.Message.Chat.Id.ToString())) {
                ReplyKeyboardMarkup CustomKeyboard = new[]
                {
                  new[] {"Поиск","Режим учебника","Режим обучения"},
                };
                bool t = false;
                string name = null;
                for (int q = 0; q < info.Count; q++)
                {
                    if (ev.CallbackQuery.Data.ToLower() == info[q].Name.ToLower())
                    {
                        t = true;
                        name = info[q].Name.ToLower();
                        Console.WriteLine(q);
                    }
                }
                if (t == true)
                {
                    string section = " " + name;
                    Id_Module call = new Id_Module();
                    call.branching(ev, section, ref ID_Message);
                    botClient.SendTextMessageAsync(ev.CallbackQuery.Message.Chat,  "Значит язык программирование " + ev.CallbackQuery.Data + ". \r\nВыберите режим, используя нижние кнопки. \r\nУзнать о режимах работы можно с помощью команды /help. \r\nДля того что бы выйти из режима " + name + " используйте команду\r\n /выход", replyMarkup: CustomKeyboard);
                    botClient.AnswerCallbackQueryAsync(ev.CallbackQuery.Id);
                }
            }
            else
            {
                BotModul call = new BotModul();
                for (int i = 0; i < ID_Message.Length; i++)
                {
                    for (int t = 0; t < sections.Count; t++) {
                        if (sections[t].Name.Split('_')[1].ToLower() == ID_Message[i].Split(' ')[1] && ID_Message[i].Split(' ')[0].ToLower() == ev.CallbackQuery.Message.Chat.Id.ToString() && ev.CallbackQuery.Data == "Глава " + sections[t].Name.Split('_')[2].ToLower())
                        {
                            botClient.SendTextMessageAsync(ev.CallbackQuery.Message.Chat, "Принято, ожидайте...");
                            string[] text = sections[t].Definition.Split(' ');
                            string old_message = sections[t].Definition;
                            string ID_File = @".\Bot_Sound\" + call.randID_audio().ToString() + ".ogg";
                            call.VoiceBot(ID_File, sections[t].Definition);
                            call.splitting_message(botClient,ev, old_message, text,ID_File);
                          
                        }
                    }
                    for (int t = 0; t < tutorial.Count; t++)
                    {
                        if (tutorial[t].Name.Split('_')[1].ToLower() == ID_Message[i].Split(' ')[1] && ID_Message[i].Split(' ')[0].ToLower() == ev.CallbackQuery.Message.Chat.Id.ToString() && ev.CallbackQuery.Data == "Урок " + tutorial[t].Name.Split('_')[2].ToLower())
                        {
                            botClient.SendTextMessageAsync(ev.CallbackQuery.Message.Chat, "Принято, ожидайте...");
                            string[] text = tutorial[t].Definition.Split(' ');
                            string old_message = tutorial[t].Definition;
                            string ID_File = @".\Bot_Sound\" + call.randID_audio().ToString() + ".ogg";
                            call.VoiceBot(ID_File, tutorial[t].Definition);
                            call.splitting_message(botClient, ev, old_message, text, ID_File);
                           
                        }
                    }
                }
            }
        }

        public void parsNoTags(object sender, RoutedEventArgs e)
        {
            if (URL_adr.Text != string.Empty) {
                string a = roadHtml.Text;
                roadHtml.Text = "//html";
                pars();
                roadHtml.Text = a;
            }
            else
            {
                MessageBox.Show("URL Адрес не введен!", "Уведомление");
            }
        }

        private void parsTags(object sender, RoutedEventArgs e)
        {
            if (URL_adr.Text != string.Empty && roadHtml.Text != string.Empty) {
                pars();
            }
            else
            {
                MessageBox.Show("URL Адрес или HTML теги не введены!", "Уведомление");
            }
        }

        public void pars()
        {
            //парсинг
            HtmlWeb webDoc = new HtmlWeb();
            webDoc.OverrideEncoding = Encoding.UTF8;
            try
            {
                HtmlDocument doc = webDoc.Load(URL_adr.Text);
                HtmlNodeCollection par = doc.DocumentNode.SelectNodes(roadHtml.Text);
                if (par != null)
                {
                    foreach (HtmlNode tag in par)
                    {
                        string explor = tag.InnerText;
                        pageHtml.Text = explor;
                    }
                    MessageBox.Show("Извлечение данных завершено!", "Уведомление");
                }
                else
                {
                    MessageBox.Show("Ошибка, Теги заданы неверно", "Уведомление");
                }
            }
            catch
            {
                MessageBox.Show("Ошибка, возникло исключение!", "Уведомление");
            }
        }

        private void Add_JSON(object sender, RoutedEventArgs e)
        {
            if (fileJson.Text != string.Empty) {
                if (pageHtml.Text != string.Empty) {
                    BotModul transition = new BotModul();
                    transition.Recording_JSON(fileJson, pageHtml, Name_JSON, URL_adr, file_list, checking, JournalJson, explorer, sections, journal, function, tutorial);
                }
                else
                {
                    MessageBox.Show("Поле данные пустое!", "Уведомление");
                }
            }
            else
            {
                MessageBox.Show("Выберете путь к файлу", "Уведомление");
            }
        }

        private void clearEnter(object sender, RoutedEventArgs e)
        {
           StringBuilder text = new StringBuilder(pageHtml.Text);
           string new_text = "";
           for (int i = 0; i<text.Length; i++)
           {
                string check_text = text[i].ToString();
                if (check_text != "\n")
                {
                    new_text += check_text;
                }

           }
           pageHtml.Text = new_text;
           MessageBox.Show("Очистка данных закончена!", "Уведомление");
        }

        private void relog(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Внимание! Приложение будет перезапущено\nПерезапустить приложение", "Уведомление", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            else
            {
                MessageBox.Show("Возврат в приложение", "Уведомление");
            }
        }
    }
}
