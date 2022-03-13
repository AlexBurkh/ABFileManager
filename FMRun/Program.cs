using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using FMCore.Engine;
using FMCore.Models.UI.Pages;
using FMCore.Models.CatalogTree;
using System.Text.Json;

namespace FMRun
{
    internal class Program
    {
        static readonly string configPath = $"{Directory.GetCurrentDirectory()}\\appconfig.json";
        static readonly string logDir = $"{Directory.GetCurrentDirectory()}\\log\\";
        static readonly string errorsDir = $"{logDir}\\errors\\";

        static Config appConfig;
        static int currentIndex = 0;
        static string currentCatalog = string.Empty;

        static string sourceFileToCopy;
        static List<string> directoryCopyBuffer = new List<string>();



        static PageManager pageManager;

        static void Main(string[] args)
        {
            
            appConfig = ReadConfig(); 
            pageManager = new PageManager(appConfig);
            currentCatalog = appConfig.CurrentDir;
            currentIndex = appConfig.CurrentIndex;
            Directory.CreateDirectory(logDir);
            Directory.CreateDirectory(errorsDir);

            pageManager.MakePage(currentIndex, currentCatalog);
            ProcessUserInput();
        }


        static Config ReadConfig()
        {
            try
            {
                string json = File.ReadAllText(configPath);
                Config config = JsonSerializer.Deserialize<Config>(json);
                return config;
            }
            catch (Exception ex)
            {
                Log(ex);
                return null;
            }
        }
        static void ProcessUserInput()
        {
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        {
                            if (currentIndex > 0)
                            {
                                currentIndex -= 1;
                                pageManager.MakePage(currentIndex, currentCatalog);
                            }
                        }
                        continue;
                    case ConsoleKey.DownArrow:
                        {
                            if (currentIndex < pageManager.MaxIndex)
                            {
                                currentIndex += 1;
                                pageManager.MakePage(currentIndex, currentCatalog);
                            }
                        }
                        continue;
                    case ConsoleKey.LeftArrow:
                        {
                            DirectoryInfo parentDir = new DirectoryInfo(pageManager.CurrentWorkDir).Parent;
                            if (parentDir != null)
                            {
                                currentCatalog = parentDir.FullName;
                                currentIndex = 0;
                                pageManager.MakePage(currentIndex, currentCatalog);
                            }
                        }
                        continue;
                    case ConsoleKey.RightArrow:
                        {
                            string item = pageManager.SelectedItem;
                            if (Directory.Exists(item))
                            {
                                currentCatalog = item;
                                currentIndex = 0;
                                pageManager.MakePage(currentIndex, currentCatalog);
                            }
                        }
                        continue;
                    case ConsoleKey.Enter:
                        {
                            string item = pageManager.SelectedItem;
                            if (File.Exists(item))
                            {
                                Process.Start(new ProcessStartInfo() { FileName = item, UseShellExecute = true });
                                pageManager.MakePage(currentIndex, currentCatalog);
                            }
                        }
                        continue;
                    case ConsoleKey.F1:
                        {
                            string item = pageManager.SelectedItem;
                            string status = string.Empty;
                            if (Directory.Exists(item))
                            {
                                WalkDirectory(item);
                                if (directoryCopyBuffer != null)
                                {
                                    status = $"Каталог {item} скопирован в память";
                                    Log(status);
                                }
                            }
                            else
                            {
                                status = CopyFile(item);
                                Log($"Файл {item} скопирован в память");
                            }
                            pageManager.Status = status;
                            pageManager.MakePage(currentIndex, currentCatalog);
                        }
                        continue;
                    case ConsoleKey.F2:
                        {
                            string item = pageManager.SelectedItem;
                            string status = string.Empty;
                            if (Directory.Exists(item))
                            {
                                if (!string.IsNullOrWhiteSpace(sourceFileToCopy))
                                {
                                    status = PasteFile(item);
                                }
                                else
                                {
                                    status = PasteDirectory(item);
                                    directoryCopyBuffer.Clear();
                                }
                            }
                            pageManager.Status = status;
                            pageManager.MakePage(currentIndex, currentCatalog);
                        }
                        continue;
                    case ConsoleKey.F3:
                        {
                            var selectedItem = pageManager.SelectedItem;
                            if (Directory.Exists(selectedItem))
                            {
                                WalkDirectory(selectedItem);

                                for (int i = directoryCopyBuffer.Count - 1; i >= 0; i--)
                                {
                                    try
                                    {
                                        if (Directory.Exists(directoryCopyBuffer[i]))
                                        {
                                            Directory.Delete(directoryCopyBuffer[i]);
                                            continue;
                                        }
                                        File.Delete(directoryCopyBuffer[i]);
                                        Log($"{directoryCopyBuffer[i]} удален" + '\n');
                                    }
                                    catch (Exception ex)
                                    {
                                        Log(ex);
                                    }
                                }

                                directoryCopyBuffer.Clear();
                            }
                            else
                            {
                                string status = string.Empty;
                                try
                                {
                                    File.Delete(selectedItem);
                                    status = $"Файл {selectedItem} удален";
                                    pageManager.Status = status;
                                    Log(status + '\n');
                                }
                                catch (Exception ex)
                                {
                                    status = $"Ошибка удаления файла {selectedItem}";
                                    pageManager.Status = status;
                                    Log(ex);
                                }
                            }
                            pageManager.MakePage(currentIndex, currentCatalog);
                        }
                        continue;
                    case ConsoleKey.Escape:
                    case ConsoleKey.F10:
                        break;
                    default:
                        {
                            if ((keyInfo.Key > ConsoleKey.A) && (keyInfo.Key < ConsoleKey.Z))
                            {
                                ChangeDrive((char) keyInfo.Key);
                            }
                        }
                        continue;
                }
                SaveState();
                break;
            }
        }

        static void ChangeDrive(char path)
        {
            string possibleLogicalDrive = $"{path}:\\";
            if (Directory.Exists(possibleLogicalDrive))
            {
                currentCatalog = possibleLogicalDrive;
                pageManager.Status = $"Выполнен переход к логическому диску: {possibleLogicalDrive}";
            }
            else
            {
                pageManager.Status = $"Логический диск: {possibleLogicalDrive} не обнаружен";
            }
            pageManager.MakePage(currentIndex, currentCatalog);
            ProcessUserInput();
        }

        static void WalkDirectory(string dirPath)
        {
            try
            {
                if (Directory.Exists(dirPath))
                {
                    DirectoryInfo root = new DirectoryInfo(dirPath);
                    directoryCopyBuffer.Add(root.FullName);
                    FileInfo[] files = root.GetFiles();
                    for (int i = 0; i < files.Length; i++)
                    {
                        directoryCopyBuffer.Add(files[i].FullName);
                    }
                    DirectoryInfo[] dirs = root.GetDirectories();
                    for (int i = 0; i < dirs.Length; i++)
                    {
                        WalkDirectory(dirs[i].FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }
        static string CopyFile(string filePath)
        {
            sourceFileToCopy = filePath;
            string status = $"Файл {new FileInfo(filePath).Name} выбран для копирования";
            Log(status + '\n');
            return status;
        }
        
        static string PasteDirectory(string dirPath)
        {
            if (directoryCopyBuffer != null)
            {
                string pathForSave = dirPath;
                for (int i = 0; i < directoryCopyBuffer.Count; i++)
                {
                    try
                    {
                        if (Directory.Exists(directoryCopyBuffer[i]))
                        {
                            pathForSave += $"\\{new DirectoryInfo(directoryCopyBuffer[i]).Name}";
                            Directory.CreateDirectory(pathForSave);
                        }
                        File.Copy(directoryCopyBuffer[i], $"{pathForSave}\\{new FileInfo(directoryCopyBuffer[i]).Name}");
                        Log($"{directoryCopyBuffer[i]} скопирован" + '\n');
                    }
                    catch (Exception ex)
                    {
                        Log(ex);
                        continue;
                    }
                }
                return $"Каталог {directoryCopyBuffer[0]} скопирован в каталог {dirPath}";
            }
            return string.Empty;
        }
        static string PasteFile(string filePath)
        {
            try
            {
                File.Copy(sourceFileToCopy, $"{filePath}\\{new FileInfo(sourceFileToCopy).Name}");
                sourceFileToCopy = string.Empty;
                Log($"{sourceFileToCopy} скопирован" + '\n');
                return $"Файл {new FileInfo(sourceFileToCopy).Name} скопирован в каталог {currentCatalog}\\";
            }
            catch (Exception ex)
            {
                Log(ex);
                return string.Empty;
            }
        }

        static void Log(string logText)
        {
            if (string.IsNullOrWhiteSpace(logText))
            {
                return;
            }

            string exceptionsDir = errorsDir;
            string logFilePath = $"{logDir}INFO.txt";

            File.AppendAllText(logFilePath, $"[{DateTime.Now}] {logText}"); ;
        }
        static void Log(Exception ex)
        {
            string logFilePath = $"{errorsDir}{ex.GetType()}.error";

            File.AppendAllText(logFilePath, $"[{DateTime.Now}] {ex.Message}");
        }
        static void SaveState()
        {
            appConfig.CurrentDir = currentCatalog;
            appConfig.CurrentIndex = currentIndex;
            try
            {
                string json = JsonSerializer.Serialize<Config>(appConfig);
                File.WriteAllText(configPath, json);
                Log($"Текущее состояние файлового менеджера сохранено в файле конфигурации {configPath}");
            }
            catch (Exception ex)
            {
                Log(ex);
            }

        }
    }
}
