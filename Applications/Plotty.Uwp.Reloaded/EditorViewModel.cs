using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using ReactiveUI;

namespace Plotty.Uwp.Reloaded
{
    public class EditorViewModel : ReactiveObject
    {
        private string source;
        private int returnedValue;

        public EditorViewModel(INotificationService notificationService)
        {
            Source = "";

            var saveExtensions = new List<KeyValuePair<string, IList<string>>>
            {
                new KeyValuePair<string, IList<string>>(".c", new List<string> {".c"})
            };

            FileCommands = new FileCommands<string>(() => "", LoadFile, SaveFile, new[] { ".c" }, saveExtensions);
            FileCommands.Objects.Subscribe(s => Source = s);

            var machineLoop = CreateMachineLoop();

            PlayCommand = ReactiveCommand.CreateFromObservable(() => machineLoop
                .ObserveOnDispatcher().TakeUntil(StopCommand));


            PlayCommand.ThrownExceptions.Subscribe(exception => { });
            var executionFinished = PlayCommand.IsExecuting.Skip(1).Where(isExecuting => isExecuting == false);

            StopCommand = ReactiveCommand.Create(() => { }, PlayCommand.IsExecuting);
            var lastState = executionFinished.WithLatestFrom(PlayCommand, (_, state) => state);

            lastState.Subscribe(state =>
            {
                ReturnedValue = state.ReturnedValue;
                notificationService.Show($"Execution completed! Returned value: {ReturnedValue}",
                    TimeSpan.FromSeconds(2));
                Results.Add(ReturnedValue);
            });
        }

        private IObservable<MachineState> CreateMachineLoop()
        {
            var scheduler = new EventLoopScheduler();
            return Observable.Using(() => new MachineContext(source), context => Observable
                .Generate(0, x => context.CanExecute, x => x, x =>
                {
                    context.Execute();
                    return context.State;
                }, scheduler));
        }

        public FileCommands<string> FileCommands { get; }

        private static async Task<string> LoadFile(IStorageFile arg)
        {
            using (var reader = new StreamReader(await arg.OpenStreamForReadAsync()))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private async Task SaveFile(IStorageFile arg)
        {
            using (var reader = new StreamWriter(await arg.OpenStreamForWriteAsync()))
            {
                await reader.WriteAsync(source);
            }
        }

        public int ReturnedValue
        {
            get => returnedValue;
            set => this.RaiseAndSetIfChanged(ref returnedValue, value);
        }

        public ReactiveCommand<Unit, Unit> StopCommand { get; }

        public string Source
        {
            get => source;
            set => this.RaiseAndSetIfChanged(ref source, value);
        }

        public ReactiveCommand<Unit, MachineState> PlayCommand { get; }

        public ObservableCollection<int> Results { get; } = new ObservableCollection<int>();
    }

    public class LoadFacade<T>
    {
    }
}