using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OCSFX.EZFMOD;
using UnityEditor;
using UnityEngine;

namespace OCSFX.EZFMODEditor.Metadata.Deserialization
{
    public static class EZFMODProjectBankFileWatcher
    {
        private static string _currentWatchPath;
        private static FileSystemWatcher _fileWatcher;
        private static bool _processingChanges;
        private static readonly object _LOCK_OBJECT = new object();
        private static CancellationTokenSource _cancellationTokenSource;
        private const string _BANK_FILE_EXTENSION = "*.bank";

        [InitializeOnLoadMethod]
        public static void Startup()
        {
            _fileWatcher = new FileSystemWatcher();
            _fileWatcher.IncludeSubdirectories = true;
            _fileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            _fileWatcher.Filter = _BANK_FILE_EXTENSION;

            _fileWatcher.Changed += OnFileChanged;
            _fileWatcher.Created += OnFileChanged;
            _fileWatcher.Deleted += OnFileChanged;

            EditorApplication.quitting += OnEditorQuitting;

            UpdateFileWatcherPath();

            OnEditorStartup();
        }

        private static void OnEditorStartup()
        {
            var ezFmodSettings = EZFMODSettings.Get();
            if (ezFmodSettings && !ezFmodSettings.ReconcileAtStartup) return;

            // Reconcile FMOD Project Metadata once on startup
            if (SessionState.GetBool($"{nameof(EZFMODProjectBankFileWatcher)}{nameof(OnEditorStartup)}", false)) return;

            CheckFilesChanged();

            SessionState.SetBool($"{nameof(EZFMODProjectBankFileWatcher)}{nameof(OnEditorStartup)}", true);
        }

        private static Task _bufferTask;

        private static void OnFileChanged(object source, FileSystemEventArgs e)
        {
            var ezFmodSettings = EZFMODSettings.Get();
            if (ezFmodSettings && !ezFmodSettings.ReconcileOnBankImport) return;

            UpdateFileWatcherPath();
            
            _cancellationTokenSource?.Cancel();
            
            _cancellationTokenSource = new CancellationTokenSource();

            // Start the buffer task
            _bufferTask = FileChangeBuffer(_cancellationTokenSource.Token);
        }

        private static async Task FileChangeBuffer(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    // Debug.Log("Starting buffer delay...");
                    await Task.Delay(1000, cancellationToken);

                    lock (_LOCK_OBJECT)
                    {
                        if (_processingChanges)
                        {
                            // Debug.Log("Processing changes already in progress, exiting buffer.");
                            return;
                        }

                        // Check if there were any new changes during the delay
                        if (_cancellationTokenSource.IsCancellationRequested)
                        {
                            // Debug.Log("New file change detected, restarting buffer delay.");
                            _cancellationTokenSource = new CancellationTokenSource();
                            continue;
                        }

                        _processingChanges = true;
                    }

                    // Debug.Log("Buffer period ended, checking files changed.");
                    CheckFilesChanged();
                    break;
                }
            }
            catch (TaskCanceledException)
            {
                // Debug.Log("File change buffer task canceled.");
            }
        }

        private static void CheckFilesChanged()
        {
            Debug.Log($"[{nameof(EZFMOD)}] Scheduling FMOD project metadata deserialization...");
            EditorApplication.update += RunDeserializeOnMainThread;
        }

        private static async void RunDeserializeOnMainThread()
        {
            try
            {
                EditorApplication.update -= RunDeserializeOnMainThread;
                
                await Task.Delay(500); // Wait for half a second before processing, for safety.

                var blockingProcess = false;
                var waitForProcess = string.Empty;
                var printedWaitForProcess = string.Empty;

                while (
                    EditorApplication.isCompiling
                    || EditorApplication.isUpdating
                    || EditorApplication.isPlayingOrWillChangePlaymode
                    || !FMODUnity.EventManager.IsInitialized
                    || !FMODUnity.EditorUtils.System.isValid()
                )
                {
                    waitForProcess =
                        EditorApplication.isCompiling ? "Editor compilation"
                        : EditorApplication.isUpdating ? "Editor update"
                        : EditorApplication.isPlayingOrWillChangePlaymode ? "Play Mode change"
                        : !FMODUnity.EventManager.IsInitialized ? $"{nameof(FMODUnity.EventManager)} initialization"
                        : $"{nameof(FMODUnity.EditorUtils)} System initialization";

                    blockingProcess = true;

                    if (waitForProcess != printedWaitForProcess)
                    {
                        Debug.Log($"[{nameof(EZFMOD)}] Waiting for {waitForProcess}...");
                        printedWaitForProcess = waitForProcess;
                    }

                    await Task.Delay(200); // Wait for .2 seconds before checking again
                }

                if (blockingProcess)
                {
                    Debug.Log($"[{nameof(EZFMOD)}] {waitForProcess} finished. Updating EZFMOD metadata...");
                }

                await Task.Delay(500); // Wait an extra .5 seconds for safety

                EZFMODMetadataDeserializer.DeserializeFMODProjectMetadata();

                lock (_LOCK_OBJECT)
                {
                    _processingChanges = false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(EZFMOD)}] Error deserializing FMOD project metadata: {e}");
                lock (_LOCK_OBJECT)
                {
                    _processingChanges = false;
                }
            }
        }

        private static void OnEditorQuitting()
        {
            EditorApplication.quitting -= OnEditorQuitting;

            // Clean up
            _fileWatcher?.Dispose();
        }

        private static void UpdateFileWatcherPath()
        {
            var fmodProjectBankPath = FMODUnity.Settings.Instance.SourceBankPath;

            var pathToWatch = Path.IsPathRooted(fmodProjectBankPath)
                ? Path.GetFullPath(fmodProjectBankPath)
                : Path.GetFullPath(Environment.CurrentDirectory + "/" + fmodProjectBankPath);

            // No change
            if (_currentWatchPath == pathToWatch) return;

            OnWatchPathChanged(pathToWatch, fmodProjectBankPath);
        }

        private static void OnWatchPathChanged(string newWatchPath, string fmodProjectBankPath)
        {
            _currentWatchPath = newWatchPath;

            try
            {
                _fileWatcher.EnableRaisingEvents = false;
                _processingChanges = false;

                if (string.IsNullOrEmpty(fmodProjectBankPath)) return;

                _fileWatcher.Path = newWatchPath;
                _fileWatcher.EnableRaisingEvents = true;
            }
            catch (ArgumentException e)
            {
                Debug.LogError($"[{nameof(EZFMOD)}] Error changing watch path: {e}");
            }
        }
    }
}