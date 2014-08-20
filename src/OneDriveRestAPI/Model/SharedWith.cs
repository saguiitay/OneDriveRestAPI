namespace OneDriveRestAPI.Model
{
    public class SharedWith
    {
        ///<summary>
        ///  Info about who can access the folder. The options are:
        ///  - People I selected
        ///  - Just me
        ///  - Everyone (public)
        ///  - Friends
        ///  - My friends and their friends
        ///  - People with a link
        ///  The default is Just me.
        ///</summary>
        public string Access { get; set; }
    }
}