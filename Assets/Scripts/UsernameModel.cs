using Normal.Realtime;

[RealtimeModel]
public partial class UsernameModel {
    [RealtimeProperty(1, true, true)] 
    private string _username;
}
