using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCore.Engine
{
    /// <summary>
    /// Обеспечивает конфигурацию приложения
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Количество строк файлового дерева на страницу
        /// </summary>
        public int LinesOnPage { get; set; }
        /// <summary>
        /// Последний каталог, на котором остановился пользователь
        /// </summary>
        public string CurrentDir { get; set; }
    }
}