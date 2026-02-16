namespace BookNest.MVC.Areas.AdminPanel.ViewModels
{
    public class DashboardVM
    {
        public int TotalBooks { get; set; }
        public int TotalAuthors { get; set; }
        public int TotalGenres { get; set; }
        public int TotalReviews { get; set; }
        public int TotalUsers { get; set; }

        public string LatestBookTitle { get; set; }
        public string LatestReviewerName { get; set; }

        public string AdminName { get; set; }
    }
}
