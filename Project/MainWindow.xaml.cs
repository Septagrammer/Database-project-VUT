using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Project
{
    public partial class MainWindow : Window
    {

        public enum Type
        {
            Rock,
            Classic,
            Jazz,
            Pop,
            Electronic
        }

        public MainWindow()
        {
            InitializeComponent();
            foreach (var genre in Enum.GetValues(typeof(Type)))
            {
                ListOfGenre.Items.Add(genre);
            }
            this.refresh();
        }

        public Project.DataClasses1DataContext context = new Project.DataClasses1DataContext();

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "")
            {
                MessageBox.Show("You shoud write some text to search");
                refresh();
                return;
            }
            var bands = context.Table;
            List<Table> bandsRes = new List<Table>();
            foreach (var band in bands)
            {
                if (band.Name.Contains(SearchBox.Text))
                {
                    bandsRes.Add(band);
                }
            }
            Display.ItemsSource = bandsRes;
            SearchBox.Text = "";
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            if ((NameText.Text == "") || (YearText.Text == "") || (YearText.Text == "") || (ListOfGenre.SelectedItem.ToString() == ""))
            {
                MessageBox.Show("Please fill in all fields to create new entry");
                return;
            }
            Table band = new Table();
            band.Name = NameText.Text;
            band.Year = YearText.Text;
            band.Songs = SongsText.Text;
            band.Genre = ListOfGenre.SelectedItem.ToString();
            context.Table.InsertOnSubmit(band);
            context.SubmitChanges();
            NameText.Text = "";
            YearText.Text = "";
            SongsText.Text = "";
            this.refresh();
        }

        public void refresh()
        {
            List<Table> bands = new List<Table>();

            foreach (var item in context.Table)
            {
                Table band = new Table();
                band.Name = item.Name;
                band.Year = item.Year;
                band.Songs = item.Songs;
                band.Genre = item.Genre;
                bands.Add(band);
            }
            Display.ItemsSource = bands;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var csv = new StringBuilder();
            foreach (var row in context.Table)
            {
                var line = string.Format("{0};{1};{2};{3}", row.Name, row.Year, row.Genre, row.Songs);
                csv.AppendLine(line);
            }
            File.WriteAllText("Database.csv", csv.ToString());
            MessageBox.Show("Database successfully exported into Database.csv");
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(@"Database.csv"))
            {
                MessageBox.Show("There is no database to import");
                return;
            }
            StreamReader reader = File.OpenText("Database.csv");
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                var rows = line.Split('\n');
                foreach (string row in rows)
                {
                    string[] parsed = row.Split(';');
                    Table band = new Table();
                    band.Name = parsed[0];
                    band.Year = parsed[1];
                    band.Genre = parsed[2];
                    band.Songs = parsed[3];

                    Table ifEx = (from Table in context.Table where Table.Name == band.Name select Table).FirstOrDefault();
                    if (ifEx == null)
                    {
                        context.Table.InsertOnSubmit(band);
                        context.SubmitChanges();
                    }
                }
            }
            this.refresh();
            reader.Close();
            MessageBox.Show("Database was successfully imported from Database.csv");
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Table selected = (Table)Display.SelectedItem;
            Table toDel = (from Table in context.Table where Table.Name == selected.Name select Table).FirstOrDefault();
            context.Table.DeleteOnSubmit(toDel);
            context.SubmitChanges();
            this.refresh();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if ((NameText.Text == "") || (YearText.Text == "") || (YearText.Text == "") || (ListOfGenre.SelectedItem.ToString() == ""))
            {
                MessageBox.Show("Please fill in all fields to edit entry");
                return;
            }

            Table selected = (Table)Display.SelectedItem;
            Table toDel = (from Table in context.Table where Table.Name == selected.Name select Table).FirstOrDefault();

            Table edited = new Table();
            edited.Name = NameText.Text;
            edited.Year = YearText.Text;
            edited.Songs = SongsText.Text;
            edited.Genre = ListOfGenre.SelectedItem.ToString();

            NameText.Text = "";
            YearText.Text = "";
            SongsText.Text = "";

            if (selected == null)
            {
                return;
            }

            context.Table.DeleteOnSubmit(toDel);
            context.SubmitChanges();
            context.Table.InsertOnSubmit(edited);
            context.SubmitChanges();
            refresh();
        }
    }
}
