using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.AI
{
    public interface IAIService
    {
        Task<string> ChatAsync(string message);
        Task<string> GetHintAsync(string question);
    }
}
