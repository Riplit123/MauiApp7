using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp7
{
    public class TaskModel
    {
        public string Name { get; set; }
        public string Category { get; set; } // "Работа", "Учёба", "Личное"
        public string Duration { get; set; } // "1 час", "5 часов", "1 день", "неделя", "месяц"
        public bool IsImportant { get; set; }
    }
}
