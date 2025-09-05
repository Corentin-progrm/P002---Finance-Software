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

            // On définit le fichier JSON unique dans le dossier local de l'app
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                "donnees.json", CreationCollisionOption.OpenIfExists);

            List<ModelLignesDepenses> anciennesDonnees = new();

            try
            {
                string jsonContent = await FileIO.ReadTextAsync(file);
                anciennesDonnees = JsonSerializer.Deserialize<List<ModelLignesDepenses>>(jsonContent) ?? new();
            }
            catch
            {
                // Si le fichier est vide ou corrompu
                anciennesDonnees = new();
            }

            // Fusionner les anciennes + nouvelles données
            anciennesDonnees.AddRange(Donnees);

            // Sauvegarder le fichier mis à jour
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
                        Date = row.Cell(1).GetValue<string>(),
                        Prix = row.Cell(2).GetValue<string>(),
                        Description = row.Cell(3).GetValue<string>(),
                        Personne = row.Cell(4).GetValue<string>(),
                    });
                }
            }
        }
    }
}
