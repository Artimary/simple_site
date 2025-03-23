using Domain.Entities;
using Controllers;

namespace Application.Interfaces
{
    public interface IMouseService
    {
        Task SaveDataAsync(List<MouseEvent> events);
    }
}