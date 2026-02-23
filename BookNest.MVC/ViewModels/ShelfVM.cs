using BookNest.Core.Entities;

namespace BookNest.MVC.ViewModels
{
    public class ShelfVM
    {
        public List<ReadingList> ReadingLists { get; set; }
        public int CurrentStatus { get; set; }
    }
}