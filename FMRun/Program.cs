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
    /// <summary>
    /// Главный класс приложения, обеспечивающий формирование единого приложения с пользовательским интерфейсом
    /// посредством использования механизмов сборки (.dll) FmCore
    /// </summary>
    internal class Program
    {
        static readonly string configPath = $"{Directory.GetCurrentDirectory()}\\appconfig.json";
        static readonly string logDir = $"{Directory.GetCurrentDirectory()}\\log\\";
        static readonly string errorsDir = $"{logDir}\\errors\\";

        static Config appConfig;
        static int startIndex = 0;
        static int currentIndex = 0;
        static string currentCatalog = string.Empty;

        static string sourceFileToCopy;
        static List<string> directoryCopyBuffer = new List<string>();



        static PageManager pageManager;

        static void Main(string[] args)
        {
            Directory.CreateDirectory(logDir);
            Directory.CreateDirectory(errorsDir);
            appConfig = ReadConfig();
            if (appConfig is not null)
            {
                Log($"[{DateTime.Now}] Прочитан конфигурационный файл. Приложение запущено");
                if (appConfig.LinesOnPage < 34)
                {
                    pageManager = new PageManager(appConfig);
                    currentCatalog = appConfig.CurrentDir;
                    currentIndex = startIndex;

                    pageManager.MakePage(currentIndex, ref currentCatalog);
                    ProcessUserInput();
                }
            }
            else
            {
                Log($"Отсутствует файл конфигурации. Выход из приложения");
            }
        }

        /// <summary>
        /// Обеспечивает создание объекта класса Config, представляющего собой класс конфигурации приложения
        /// </summary>
        /// 
        /// <returns>Объект класса Config</returns>
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
                Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Обеспечивает обработку нажатия пользователем клавиш для выполнения команд
        /// </summary>
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
                                pageManager.MakePage(currentIndex, ref currentCatalog);
                            }
                        }
                        continue;
                    case ConsoleKey.DownArrow:
                        {
                            if (currentIndex < pageManager.MaxIndex)
                            {
                                currentIndex += 1;
                                pageManager.MakePage(currentIndex, ref currentCatalog);
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
                                pageManager.MakePage(currentIndex, ref currentCatalog);
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
                                pageManager.MakePage(currentIndex, ref currentCatalog);
                            }
                        }
                        continue;
                    case ConsoleKey.Enter:
                        {
                            string item = pageManager.SelectedItem;
                            if (File.Exists(item))
                            {
                                Process.Start(new ProcessStartInfo() { FileName = item, UseShellExecute = true });
                                pageManager.MakePage(currentIndex, ref currentCatalog);
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
                            pageManager.MakePage(currentIndex, ref currentCatalog);
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
                            pageManager.MakePage(currentIndex, ref currentCatalog);
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
                                        Log($"{directoryCopyBuffer[i]} удален");
                                    }
                                    catch (Exception ex)
                                    {
                                        Error(ex);
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
                                    Log(status);
                                }
                                catch (Exception ex)
                                {
                                    status = $"Ошибка удаления файла {selectedItem}";
                                    pageManager.Status = status;
                                    Error(ex);
                                }
                            }
                            pageManager.MakePage(currentIndex, ref currentCatalog);
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
                Log("Приложение завершило работу по команде от пользователя. Текущее состояние сохранено в файле конфигурации");
                break;
            }
        }

        /// <summary>
        /// Обеспечивает обнаружение и переход по логическим дискам по их метке, вводимой пользователем с клавиатуры
        /// </summary>
        /// <param name="path">Метка диска для перемещения</param>
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
            pageManager.MakePage(currentIndex, ref currentCatalog);
            ProcessUserInput();
        }

        /// <summary>
        /// Обеспечивает рекурсивный обход директории, указанной в параметрах и сохранение 
        /// путей к файлам и папкам в буферный список (что-то наподобие стека)
        /// </summary>
        /// <param name="dirPath">Путь к каталогу для сохранения его внутренней структуры</param>
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
                Error(ex);
            }
        }

        /// <summary>
        /// Обеспечивает сохранение пути к файлу, который необходимо скопировать в специальное поле
        /// </summary>
        /// <param name="filePath">Путь к файлу для копирования</param>
        /// <returns>Статус запрошенной операции</returns>
        static string CopyFile(string filePath)
        {
            sourceFileToCopy = filePath;
            string status = $"Файл {new FileInfo(filePath).Name} выбран для копирования";
            Log(status);
            return status;
        }
        
        /// <summary>
        /// СОздать структуру каталогов и файлов в указанном каталоге из специального буфера
        /// </summary>
        /// <param name="dirPath">Путь для копирования</param>
        /// <returns>Статус операции</returns>
        static string PasteDirectory(string dirPath)
        {
            if (directoryCopyBuffer != null)
            {
                string pathForSave = dirPath;
                int oldRootDirLength = directoryCopyBuffer[0].Length - new DirectoryInfo(directoryCopyBuffer[0]).Name.Length;
                for (int i = 0; i < directoryCopyBuffer.Count; i++)
                {
                    try
                    {
                        if (Directory.Exists(directoryCopyBuffer[i]))
                        {
                            pathForSave = $"{dirPath}\\{new DirectoryInfo(directoryCopyBuffer[i]).FullName.Substring(oldRootDirLength)}";
                            Directory.CreateDirectory(pathForSave);
                        }
                        File.Copy(directoryCopyBuffer[i], $"{dirPath}\\{new FileInfo(directoryCopyBuffer[i]).FullName.Substring(oldRootDirLength)}");
                        Log($"{directoryCopyBuffer[i]} скопирован");
                    }
                    catch (Exception ex)
                    {
                        Error(ex);
                        continue;
                    }
                }
                return $"Каталог {directoryCopyBuffer[0]} скопирован в каталог {dirPath}";
            }
            return string.Empty;
        }

        /// <summary>
        /// Вставка файла. Источник - буферное поле, назначение - аргумент
        /// </summary>
        /// <param name="filePath">Путь для вставки файла</param>
        /// <returns>Статус операции</returns>
        static string PasteFile(string filePath)
        {
            try
            {
                File.Copy(sourceFileToCopy, $"{filePath}\\{new FileInfo(sourceFileToCopy).Name}");
                sourceFileToCopy = string.Empty;
                Log($"{sourceFileToCopy} скопирован");
                return $"Файл {new FileInfo(sourceFileToCopy).Name} скопирован в каталог {currentCatalog}\\";
            }
            catch (Exception ex)
            {
                Error(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Обеспечивает логгирование
        /// </summary>
        /// <param name="logText">Текст, который включается в лог</param>
        static void Log(string logText)
        {
            if (string.IsNullOrWhiteSpace(logText))
            {
                return;
            }

            string exceptionsDir = errorsDir;
            string logFilePath = $"{logDir}INFO.txt";

            File.AppendAllText(logFilePath, $"[{DateTime.Now}] {logText}\n"); ;
        }

        /// <summary>
        /// Перегрузка метода логгирования для журналирования ошибок
        /// </summary>
        /// <param name="ex">Объект исключения</param>
        static void Error(Exception ex)
        {
            string logFilePath = $"{errorsDir}{ex.GetType()}.error";

            File.WriteAllText(logFilePath, $"[{DateTime.Now}] {ex.Message}\n");
        }

        /// <summary>
        /// Обеспечивает сохранение состояния приложения в файл конфигурации (каталог и настройка вывода строк на экран) 
        /// </summary>
        static void SaveState()
        {
            appConfig.CurrentDir = currentCatalog;
            try
            {
                string json = JsonSerializer.Serialize<Config>(appConfig);
                File.WriteAllText(configPath, json);
                Log($"Текущее состояние файлового менеджера сохранено в файле конфигурации {configPath}");
            }
            catch (Exception ex)
            {
                Error(ex);
            }

        }
    }
}