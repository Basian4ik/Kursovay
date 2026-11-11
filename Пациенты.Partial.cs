using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Поликлиника
{
    public partial class Пациенты
    {
        [NotMapped] // Говорим Entity Framework, что этого поля нет в БД
        public int Возраст
        {
            get
            {
                // Если вдруг дата по умолчанию (01.01.0001), возвращаем 0
                if (Дата_рождения == default(System.DateTime))
                    return 0;

                var today = System.DateTime.Today;
                int age = today.Year - Дата_рождения.Year;

                // Если день рождения в этом году еще не прошел — вычитаем год
                if (Дата_рождения.Date > today.AddYears(-age))
                    age--;

                return age;
            }
        }
    }
}
