using System;
using System.ComponentModel;

namespace FoodResultEntry
{
    public class ResultToAdd : INotifyPropertyChanged
    {




        public long ResultId { get; set; }
        public string ClientName { get; set; }
        public decimal ResultValue { get; set; }
        public string ResultName { get; set; }
        public string SampleDescription { get; set; }
        public string Description
        {
            get;
            set;
        }
        public int _duplicates;
        public bool MaxValueIncrease { get; set; }
        public string SdgName { get; set; }
        public Nullable<long> TestId { get; set; }
        public Nullable<long> ResultTemplateId { get; set; }
        public int Dilution { get; set; }


        public int Duplicates
        {
            get { return _duplicates; }
            set
            {
                _duplicates = value;
                OnPropertyChanged("Duplicates");
            }
        }

        public decimal CalculateValueToSet()
        {
            if (MaxValueIncrease)
            {
                return 99999;
            }
            else
            {
                if (Dilution < 1)
                {
                    return ResultValue;
                }
                else
                {
                    double x = Convert.ToDouble(ResultValue) * Math.Pow(10, Dilution);
                    return Convert.ToDecimal(x);

                }
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}