
namespace Shared
{
    public enum NetMessageType : byte
    {
        // Client to server
        Move = 0, // float dx, float dy

        // Server to client
        SetPosition = 10, // int playerId, float x, float y
    }
}
