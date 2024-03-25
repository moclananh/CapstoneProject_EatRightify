using Component.ViewModels.Utilities.Slides;

namespace Component.Application.Utilities.Slides
{
    public interface ISlideService
    {
        Task<List<SlideVm>> GetAll();
    }
}
