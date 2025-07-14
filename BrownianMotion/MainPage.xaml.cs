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

        if (allSimulations == null || !allSimulations.Any())
            return;

        float width = e.Info.Width;
        float height = e.Info.Height;

        // Obter o valor máximo e mínimo global de todas as simulações
        double globalMax = allSimulations.Max(sim => sim.Max());
        double globalMin = allSimulations.Min(sim => sim.Min());
        double globalRange = globalMax - globalMin;

        // Definir cores diferentes para cada simulação
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
                float x = (float)i / (prices.Length - 1) * width;
                float y = height - (float)((prices[i] - globalMin) / globalRange * height);

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
