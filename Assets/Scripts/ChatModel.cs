using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class ChatModel {
    [RealtimeProperty(1, true, true)]
    private string _messagesJson;
}
