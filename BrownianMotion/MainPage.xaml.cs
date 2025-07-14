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
            if (e.PropertyName == nameof(_viewModel.Prices))
                ChartView.InvalidateSurface();
        };
    }

    private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear();

        var prices = _viewModel.Prices;
        if (prices == null || prices.Length < 2)
            return;

        float width = e.Info.Width;
        float height = e.Info.Height;

        double max = prices.Max();
        double min = prices.Min();
        double range = max - min;

        var paint = new SKPaint
        {
            Color = SKColors.Blue,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
            IsAntialias = true
        };

        var path = new SKPath();

        for (int i = 0; i < prices.Length; i++)
        {
            float x = (float)i / (prices.Length - 1) * width;
            float y = height - (float)((prices[i] - min) / range * height);

            if (i == 0)
                path.MoveTo(x, y);
            else
                path.LineTo(x, y);
        }

        canvas.DrawPath(path, paint);
    }
}
