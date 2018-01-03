using System;
using ReactiveUI;

namespace Plotty.Uwp
{
    public class ConsoleHandler : ReactiveObject
    {
        private readonly int[] memory;
        private char[,] screen;
        public int Rows { get; }
        public int Columns { get; }

        public char[,] Screen
        {
            get { return screen; }
            set
            {
                screen = value;
                this.RaisePropertyChanged();
            }
        }

        public ConsoleHandler(int[] memory, int columns, int rows)
        {
            this.memory = memory;
            Rows = rows;
            Columns = columns;

            Screen = new char[columns, rows];
        }

       

        public void Update()
        {           
            var convertAll = Array.ConvertAll(memory, input => (char)input);
            var newScreen = new char[Rows, Columns];

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    newScreen[x, y] = convertAll[y * Columns + x];
                }
            }

            Screen = newScreen;
        }
    }
}