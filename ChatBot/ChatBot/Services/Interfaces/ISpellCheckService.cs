using System.Threading.Tasks;

namespace ChatBot.Ultilities.Interfaces
{
    public interface ISpellCheckService
    {
        Task<string> GetCorrectedTextAsync(string text);
    }
}