﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MayVideoChat
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form = new Form1();

            SenderPresenter senderPresenter = new SenderPresenter(form);
            ReceiverPresenter receiverPresenter = new ReceiverPresenter(form);
            Application.Run(form);
        }
    }
}
