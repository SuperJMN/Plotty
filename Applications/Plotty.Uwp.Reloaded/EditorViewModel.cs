using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
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

            NewFileCommand = ReactiveCommand.Create(() => "" );

            OpenFileCommand = ReactiveCommand.CreateFromTask(PickFileToOpen);

            OpenFileCommand
                .Where(file => file != null)
                .SelectMany(LoadFile)
                .Merge(NewFileCommand)
                .ObserveOnDispatcher()
                .Subscribe(s => Source = s);

            OpenFileCommand.ThrownExceptions.Subscribe(exception => { });
            
            SaveFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var pickFileToSave = await PickFileToSave();
                if (pickFileToSave != null)
                {
                    await SaveFile(pickFileToSave);
                }
            }, this.WhenAnyValue(x => x.Source, s => !string.IsNullOrEmpty(s)));

            SaveFileCommand.ThrownExceptions.Subscribe(exception => { });


            var scheduler = new EventLoopScheduler();

            var machineLoop = Observable.Using(() => new MachineContext(source), context => Observable
                    .Generate(0, x => context.CanExecute, x => x, x =>
                    {
                        context.Execute();
                        return context.State;
                    }, scheduler));

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

        public ReactiveCommand<Unit, IStorageFile> OpenFileCommand { get; }

        public ReactiveCommand<Unit, Unit> SaveFileCommand { get; set; }

        public ReactiveCommand<Unit, string> NewFileCommand { get; set; }

        public int ReturnedValue
        {
            get => returnedValue;
            set => this.RaiseAndSetIfChanged(ref returnedValue, value);
        }

        public ReactiveCommand<Unit, int> AnotherCommand { get; set; }

        public ReactiveCommand<Unit, Unit> StopCommand { get; }

        private async Task SaveFile(IStorageFile file)
        {          
            using (var stream = new StreamWriter(await file.OpenStreamForWriteAsync()))
            {          
                stream.Write(source);
            }
        }
        
        public string Source
        {
            get => source;
            set => this.RaiseAndSetIfChanged(ref source, value);
        }

        public ReactiveCommand<Unit, MachineState> PlayCommand { get; }

        public ObservableCollection<int> Results { get; } = new ObservableCollection<int>();

        private async Task<string> LoadFile(IStorageFile file)
        {
            using (var stream = new StreamReader(await file.OpenStreamForReadAsync()))
            {
                return stream.ReadToEnd();
            }
        }

        private async Task<IStorageFile> PickFileToOpen()
        {          
            var picker = new FileOpenPicker
            {
                CommitButtonText = "Open",
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            picker.FileTypeFilter.Add(".c");
            picker.FileTypeFilter.Add(".txt");

            return await picker.PickSingleFileAsync();
        }

        private async Task<IStorageFile> PickFileToSave()
        {

            var picker = new FileSavePicker
            {
                CommitButtonText = "Save",
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            picker.FileTypeChoices.Add(new KeyValuePair<string, IList<string>>(".c", new List<string>() { ".c" }));

            return await picker.PickSaveFileAsync();
        }
    }
}