using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Runtime.CompilerServices;

namespace Player_v2
{
    //internal -- доступен классам-потомкам и в других сборках.
    internal class PlayerViewModel : INotifyPropertyChanged
    {
        string? playerPath = @"C:\Users\jakej\Downloads\Fast Typing.mp4";
        // Здесь хранится путь до видео. С помощью привязки данных будет помещаться в атрибут Source у MediaElement.
        public string? PlayerPath
        {
            get => playerPath;
            set
            {
                playerPath = value;
                OnPropertyChanged("PlayerPath");
            }
        }
        //теперь добавим свойства видимости и доступности для элементов Button1, ButtonPlay, ButtonStop и Text1. Первую и последнюю по умолчанию видно, остальные -- нет. Ещё добавим такие же свойства для плеера
        //Visibility-атрибут представляет собой перечисление enum, где значения Visible=0, Hidden=1 и они типа byte. Поэтому хранить их и биндить их нужно в таком же формате.
        Visibility buttonVisible = Visibility.Visible;
        public Visibility ButtonVisible
        {
            get => buttonVisible;
            set
            {
                buttonVisible = value;
                OnPropertyChanged("ButtonVisible");
            }
        }

        bool buttonEnable = true;
        public bool ButtonEnable
        {
            get => buttonEnable;
            set
            {
                buttonEnable = value;
                OnPropertyChanged("ButtonEnable");
            }
        }

        Visibility buttonPlayVisible = Visibility.Hidden;
        public Visibility ButtonPlayVisible
        {
            get => buttonPlayVisible;
            set
            {
                buttonPlayVisible = value;
                OnPropertyChanged("ButtonPlayVisible");
            }
        }

        bool buttonPlayEnable = false;
        public bool ButtonPlayEnable
        {
            get => buttonPlayEnable;
            set
            {
                buttonPlayEnable = value;
                OnPropertyChanged("ButtonPlayEnable");
            }
        }

        Visibility buttonStopVisible = Visibility.Hidden;
        public Visibility ButtonStopVisible
        {
            get => buttonStopVisible;
            set
            {
                buttonStopVisible = value;
                OnPropertyChanged("ButtonStopVisible");
            }
        }

        bool buttonStopEnable = false;
        public bool ButtonStopEnable
        {
            get => buttonStopEnable;
            set
            {
                buttonStopEnable = value;
                OnPropertyChanged("ButtonStopEnable");
            }
        }

        Visibility textVisible = Visibility.Visible;
        public Visibility TextVisible
        {
            get => textVisible;
            set
            {
                textVisible = value;
                OnPropertyChanged("TextVisible");
            }
        }

        bool textEnable = true;
        public bool TextEnable
        {
            get => textEnable;
            set
            {
                textEnable = value;
                OnPropertyChanged("TextEnable");
            }
        }

        Visibility videoVisible = Visibility.Hidden;
        public Visibility VideoVisible
        {
            get => videoVisible;
            set
            {
                videoVisible = value;
                OnPropertyChanged("VideoVisible");
            }
        }

        bool videoEnable = false;
        public bool VideoEnable
        {
            get => videoEnable;
            set
            {
                videoEnable = value;
                OnPropertyChanged("VideoEnable");
            }
        }



        //У LoadedBehaviour-свойства есть несколько состояний, которые находятся в enum. Они имеют тип MediaState. Чтобы не использовать конвертеры, сразу укажем тип свойства MediaState
        //Manual = 0,
        //
        // Сводка:
        //     The state used to play the media. . Media will preroll automatically being playback
        //     when the System.Windows.Controls.MediaElement is assigned valid media source.
        //Play = 1,
        //
        // Сводка:
        //     The state used to close the media. All media resources are released (including
        //     video memory).
        //Close = 2,
        //
        // Сводка:
        //     The state used to pause the media. Media will preroll but remains paused when
        //     the System.Windows.Controls.MediaElement is assigned valid media source.
        //Pause = 3,
        //
        // Сводка:
        //     The state used to stop the media. Media will preroll but not play when the System.Windows.Controls.MediaElement
        //     is assigned valid media source. Media resources are not released.
        //Stop = 4
        MediaState videoState = MediaState.Pause; // По умолчанию на паузе
        public MediaState VideoState
        {
            get => videoState;
            set
            {
                videoState = value;
                OnPropertyChanged("VideoState");
            }
        }



        //Добавим объекты реализованного интерфейса ICommand для кнопок Button, ButtonPlay и ButtonStop
        private RelayCommand addCommandButton;
        public RelayCommand AddCommandButton
        {
            get
            {
                return addCommandButton ?? (addCommandButton = new RelayCommand(obj =>
                {
                    if ((PlayerPath != null) || (PlayerPath != ""))
                    {
                        //После нажатия делаем недоступными и невидимыми сами текстовый блок и кнопку для комфортного просмотра видео
                        ButtonVisible = Visibility.Hidden;
                        ButtonEnable = false;
                        TextVisible = Visibility.Hidden;
                        TextEnable = false;

                        //Отображаем видеоплеер, делаем его доступным для взаимодействия
                        VideoEnable = true;
                        VideoVisible = Visibility.Visible;

                        //"Включаем" и отображаем кнопки плей и стоп для взаимодействия с видео
                        ButtonPlayEnable = true;
                        ButtonPlayVisible = Visibility.Visible;
                        ButtonStopEnable = true;
                        ButtonStopVisible = Visibility.Visible;
                    }
                    else
                        MessageBox.Show("Введите путь до видео", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }));

            }
        }


        private RelayCommand addCommandButtonPlay;
        public RelayCommand AddCommandButtonPlay
        {
            get
            {
                return addCommandButtonPlay ?? (addCommandButtonPlay = new RelayCommand(obj =>
                {
                    playClick++;//каждое нажатие прибавляет счётчик числа нажатий (логично)
                                //Если нажато нечётное количество раз (то есть первый раз), то видео начинает проигрываться. Если чётное, то ставится на паузу.
                    if (playClick % 2 == 1)
                        VideoState = MediaState.Play;
                    else
                        VideoState = MediaState.Pause;
                    playClick %= 2; //допускаются только первое нажатие или второе нажатие (значение 1 и 0 соответственно)
                }));
            }
        }

        private RelayCommand addCommandButtonStop;
        public RelayCommand AddCommandButtonStop
        {
            get
            {
                return addCommandButtonStop ?? (addCommandButtonStop = new RelayCommand(obj =>
                {
                    //Выключаем видео
                    VideoState = MediaState.Stop;

                    //После нажатия делаем доступными и видимыми сами текстовый блок и кнопку 
                    ButtonVisible = Visibility.Visible;
                    ButtonEnable = true;
                    TextVisible = Visibility.Visible;
                    TextEnable = true;

                    //Скрываем видеоплеер, делаем его недоступным для взаимодействия
                    VideoEnable = false;
                    VideoVisible = Visibility.Hidden;

                    //"Выключаем" и скрываем кнопки плей и стоп 
                    ButtonPlayEnable = false;
                    ButtonPlayVisible = Visibility.Hidden;
                    ButtonStopEnable = false;
                    ButtonStopVisible = Visibility.Hidden;
                }));
            }
        }

        //1 кнопка отвечает за паузу и проигрывание видео. Поэтому нужен счётчик нажатий.
        byte playClick = 0;
         
        //Событие, которые вызывается при изменении состояния PlayerPath
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
