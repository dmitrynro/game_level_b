
namespace Shared
{
    public enum NetMessageType : byte
    {
        // Client to server
        SetMoveDirection = 0, // float dx, float dy

        // Server to client
        ClientConnected = 10, // int gameObjectId
        ClientDisconnected = 11, // int gameObjectId
        SetClientId = 12, // int gameObjectId
        SetPosition = 13, // int gameObjectId, float x, float y
        SetVelocity = 14, // int gameObjectId, float dx, float dy
    }
}
