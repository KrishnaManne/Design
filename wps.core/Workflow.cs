namespace wps.core
{
    public class Workflow
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string RunAfter { get; set; }
        public List<Activity> Activities { get; set; }
    }
}