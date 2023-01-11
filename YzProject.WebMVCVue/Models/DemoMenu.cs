namespace YzProject.WebMVCVue.Models
{
    public class DemoMenu
    {
        public DemoMenu(string _title, string _path, string _name, string _icon, string _id, string _parentId)
        {
            title = _title;
            path = _path;
            name = _name;
            icon = _icon;
            id = _id;
            parentId = _parentId;

        }
        public string id { set; get; }
        public string parentId { set; get; }
        public string name { set; get; }
        public string title { set; get; }
        public string icon { set; get; }
        public string path { get; set; }
    }
}
