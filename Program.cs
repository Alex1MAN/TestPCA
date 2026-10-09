using TestPCA;

Console.WriteLine("=== PCA Console Application ===\n");

// Путь к файлу с данными (измените на свой)
string dataPath = args.Length > 0 ? args[0] : "NiCu100.xlsx";

if (!File.Exists(dataPath))
{
    Console.WriteLine($"Файл '{dataPath}' не найден!");
    return;
}

try
{
    // 1. Загрузка и подготовка данных
    Console.WriteLine($"Загрузка данных из {dataPath}...");
    var dataProcessor = new DataProcessor();
    var rawData = dataProcessor.LoadDataFromExcel(dataPath);

    Console.WriteLine($"Загружено строк: {rawData.RowCount}");
    Console.WriteLine($"Количество столбцов: {rawData.ColumnCount}");
    Console.WriteLine($"Столбцы: {string.Join(", ", rawData.ColumnNames)}\n");

    // 2. Построение PCA модели
    Console.WriteLine("Построение PCA модели...");
    var pcaModel = new PcaModel(rawData);
    pcaModel.Fit();

    Console.WriteLine($"Компоненты: {pcaModel.ComponentsCount}");
    Console.WriteLine($"Объяснённая дисперсия (первые 3 PC):");
    for (int i = 0; i < Math.Min(3, pcaModel.ExplainedVarianceRatio.Length); i++)
    {
        Console.WriteLine($"  PC{i + 1}: {pcaModel.ExplainedVarianceRatio[i]:F4} " +
                         $"({pcaModel.ExplainedVarianceRatio[i] * 100:F2}%)");
    }
    Console.WriteLine();

    // 3. Визуализация для контроля
    Console.WriteLine("Создание визуализации...");
    var visualization = new Visualization();

    // Сохраняем графики в папку output
    string outputDir = "output";
    if (!Directory.Exists(outputDir))
        Directory.CreateDirectory(outputDir);

    // График 1: Scree plot (объяснённая дисперсия по компонентам)
    string screePlotPath = Path.Combine(outputDir, "scree_plot.png");
    visualization.CreateScreePlot(pcaModel.ExplainedVarianceRatio, screePlotPath);
    Console.WriteLine($"Scree plot сохранён: {screePlotPath}");

    // График 2: Scores plot (PC1 vs PC2)
    string scoresPlotPath = Path.Combine(outputDir, "scores_plot.png");
    visualization.CreateScoresPlot(pcaModel.Scores, rawData.ColumnNames, scoresPlotPath);
    Console.WriteLine($"Scores plot сохранён: {scoresPlotPath}");

    // График 3: Loadings plot (вклад исходных переменных)
    string loadingsPlotPath = Path.Combine(outputDir, "loadings_plot.png");
    visualization.CreateLoadingsPlot(pcaModel.Loadings, rawData.ColumnNames, loadingsPlotPath);
    Console.WriteLine($"Loadings plot сохранён: {loadingsPlotPath}");

    // 4. Вывод сводки для быстрой проверки
    Console.WriteLine("\n=== Сводка для проверки ===");
    Console.WriteLine($"Суммарная объяснённая дисперсия (2 PC): " +
                     $"{pcaModel.CumulativeVariance[1]:F4} ({pcaModel.CumulativeVariance[1] * 100:F2}%)");
    Console.WriteLine($"Суммарная объяснённая дисперсия (3 PC): " +
                     $"{pcaModel.CumulativeVariance[Math.Min(2, pcaModel.CumulativeVariance.Length - 1)]:F4}");

    Console.WriteLine("\nГотово! Проверьте папку 'output' для графиков.");
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
    Console.WriteLine($"Детали: {ex.StackTrace}");
}