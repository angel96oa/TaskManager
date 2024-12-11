namespace TaskManager.Data
{
    public class TaskElement
    {
        public int id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime TaskCreateDate { get; set; } = DateTime.Now;
        public DateTime finishTaskDate { get; set; }
        public string status { get; set; } = string.Empty;
    }
}