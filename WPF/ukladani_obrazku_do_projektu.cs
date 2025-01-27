private void Button_Click(object sender, RoutedEventArgs e)
{

    // Configure open file dialog box
    var dialog = new Microsoft.Win32.OpenFileDialog();
    dialog.FileName = "Image"; // Default file name
    dialog.DefaultExt = ".png"; // Default file extension
    dialog.Filter = "Obrázky (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Všechny soubory (*.*)|*.*"; // Filter files by extension

    // Show open file dialog box
    bool? result = dialog.ShowDialog();

    // Process open file dialog box results
    if (result == true)
    {
        // Open document
        string sourceFilePath = dialog.FileName;

        // Cíl: složka Resources ve vašem projektu
        string projectPath = Directory.GetCurrentDirectory(); // Aktuální adresář aplikace
        string resourcesPath = System.IO.Path.Combine(projectPath, "Resources");

        // Vytvoření složky Resources, pokud neexistuje
        if (!Directory.Exists(resourcesPath))
        {
            Directory.CreateDirectory(resourcesPath);
        }

        // Určení cílové cesty
        string fileName = System.IO.Path.GetFileName(sourceFilePath);
        string destinationFilePath = System.IO.Path.Combine(resourcesPath, fileName);

        // Kopírování souboru
        File.Copy(sourceFilePath, destinationFilePath, overwrite: true);


        using (StreamWriter sw = new StreamWriter("my.csv", true)) 
        {
            sw.WriteLine("{0},{1}",MyTextBox.Text, fileName);
        }
        string filename2 = "";
        using (StreamReader sr = new StreamReader("my.csv"))
        {
            filename2 = sr.ReadLine().Split(',')[1];
        }
        
        // Create a new BitmapImage
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.UriSource = new Uri(System.IO.Path.Combine(resourcesPath, filename2), UriKind.Absolute); // Load the file as an absolute URI
        bitmap.EndInit();

        // Assign the BitmapImage to the Image control
        MyImage.Source = bitmap;
    }
}