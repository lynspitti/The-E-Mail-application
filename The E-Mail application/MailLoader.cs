﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Threading;
using System.Data.SQLite;
using System.Data;

namespace The_E_Mail_application
{
    /// <summary>
    /// Backgroundworker used to Load/Check mails from clients
    /// </summary>
    class MailLoader
    {
        //Loader and Timer
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        BackgroundWorker Loader = new BackgroundWorker();
        MainWindow Main_Window;
        SQLiteConnection m_dbConnection;

        /// <summary>
        /// Creats background threads
        /// </summary>
        /// <param name="Window">Interface window containing clients (ClientPanel)</param>
        public MailLoader(MainWindow Window)
        {
            Main_Window = Window;
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 30);

            Loader = new BackgroundWorker();
            Loader.DoWork += new DoWorkEventHandler(Loader_DoWork);
            
            Create_JoinDatabase();
            
            dispatcherTimer.Start();
        }
        
        /// <summary>
        /// Timer to update has trickered
        /// </summary>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            while (Loader.IsBusy || Loads > 0) { }
            Loader.RunWorkerAsync();
        }

        /// <summary>
        /// Update All clients mail list
        /// </summary>
        private void Loader_DoWork(object sender, DoWorkEventArgs e)
        {
            dispatcherTimer.Stop();
            App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate(){
                foreach (MailClient obj in Main_Window.ClientPanel.Children)
                {
                    if (obj is MailClient)
                    {
                        // start client update
                        MailClient client = obj as MailClient;
                        ThreadStart processTaskThread = delegate { Load_Mails(client); };
                        Thread thread = new Thread(processTaskThread);
                        thread.Start();
                    }
                }
            }));
            Thread.Sleep(0300);
            dispatcherTimer.Start();
        }

        /// <summary>
        /// Load mails from specefic client
        /// </summary>
        int Loads = 0;
        private void Load_Mails(MailClient client)
        {
            if (!client.IsConnected) return;
            Loads++;
            string[][] Mails = client.Load_Mail_List();
            if (Mails != null)
            {
                m_dbConnection.Open();
                foreach (string[] Mail in Mails)
                {
                    if (Mail == null) continue;
                    // insert new mails into database
                    SQLiteCommand insertSQL = new SQLiteCommand("INSERT INTO MailList (MessageId, Receiver, Sender, Date, Subject, Container) VALUES ('" + Mail[0] + "','" + client.UserEmail + "','" + Mail[1] + "','" + Mail[2] + "','" + Mail[3] + "','Inbox')", m_dbConnection);
                    try
                    {
                        insertSQL.ExecuteNonQuery();
                    }
                    catch
                    { }
                }
                m_dbConnection.Close();
            }

            App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate(){
                client.MailListMap_Selected(client.HeadUser.SelectedItem, new RoutedEventArgs());
            }));
            Loads--;
        }

        /// <summary>
        /// Create database / table if not existing
        /// else open connection
        /// </summary>
        private void Create_JoinDatabase()
        {
            #region create non existing database
            if (!System.IO.File.Exists("MyDatabase.sqlite"))
            {
                SQLiteConnection.CreateFile("MyDatabase.sqlite");
            }
            #endregion

            // open connection
            m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();

            #region create non existing table
            bool exists = false;
            DataTable dt = m_dbConnection.GetSchema("Tables");
            foreach (DataRow row in dt.Rows)
            {
                string tablename = (string)row[2];
                if (tablename == "MailList") exists = true;
            }

            if (!exists)
            {
                string sql = "CREATE TABLE MailList(MessageId varchar(50) not null, Receiver varchar(50), Sender varchar(50), Date varchar(20), Subject varchar(60), Container varchar(20), UNIQUE(MessageId))";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
            #endregion

            m_dbConnection.Close();
        }
    }
}
