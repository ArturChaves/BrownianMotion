using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using BrownianMotion.ViewModels;

namespace BrownianMotion;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        _viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(_viewModel.AllSimulations))
                ChartView.InvalidateSurface();
        };
    }


    private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        var allSimulations = _viewModel.AllSimulations;
        if (allSimulations == null || !allSimulations.Any()) return;

        float width = e.Info.Width;
        float height = e.Info.Height;

        double globalMax = allSimulations.Max(sim => sim.Max());
        double globalMin = allSimulations.Min(sim => sim.Min());
        double globalRange = globalMax - globalMin;

        // Margens para eixos
        float marginLeft = 60;
        float marginBottom = 40;

        var graphWidth = width - marginLeft - 10;
        var graphHeight = height - marginBottom - 10;

        // Linhas de grade
        var gridPaint = new SKPaint
        {
            Color = SKColors.Gray,
            StrokeWidth = 1,
            PathEffect = SKPathEffect.CreateDash(new float[] { 10, 10 }, 0)
        };

        // Fonte base
        var labelFont = new SKFont(SKTypeface.Default, 18);
        var axisFont = new SKFont(SKTypeface.Default, 12);

        // Paint dos textos dos eixos
        var yAxisLabelPaint = new SKPaint
        {
            Color = SKColors.White,
            IsAntialias = true,
            FakeBoldText = true
        };

        var xAxisLabelPaint = new SKPaint
        {
            Color = SKColors.White,
            IsAntialias = true,
            FakeBoldText = true,
            TextAlign = SKTextAlign.Center
        };

        // Eixo Y (preço)
        int yLabels = 5;
        for (int i = 0; i <= yLabels; i++)
        {
            float y = 10 + graphHeight - (graphHeight / yLabels) * i;
            canvas.DrawLine(marginLeft, y, width, y, gridPaint);

            var priceLabel = globalMin + (globalRange / yLabels) * i;

            var labelPaint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true
            };

            canvas.DrawText($"{priceLabel:F2}", 5, y + 5, labelFont, labelPaint);
        }

        // Eixo X (tempo)
        int xLabels = 5;
        for (int i = 0; i <= xLabels; i++)
        {
            float x = marginLeft + (graphWidth / xLabels) * i;
            canvas.DrawLine(x, 10, x, height - marginBottom, gridPaint);

            var dayLabel = (_viewModel.NumDays / xLabels) * i;

            var labelPaint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center
            };

            canvas.DrawText($"{dayLabel}", x, height - marginBottom + 20, labelFont, labelPaint);
        }

        // Texto rotacionado do eixo Y ("Preço")
        canvas.Save();

        float yLabelX = 20; // distância da borda esquerda
        float yLabelY = 10 + graphHeight / 2 + 20; // centro do gráfico verticalmente

        canvas.Translate(yLabelX, yLabelY);
        canvas.RotateDegrees(-90);

        canvas.DrawText("Preço (R$)", 0, 0, axisFont, yAxisLabelPaint);

        canvas.Restore();

        // Texto do eixo X ("Tempo (dias)")
        canvas.DrawText("Tempo (dias)", marginLeft + graphWidth / 2, height - 5, axisFont, xAxisLabelPaint);



        // Cores das simulações
        SKColor[] colors = { SKColors.LightBlue, SKColors.Yellow, SKColors.Orange, SKColors.Green, SKColors.Purple };
        int colorIndex = 0;

        foreach (var prices in allSimulations)
        {
            var paint = new SKPaint
            {
                Color = colors[colorIndex % colors.Length],
                StrokeWidth = 2,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            var path = new SKPath();

            for (int i = 0; i < prices.Length; i++)
            {
                float x = marginLeft + (float)i / (prices.Length - 1) * graphWidth;
                float y = 10 + graphHeight - (float)((prices[i] - globalMin) / globalRange * graphHeight);

                if (i == 0)
                    path.MoveTo(x, y);
                else
                    path.LineTo(x, y);
            }

            canvas.DrawPath(path, paint);
            colorIndex++;
        }
    }


}
