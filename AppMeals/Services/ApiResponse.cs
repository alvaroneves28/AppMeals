using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMeals.Services
{
    public class ApiResponse<T>
    {
        
        public string? ErrorMessage { get; set; }

        public bool Success { get; set; } = false;

        public T? Data { get; set; }

    }

}
