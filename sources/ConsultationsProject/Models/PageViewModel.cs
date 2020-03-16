using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultationsProject.Models
{
    /// <summary>
    /// Модель пагинации.
    /// </summary>
    public class PageViewModel
    {
        /// <summary>
        /// Номер текущей страницы.
        /// </summary>
        public int PageNumber { get; private set; }

        /// <summary>
        /// Всего страниц.
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// Конструктор пагинации.
        /// </summary>
        /// <param name="count">Количество элементов в БД.</param>
        /// <param name="pageNumber">Номер текущей страницы.</param>
        /// <param name="pageSize">Размер страницы (кол-во элементов на странице).</param>

        public PageViewModel(int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        /// <summary>
        /// Проверка на наличие предыдущей страницы.
        /// </summary>

        public bool HasPreviousPage
        {
            get
            {
                return (PageNumber > 1);
            }
        }

        /// <summary>
        /// Проверка на наличие следующей страницы.
        /// </summary>
        public bool HasNextPage
        {
            get
            {
                return (PageNumber < TotalPages);
            }
        }
    }
}
