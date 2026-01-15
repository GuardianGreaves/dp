using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace diplom_loskutova.Class
{
    public class PaginationViewModel : INotifyPropertyChanged
    {
        private int _currentPage = 1;
        private int _totalRecords = 0;
        private const int PAGE_SIZE = 15;

        public int CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(); UpdatePageInfo(); }
        }

        public int TotalRecords => _totalRecords;
        public int TotalPages => (int)Math.Ceiling((double)_totalRecords / PAGE_SIZE);
        public bool CanGoPrevious => CurrentPage > 1;
        public bool CanGoNext => CurrentPage < TotalPages;

        public string CurrentPageDisplay => $"Страница {CurrentPage}";
        public string PageInfo => $"Записей {GetStartRecord()}–{GetEndRecord()} из {TotalRecords}";

        public ICommand PreviousCommand { get; }
        public ICommand NextCommand { get; }

        public PaginationViewModel()
        {
            PreviousCommand = new RelayCommand(Previous, () => CanGoPrevious);
            NextCommand = new RelayCommand(Next, () => CanGoNext);
        }

        private void Previous() => CurrentPage--;
        private void Next() => CurrentPage++;

        private int GetStartRecord() => (CurrentPage - 1) * PAGE_SIZE + 1;
        private int GetEndRecord() => Math.Min(CurrentPage * PAGE_SIZE, TotalRecords);

        public void UpdatePageInfo()
        {
            OnPropertyChanged(nameof(CurrentPageDisplay));
            OnPropertyChanged(nameof(PageInfo));
            OnPropertyChanged(nameof(CanGoPrevious));
            OnPropertyChanged(nameof(CanGoNext));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
