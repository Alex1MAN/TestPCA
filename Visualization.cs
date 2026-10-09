using MathNet.Numerics.LinearAlgebra;
using ScottPlot;

namespace TestPCA;

public class Visualization
{
    public void CreateScreePlot(double[] explainedVariance, string outputPath)
    {
        if (explainedVariance.Length == 0) throw new ArgumentException("Нет компонент.");
        var plot = new Plot();
        double[] xs = Enumerable.Range(1, explainedVariance.Length).Select(i => (double)i).ToArray();
        var bars = explainedVariance.Select((v, i) => new Bar
        {
            Position = xs[i], Value = v * 100,
            FillColor = Colors.Blue.WithAlpha(0.7)
        }).ToArray();
        plot.Add.Bars(bars);
        plot.Axes.Bottom.SetTicks(xs, xs.Select(x => $"PC{(int)x}").ToArray());
        plot.Axes.Left.Label.Text = "Объяснённая дисперсия (%)";
        plot.Axes.Bottom.Label.Text = "Главная компонента";
        plot.Title("Scree Plot — Объяснённая дисперсия по компонентам");
        plot.SavePng(outputPath, 800, 600);
    }

    public void CreateScoresPlot(Matrix<double> scores, string[] columnNames, string outputPath)
    {
        if (scores.ColumnCount < 2 || scores.RowCount == 0)
            throw new ArgumentException("Недостаточно данных для scores plot.");
        var plot = new Plot();
        var scatter = plot.Add.Scatter(scores.Column(0).ToArray(), scores.Column(1).ToArray());
        scatter.LineWidth = 0;
        scatter.MarkerSize = 6;
        scatter.MarkerShape = MarkerShape.FilledCircle;
        scatter.Color = Colors.Blue.WithAlpha(0.6);
        plot.Axes.Bottom.Label.Text = "PC1";
        plot.Axes.Left.Label.Text = "PC2";
        plot.Title("Scores Plot — Проекция наблюдений (PC1 vs PC2)");
        plot.Grid.IsVisible = true;
        plot.SavePng(outputPath, 800, 600);
    }

    public void CreateLoadingsPlot(Matrix<double> loadings, string[] columnNames, string outputPath)
    {
        if (loadings.ColumnCount < 2 || loadings.RowCount == 0)
            throw new ArgumentException("Недостаточно компонент для loadings plot.");
        if (columnNames.Length != loadings.RowCount)
            throw new ArgumentException("Количество названий не совпадает с количеством переменных.");
        var plot = new Plot();
        double[] pc1 = loadings.Column(0).ToArray();
        double[] pc2 = loadings.Column(1).ToArray();
        var scatter = plot.Add.Scatter(pc1, pc2);
        scatter.LineWidth = 0;
        scatter.MarkerSize = 8;
        scatter.MarkerShape = MarkerShape.FilledCircle;
        scatter.Color = Colors.Red.WithAlpha(0.7);
        for (int i = 0; i < columnNames.Length; i++)
            plot.Add.Text(columnNames[i], pc1[i], pc2[i]);
        plot.Add.Line(0, pc2.Min(), 0, pc2.Max());
        plot.Add.Line(pc1.Min(), 0, pc1.Max(), 0);
        plot.Axes.Bottom.Label.Text = "PC1 Loadings";
        plot.Axes.Left.Label.Text = "PC2 Loadings";
        plot.Title("Loadings Plot — Вклад переменных в главные компоненты");
        plot.Grid.IsVisible = true;
        plot.SavePng(outputPath, 800, 600);
    }

    public void CreateCumulativeVariancePlot(double[] cumulativeVariance, string outputPath)
    {
        if (cumulativeVariance.Length == 0) throw new ArgumentException("Нет компонент.");
        var plot = new Plot();
        double[] xs = Enumerable.Range(1, cumulativeVariance.Length).Select(i => (double)i).ToArray();
        double[] ys = cumulativeVariance.Select(v => v * 100).ToArray();
        var scatter = plot.Add.Scatter(xs, ys);
        scatter.LineWidth = 2;
        scatter.MarkerSize = 6;
        scatter.MarkerShape = MarkerShape.FilledCircle;
        scatter.Color = Colors.Green;
        plot.Add.HorizontalLine(95, 2, Colors.Gray, LinePattern.Dashed);
        plot.Add.Text("95%", xs.Last(), 95);
        plot.Axes.Bottom.SetTicks(xs, xs.Select(x => $"PC{(int)x}").ToArray());
        plot.Axes.Left.Label.Text = "Кумулятивная объяснённая дисперсия (%)";
        plot.Axes.Bottom.Label.Text = "Главная компонента";
        plot.Title("Кумулятивная объяснённая дисперсия");
        plot.Grid.IsVisible = true;
        plot.SavePng(outputPath, 800, 600);
    }
}
