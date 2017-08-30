

namespace ForeignExchange.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Models;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using System;
    using System.ComponentModel;
    using System.Net.Http;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using Xamarin.Forms;

    public class MainViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        bool _isRunning;
        bool _isEnabled;
        string _result;
        ObservableCollection<Rate> _rates;
        #endregion

        #region Properties
        public string Amount {
            get;
            set;
        }
        public ObservableCollection<Rate> Rates {
            get
            {
                return _rates;
            }
            set
            {
                if (_rates != value)
                {
                    _rates = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(Rates)));
                }
            }
        }
        public Rate SourceRate {
            get;
            set;
        }
        public Rate TargetRate {
            get;
            set;
        }
        public bool IsRunning {
            get
            {
                return _isRunning;
            }
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsRunning)));
                }
            }
        }
        public bool IsEnabled {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsEnabled)));
                }
            }
        }
        public string  Result {
            get
            {
                return _result;
            }
            set
            {
                if (_result != value)
                {
                    _result = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(Result)));
                }
            }
        }
        //public int MyProperty { get; set; }
        #endregion

        #region Constructors
        public MainViewModel()
        {
            LoadRates();
        }

      
        #endregion

        #region Methods
        async void LoadRates()
        {
            IsRunning = true;
            Result = "Loading rates...";
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://apiexchangerates.azurewebsites.net");
                var controller = "/api/rates";
                var response = await client.GetAsync(controller);
                var result = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    IsRunning = false;
                    Result = result;
                }

                var rates = JsonConvert.DeserializeObject<List<Rate>>(result);
                Rates = new ObservableCollection<Rate>(rates);
                IsRunning = false;
                Result = "Ready to convert";
                IsEnabled = true;
            }
            catch (Exception ex)
            {
                IsRunning = false;
                Result = ex.Message;
            }
        }
        #endregion

        #region Commands
        public ICommand ConvertCommand
        {
            get
            {
                return new RelayCommand(Convert);
            }
        }

        async void Convert()
        {
            if (string.IsNullOrEmpty(Amount))
            {
                await Application.Current.MainPage.DisplayAlert("Eror",
                    "You must enter a value i amount.",
                    "Accept");
                return;
            }

            decimal amount = 0;
            if (!decimal.TryParse(Amount, out amount))
            {
                await Application.Current.MainPage.DisplayAlert("Eror",
                                    "You must enter a numeric value i amount.",
                                    "Accept");
                return;
            }
            if (SourceRate == null)
            {
                await Application.Current.MainPage.DisplayAlert("Eror",
                    "You must select a source rate.",
                    "Accept");
                return;
            }

            if (TargetRate == null)
            {
                await Application.Current.MainPage.DisplayAlert("Eror",
                    "You must enter a target i amount.",
                    "Accept");
                return;
            }

            var amountConverted = amount / 
                                          (decimal)SourceRate.TaxRate * 
                                          (decimal)TargetRate.TaxRate;
            Result = string.Format("{0} {1:C2} = {2} {3:C2}", 
                SourceRate.Code, 
                amount, TargetRate.Code, 
                amountConverted);
        }
        #endregion

        #region Properties
        #endregion

        #region Properties
        #endregion

        #region Properties
        #endregion

        #region Properties
        #endregion
       
    }
}
