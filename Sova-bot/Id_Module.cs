using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Args;

namespace Sova_bot
{
    class Id_Module
    {
        public void assignment_Chat_ID(CallbackQueryEventArgs ev,ref string[] ID_Message)
        {
            if (ID_Message.Contains(ev.CallbackQuery.Message.Chat.Id.ToString()))
            {
                Console.WriteLine("Существует");
            }
            else
            {
                Array.Resize(ref ID_Message, ID_Message.Length + 1);
                ID_Message[ID_Message.Length - 1] = ev.CallbackQuery.Message.Chat.Id.ToString();
            }
        }

        public void no_branching(MessageEventArgs e, string section, string[] ID_Message)
        {
            for (int i = 0; i < ID_Message.Length; i++)
            {
                if (ID_Message[i] == e.Message.Chat.Id.ToString() + section)
                {
                    ID_Message[i] = e.Message.Chat.Id.ToString();
                }
                Console.WriteLine(ID_Message[i]);
            }
        }

        public void branching(CallbackQueryEventArgs ev, string section,ref string[] ID_Message)
        {

            for (int i = 0; i < ID_Message.Length; i++)
            {
                if (ID_Message[i] == ev.CallbackQuery.Message.Chat.Id.ToString())
                {
                    ID_Message[i] = ev.CallbackQuery.Message.Chat.Id.ToString() + section;
                }
            }
            for (int i = 0; i < ID_Message.Length; i++)
            {
                Console.WriteLine(ID_Message[i]); //убрать проверку
            }
        }

        public void branching_section(MessageEventArgs e,ref string section, ref string[] ID_Message)
        {
            for (int i = 0; i < ID_Message.Length; i++)
            {
                if (ID_Message[i] == e.Message.Chat.Id.ToString() + " " + section.Split(' ')[1].ToLower())
                {
                    ID_Message[i] = e.Message.Chat.Id.ToString() + section;
                }
                Console.WriteLine(ID_Message[i]);
            }
            for (int i = 0; i < ID_Message.Length; i++)
            {
                Console.WriteLine(ID_Message[i]); //убрать проверку
            }
        }
    }
}
