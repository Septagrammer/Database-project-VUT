using System;
using System.Collections.Generic;
using System.IO;
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

namespace Project
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            foreach (var genre in Enum.GetValues(typeof(Band.Type)))
            {
                ListOfGenre.Items.Add(genre);
            }
            this.refresh();
        }

        public Project.DataClasses1DataContext context = new Project.DataClasses1DataContext();

        private void Search_Click(object sender, RoutedEventArgs e)
        {
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
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
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
            string txt = string.Empty;
            foreach (var row in context.Table)
            {
                txt += row.Name + ",";
                txt += row.Year + ",";
                txt += row.Genre + ",";
                txt += row.Songs + ",";
                txt += "\n";
            }
            string folderPath = "D:\\";
            File.WriteAllText(folderPath + "Database.txt", txt);
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            StreamReader reader = File.OpenText("D:\\Database.txt");
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                var rows = line.Split('\n');
                foreach (string row in rows)
                {
                    string[] parsed = row.Split(',');
                    Table band = new Table();
                    band.Name = parsed[0];
                    band.Year = parsed[1];
                    band.Genre = parsed[2];
                    band.Songs = parsed[3];
                    context.Table.InsertOnSubmit(band);
                    context.SubmitChanges();
                }

            }
            this.refresh();
            reader.Close();
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
            Table edited = new Table();
            edited.Name = NameText.Text;
            edited.Year = YearText.Text;
            edited.Songs = SongsText.Text;
            edited.Genre = ListOfGenre.SelectedItem.ToString();
            Table toEdit = (from Table in context.Table where Table.Name == edited.Name select Table).FirstOrDefault();
            NameText.Text = "";
            YearText.Text = "";
            SongsText.Text = "";
            if (toEdit == null)
            {
                return;
            }
            context.Table.DeleteOnSubmit(toEdit);
            context.SubmitChanges();
            context.Table.InsertOnSubmit(edited);
            context.SubmitChanges();
        }
    }
}
