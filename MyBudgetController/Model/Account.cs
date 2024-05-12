using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudgetController.Model
{
    public class Account
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Currency {  get; set; }

        public override string ToString()
        {
            return $"{Name} ({Currency})";
        }
    }
}
