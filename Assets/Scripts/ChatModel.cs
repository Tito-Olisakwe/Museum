using Normal.Realtime;
using Normal.Realtime.Serialization;
using System.Collections.Generic;

[RealtimeModel]
public partial class ChatModel {
    [RealtimeProperty(1, true, true)]
    private string _latestMessage;
}
