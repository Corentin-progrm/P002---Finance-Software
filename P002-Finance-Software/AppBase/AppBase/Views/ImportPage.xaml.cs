using AppBase.Models;
using AppBase;
using ClosedXML.Excel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;


namespace AppBase.Views
{
    public sealed partial class ImportPage : Page
    {
        public ObservableCollection<ModelLignesDepenses> Donnees { get; set; } = new();

        public List<string> Categories { get; set; } = new() { "ELE", "LOG", "MEC" };

        public ImportPage()
        {
            this.InitializeComponent();
            DataGridExcel.ItemsSource = Donnees;
        }

        private async void OnSaveJsonClick(object sender, RoutedEventArgs e)
        {
            if (Donnees == null || Donnees.Count == 0)
            {
                ContentDialog noDataDialog = new()
                {
                    Title = "Aucune donnée",
                    Content = "Aucune donnée n'a été importée.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await noDataDialog.ShowAsync();
                return;
            }

            // Récupérer le chemin du dossier depuis les settings
            var localSettings = ApplicationData.Current.LocalSettings;
            string folderPath = localSettings.Values["DataFolder"] as string;

            StorageFolder folder;
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                // Si pas de dossier défini, demander à l'utilisateur
                var picker = new FolderPicker();
                var hwnd = WindowNative.GetWindowHandle(App.MainWindowInstance);
                InitializeWithWindow.Initialize(picker, hwnd);

                var pickedFolder = await picker.PickSingleFolderAsync();
                if (pickedFolder == null)
                {
                    return; // L'utilisateur a annulé
                }
                folder = pickedFolder;

                // Sauvegarder le chemin choisi dans les settings
                localSettings.Values["DataFolder"] = folder.Path;
            }
            else
            {
                folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            }

            // Créer ou ouvrir le fichier JSON
            StorageFile file = await folder.CreateFileAsync(
                "import.json", CreationCollisionOption.OpenIfExists);

            List<ModelLignesDepenses> anciennesDonnees = new();
            try
            {
                string jsonContent = await FileIO.ReadTextAsync(file);
                anciennesDonnees = JsonSerializer.Deserialize<List<ModelLignesDepenses>>(jsonContent) ?? new();
            }
            catch
            {
                anciennesDonnees = new();
            }

            anciennesDonnees.AddRange(Donnees);

            string newJson = JsonSerializer.Serialize(anciennesDonnees, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await FileIO.WriteTextAsync(file, newJson);

            ContentDialog successDialog = new()
            {
                Title = "Succès",
                Content = $"Les données ont été sauvegardées dans {file.Path}",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await successDialog.ShowAsync();
        }


        private async void OnOpenFileClick(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            var hwnd = WindowNative.GetWindowHandle(App.MainWindowInstance);
            InitializeWithWindow.Initialize(picker, hwnd);


            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".xlsx");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                using var stream = await file.OpenStreamForReadAsync();
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);

                Donnees.Clear();
                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    Donnees.Add(new ModelLignesDepenses
                    {
                        TypeDeMontant = row.Cell(1).GetValue<string>(),
                        DateReport = row.Cell(2).GetValue<string>(),
                        // Document = row.Cell(3).GetValue<string>(),
                        Type = row.Cell(4).GetValue<string>(),
                        DescEntiteExterne = row.Cell(5).GetValue<string>(),
                        // EntiteExterne = row.Cell(6).GetValue<string>(),
                        // Reference = row.Cell(7).GetValue<string>(),
                        Description = row.Cell(8).GetValue<string>(),
                        Date = row.Cell(9).GetValue<string>(),
                        UBR = row.Cell(10).GetValue<string>(),
                        Impact = row.Cell(11).GetValue<string>(),
                        MontantDoc = row.Cell(12).GetValue<string>(),

                    });
                }
            }
        }
    }
}


