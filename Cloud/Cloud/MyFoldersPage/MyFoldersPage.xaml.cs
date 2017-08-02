﻿using Microsoft.Office.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;

namespace Cloud.MyFoldersPage
{
    /// <summary>
    /// Interaction logic for MyFoldersPage.xaml
    /// </summary>
    public partial class MyFoldersPage : Page
    {

        private List<String> newList = new List<String>();
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Chester\Documents\test_db.mdf;Integrated Security=True;Connect Timeout=30");
        string dtformat = "yyyy-MM-dd HH:mm:ss";
        DataTable dt = new DataTable();

        //DEFAULT CONSTRUCTOR
        public MyFoldersPage()
        {
            InitializeComponent();

            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };

            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select docName, fileSize, lastModified from [table] where isDeleted = 'no'";
            cmd.ExecuteNonQuery();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            listView.ItemsSource = dt.DefaultView;
            con.Close();
        }


        //SEARCH FUNCTION
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            dt.DefaultView.RowFilter = String.Format("docName LIKE '%{0}%'", searchBar.Text);
        }


        //AESTHETICS FOR SEARCH BOX
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }


        //RIGHTCLICK -> OPEN
        private void openClick(object sender, RoutedEventArgs e)
        {
            String theText = "";
            String selectedText = ((DataRowView)((ListView)sender).SelectedItem)["docName"].ToString();
            fileName.Content = selectedText;
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select text from [table] where docName = '" + selectedText + "'";
            using (SqlDataReader read = cmd.ExecuteReader())
            {
                while (read.Read())
                {
                    byte[] retrieve = ((byte[])read["text"]);
                    theText = System.Text.Encoding.ASCII.GetString(retrieve);
                }
            }

            con.Close();

            byte[] byteArray = Encoding.ASCII.GetBytes(theText);

            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                TextRange tr = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                tr.Load(ms, DataFormats.Rtf);

            }

            listView.Visibility = System.Windows.Visibility.Hidden;
            mainToolBar.Visibility = System.Windows.Visibility.Hidden;
            rtbEditor.Visibility = System.Windows.Visibility.Visible;
            secondToolBar.Visibility = System.Windows.Visibility.Visible;
        }


        //RIGHTCLICK -> FAVORITE
        private void favoriteClick(object sender, RoutedEventArgs e)
        {
            String selectedText = ((DataRowView)listView.SelectedItem)["docName"].ToString();
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "update [table] set isFavorite = 'yes' where docName = '" + selectedText + "'";
            cmd.ExecuteNonQuery();
            con.Close();
        }


        //RIGHTCLICK -> DELETE
        private void deleteClick(object sender, RoutedEventArgs e)
        {
            String selectedText = ((DataRowView)listView.SelectedItem)["docName"].ToString();
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "update [table] set isDeleted = 'yes' where docName = '" + selectedText + "'";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "select docName, fileSize, lastModified from [table] where isDeleted = 'no'";
            cmd.ExecuteNonQuery();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            dt.Clear();
            listView.ItemsSource = dt.DefaultView;
            da.Fill(dt);
            listView.ItemsSource = dt.DefaultView;
            con.Close();
        }


        void p_Exited(object sender, EventArgs e)
        {
            File.Delete("temp.ppt");
        }


        //CLICK ON LIST VIEW ITEM TO OPEN
        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            String theText = "";
            String ext = "";
            String selectedText = ((DataRowView)((ListView)sender).SelectedItem)["docName"].ToString();
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select fileType from [table] where docName = '" + selectedText + "'";
            using (SqlDataReader read = cmd.ExecuteReader())
            {
                while (read.Read())
                {
                    ext = (read["fileType"].ToString());
                }
            }

            if (ext == ".doc" || ext == ".docx")
            {
                fileName.Content = selectedText;
                cmd.CommandText = "select text from [table] where docName = '" + selectedText + "'";
                using (SqlDataReader read = cmd.ExecuteReader())
                {
                    while (read.Read())
                    {
                        byte[] retrieve = ((byte[])read["text"]);
                        theText = System.Text.Encoding.ASCII.GetString(retrieve);
                    }
                }

                con.Close();

                byte[] byteArray = Encoding.ASCII.GetBytes(theText);

                using (MemoryStream ms = new MemoryStream(byteArray))
                {
                    TextRange tr = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                    tr.Load(ms, DataFormats.Rtf);

                }

                listView.Visibility = System.Windows.Visibility.Hidden;
                mainToolBar.Visibility = System.Windows.Visibility.Hidden;
                rtbEditor.Visibility = System.Windows.Visibility.Visible;
                secondToolBar.Visibility = System.Windows.Visibility.Visible;
            }

            else if (ext == ".ppt" || ext == ".pptx")
            {
                cmd.CommandText = "select text from [table] where docName = '" + selectedText + "'";
                using (SqlDataReader read = cmd.ExecuteReader())
                {
                    while (read.Read())
                    {
                        byte[] retrieve = ((byte[])read["text"]);
                        File.WriteAllBytes("temp.ppt", retrieve);
                        Process process = new Process();
                        process.StartInfo.FileName = "temp.ppt";
                        process.StartInfo.UseShellExecute = true;
                        process.Start();
                        process.EnableRaisingEvents = true;
                        process.Exited += new EventHandler(p_Exited);
                    }
                }

                con.Close();

                
            }
        }

        
        //CREATE NEW FILE
        private void newTextBtn_Click(object sender, RoutedEventArgs e)
        {
            NewFile.Visibility = System.Windows.Visibility.Visible;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            NewFile.Visibility = System.Windows.Visibility.Collapsed;

            DateTime current = new DateTime();
            current = DateTime.Now;

            String input = textbox1.Text;
            fileName.Content = input;
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "insert into [table] values('" + textbox1.Text + "', '', '', '" + current.ToString(dtformat) + "', 'no', 'no')";
            cmd.ExecuteNonQuery();
            con.Close();
            textbox1.Text = String.Empty;

            listView.Visibility = System.Windows.Visibility.Hidden;
            mainToolBar.Visibility = System.Windows.Visibility.Hidden;
            rtbEditor.Visibility = System.Windows.Visibility.Visible;
            secondToolBar.Visibility = System.Windows.Visibility.Visible;
        }


        //CANCEL 
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NewFile.Visibility = System.Windows.Visibility.Collapsed;
            textbox1.Text = String.Empty;
        }


        //SET FONT FAMILY THINGS
        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
        }

        
        //SET FONT SIZE THINGS
        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
        }


        //TEXT EDITOR FUNCTIONS
        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = rtbEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
            btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
            btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            temp = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            cmbFontFamily.SelectedItem = temp;
            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
            cmbFontSize.Text = temp.ToString();
        }


        //COLORPICKER
        private void colorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            rtbEditor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom(colorPicker.SelectedColor.ToString())));
        }


        //SAVE ANY CHANGES MADE TO DOCUMENT
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            String rtfText;
            TextRange tr = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            using (MemoryStream ms = new MemoryStream())
            {
                tr.Save(ms, DataFormats.Rtf);
                rtfText = Encoding.ASCII.GetString(ms.ToArray());
            }

            byte[] byteArray = Encoding.ASCII.GetBytes(rtfText);
            double fileSize = System.Text.ASCIIEncoding.ASCII.GetByteCount(rtfText);

            var culture = CultureInfo.CurrentUICulture;
            const String format = "#,0.0";
            String fileSizeDisplayed = "";

            if (fileSize < 1024)
            {
                fileSizeDisplayed = fileSize.ToString("#,0", culture) + " bytes";
            }

            else
            {
                fileSize /= 1024;

                if (fileSize < 1024)
                {
                    fileSizeDisplayed = fileSize.ToString(format, culture) + " KB";
                }

                else
                {
                    fileSize /= 1024;

                    if (fileSize < 1024)
                    {
                        fileSizeDisplayed = fileSize.ToString(format, culture) + " MB";
                    }

                    else
                    {
                        fileSize /= 1024;

                        if (fileSize < 1024)
                        {
                            fileSizeDisplayed = fileSize.ToString(format, culture) + " GB";
                        }

                        else
                        {
                            fileSize /= 1024;

                            if (fileSize < 1024)
                            {
                                fileSizeDisplayed = fileSize.ToString(format, culture) + " TB";
                            }
                        }
                    }
                }
            }

            DateTime current = new DateTime();
            current = DateTime.Now;
            

            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "update [table] set text = '" + rtfText + "', fileSize = '" + fileSizeDisplayed + "', lastModified = '" + current.ToString(dtformat) + "' where docName = '" + fileName.Content + "'";
            cmd.ExecuteNonQuery();
            using (var sqlWrite = new SqlCommand("update [table] set text = '" + byteArray + "', fileSize = '" + fileSizeDisplayed + "', lastModified = '" + current.ToString(dtformat) + "' where docName = '" + fileName.Content + "'", con))
            {
                SqlParameter file2 = new SqlParameter("@File", byteArray);
                sqlWrite.Parameters.Add(file2);
                sqlWrite.ExecuteNonQuery();
            }
            con.Close();
            MessageBox.Show("Save was done.");
        }


        //DOWNLOAD FILE AS MS WORD DOC
        public void TestDownload(object sender, RoutedEventArgs e)
        {
            
            String theText = "";
            String selectedText = ((DataRowView)listView.SelectedItem)["docName"].ToString();
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            object nullobject = System.Reflection.Missing.Value;
            object start = 0;
            Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Add(ref nullobject, ref nullobject, ref nullobject, ref nullobject);
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select text from [table] where docName = '" + selectedText + "'";
            using (SqlDataReader read = cmd.ExecuteReader())
            {
                while (read.Read())
                {
                    byte[] retrieve = ((byte[])read["text"]);
                    theText = System.Text.Encoding.ASCII.GetString(retrieve);
                }
            }
            con.Close();
            Clipboard.SetText(theText, TextDataFormat.Rtf);
            Microsoft.Office.Interop.Word.Range rng = doc.Range(ref start, ref nullobject);

            try
            {
                wordApp.Selection.Paste();
                String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                object filename = path + @"\" + selectedText + ".doc";
                doc.SaveAs2(filename);

            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            MessageBox.Show("Downloaded.");

        }

        private void powerpnt_SlideShowEnd(Microsoft.Office.Interop.PowerPoint.Presentation Pres)
        {
            
        }

        //UPLOAD AND OPEN PPT FROM FILE EXPLORER
        public void openPPTBtn_Click(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = ".ppt";
            openFileDialog.Filter = "PowerPoint Presentations(*.ppt;*.pptx)|*.ppt;*.pptx";

            Nullable<bool> result = openFileDialog.ShowDialog();

            if(result == true)
            {
                String filename = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                String fullfilename = System.IO.Path.GetFullPath(openFileDialog.FileName);
                String extension = System.IO.Path.GetExtension(openFileDialog.FileName);

                byte[] file;
                using (var stream = new FileStream(fullfilename, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        file = reader.ReadBytes((int)stream.Length);
                    }
                }

                Process process = new Process();
                process.StartInfo.FileName = fullfilename;
                process.StartInfo.UseShellExecute = true;
                process.Start();


                double fileSize = file.Length;

                var culture = CultureInfo.CurrentUICulture;
                const String format = "#,0.0";
                String fileSizeDisplayed = "";

                if (fileSize < 1024)
                {
                    fileSizeDisplayed = fileSize.ToString("#,0", culture) + "bytes";
                }

                else
                {
                    fileSize /= 1024;

                    if (fileSize < 1024)
                    {
                        fileSizeDisplayed = fileSize.ToString(format, culture) + " KB";
                    }

                    else
                    {
                        fileSize /= 1024;

                        if (fileSize < 1024)
                        {
                            fileSizeDisplayed = fileSize.ToString(format, culture) + " MB";
                        }

                        else
                        {
                            fileSize /= 1024;

                            if (fileSize < 1024)
                            {
                                fileSizeDisplayed = fileSize.ToString(format, culture) + " GB";
                            }

                            else
                            {
                                fileSize /= 1024;

                                if (fileSize < 1024)
                                {
                                    fileSizeDisplayed = fileSize.ToString(format, culture) + " TB";
                                }
                            }
                        }
                    }
                }

                DateTime current = new DateTime();
                current = DateTime.Now;

                con.Open();



                using (var sqlWrite = new SqlCommand("insert into [table] values(@FileName, @File, '" + fileSizeDisplayed + "', '" + current.ToString(dtformat) + "', 'no', 'no', '" + extension + "')", con))
                {
                    SqlParameter file2 = new SqlParameter("@File", file);
                    SqlParameter filename2 = new SqlParameter("@FileName", filename);
                    sqlWrite.Parameters.Add(filename2);
                    sqlWrite.Parameters.Add(file2);
                    sqlWrite.ExecuteNonQuery();
                }

                con.Close();
            }
        }

        //UPLOAD AND OPEN MS WORD DOC FROM FILE EXPLORER
        public void openMSBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = ".doc";
            openFileDialog.Filter = "Word Documents(*.doc;*.docx)|*.doc;*.docx";

            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
            {
                String filename = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                String fullfilename = System.IO.Path.GetFullPath(openFileDialog.FileName);
                Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

                object File = fullfilename;
                object nullobject = System.Reflection.Missing.Value;

                Microsoft.Office.Interop.Word.Application wordApp2 = new Microsoft.Office.Interop.Word.Application();
                wordApp2.DisplayAlerts = Microsoft.Office.Interop.Word.WdAlertLevel.wdAlertsNone;
                Microsoft.Office.Interop.Word.Document docs = wordApp.Documents.Open(ref File, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject);
                docs.ActiveWindow.Selection.WholeStory();
                docs.ActiveWindow.Selection.Copy();
                docs.Close();
                wordApp.Quit();

                fileName.Content = filename;
                rtbEditor.Document.Blocks.Clear();
                rtbEditor.Paste();

                String rtfText;
                TextRange tr = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                using (MemoryStream ms = new MemoryStream())
                {
                    tr.Save(ms, DataFormats.Rtf);
                    rtfText = Encoding.ASCII.GetString(ms.ToArray());
                }

                byte[] byteArray = Encoding.ASCII.GetBytes(rtfText);
                double fileSize = System.Text.ASCIIEncoding.ASCII.GetByteCount(rtfText);

                var culture = CultureInfo.CurrentUICulture;
                const String format = "#,0.0";
                String fileSizeDisplayed = "";

                if (fileSize < 1024)
                {
                    fileSizeDisplayed = fileSize.ToString("#,0", culture) + "bytes";
                }

                else
                {
                    fileSize /= 1024;

                    if (fileSize < 1024)
                    {
                        fileSizeDisplayed = fileSize.ToString(format, culture) + " KB";
                    }

                    else
                    {
                        fileSize /= 1024;

                        if (fileSize < 1024)
                        {
                            fileSizeDisplayed = fileSize.ToString(format, culture) + " MB";
                        }

                        else
                        {
                            fileSize /= 1024;

                            if (fileSize < 1024)
                            {
                                fileSizeDisplayed = fileSize.ToString(format, culture) + " GB";
                            }

                            else
                            {
                                fileSize /= 1024;

                                if (fileSize < 1024)
                                {
                                    fileSizeDisplayed = fileSize.ToString(format, culture) + " TB";
                                }
                            }
                        }
                    }
                }

                DateTime current = new DateTime();
                current = DateTime.Now;

                con.Open();
                using (var sqlWrite = new SqlCommand("insert into [table] values('" + filename +"', @File, '" + fileSizeDisplayed + "', '" + current.ToString(dtformat) + "', 'no', 'no')", con))
                {
                    SqlParameter file2 = new SqlParameter("@File", byteArray);
                    sqlWrite.Parameters.Add(file2);
                    sqlWrite.ExecuteNonQuery();
                }
                con.Close();

                listView.Visibility = System.Windows.Visibility.Collapsed;
                mainToolBar.Visibility = System.Windows.Visibility.Collapsed;
                rtbEditor.Visibility = System.Windows.Visibility.Visible;
                secondToolBar.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
