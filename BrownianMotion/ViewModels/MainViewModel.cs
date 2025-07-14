using System.ComponentModel;
using System.Windows.Input;
using BrownianMotion.Services;

namespace BrownianMotion.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    public double InitialPrice { get; set; }
    public double Sigma { get; set; }
    public double Mean { get; set; }
    public int NumDays { get; set; }

    private double[] _prices;
    public double[] Prices
    {
        get => _prices;
        set
        {
            _prices = value;
            OnPropertyChanged(nameof(Prices));
        }
    }

    public ICommand SimulateCommand { get; }

    public MainViewModel()
    {
        SimulateCommand = new Command(Simulate);
    }

    private void Simulate()
    {
        if (InitialPrice <= 0 || Sigma <= 0 || NumDays <= 0)
        {
            Application.Current.MainPage.DisplayAlert("Erro", "Preencha todos os campos corretamente.", "OK");
            return;
        }


        double sigmaAnual = Sigma / 100.0;
        double meanAnual = Mean / 100.0;

        double sigmaDiario = sigmaAnual / Math.Sqrt(252);
        double meanDiario = meanAnual / 252;

        Prices = BrownianMotionService.GenerateBrownianMotion(
            sigmaDiario, meanDiario, InitialPrice, NumDays);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
