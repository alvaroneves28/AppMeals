using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMeals.Services
{
    public class ApiResponse<T>
    {
        public T? data { get; set; }

        public string? ErrorMessage { get; set; }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
        public bool Success { get; set; } = false;

        public T? Data { get; set; }

    }

}
